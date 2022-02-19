using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  /// <summary>
  /// A standard grain.
  /// </summary>
  public class AgentGrain : Grain, IAgentGrain
  {
    private readonly IPersistentState<AgentState> _agentState;
    private IAsyncStream<ChatMessage> _agentChatOutStream;
    private StreamSubscriptionHandle<ChatMessage> _roomChatOutSubscription;

    public AgentGrain([PersistentState("agentState", "agentStateStore")] IPersistentState<AgentState> agentState)
    {
      _agentState = agentState;
    }
    public override async Task OnActivateAsync()
    {
      // On grain activation, recover any subscription handle which was already registered
      var streamProvider = GetStreamProvider("SMSProvider");
      if (_agentState.State?.RoomId != Guid.Empty)
      {
        var stream = streamProvider.GetStream<ChatMessage>(_agentState.State.RoomId, "RoomChat.Out");
        var subscriptionHandles = await stream.GetAllSubscriptionHandles();
        if (subscriptionHandles?.Count > 0)
        {
          // We're only listening to one room at a time.
          _roomChatOutSubscription = subscriptionHandles.First();
          await _roomChatOutSubscription.ResumeAsync(OnNextChatMessage,
                                                     OnErrorChatMessage,
                                                     OnCompletedChatMessage);
        }
      }
      // ???? Here or constructor ??
      _agentChatOutStream = streamProvider.GetStream<ChatMessage>(this.GetPrimaryKey(), "AgentChat.Out");

      await base.OnActivateAsync();
    }

    private async Task OnNextChatMessage(ChatMessage message, StreamSequenceToken token)
    {
      // Forward room chat into agent chat
      await _agentChatOutStream.OnNextAsync(message);
    }
    private async Task OnErrorChatMessage(Exception e)
    {
      // Forward room chat into agent chat
      await _agentChatOutStream.OnErrorAsync(e);
    }
    private async Task OnCompletedChatMessage()
    {
      // Forward room chat into agent chat
      await _agentChatOutStream.OnCompletedAsync();
    }
    public async Task Configure(string name, Guid roomId)
    {
      _agentState.State = new AgentState(roomId, new AgentPointer { Key = this.GetPrimaryKey(), Name = name });
      await _agentState.WriteStateAsync();
    }

    public async Task TransferRoom(Guid roomId)
    {
      if (_roomChatOutSubscription != null)
      {
        await _roomChatOutSubscription.UnsubscribeAsync();
      }
      var streamProvider = GetStreamProvider("SMSProvider");
      var stream = streamProvider.GetStream<ChatMessage>(roomId, "RoomChat.Out");

      _roomChatOutSubscription = await stream.SubscribeAsync(OnNextChatMessage,
                                                             OnErrorChatMessage,
                                                             OnCompletedChatMessage);
    }

    public async Task<CommandResult> ExecuteCommand(Command verse)
    {
      CommandResult result = null;
      Console.WriteLine(verse);
      try
      {
        if (verse.Verb.Token == "inventory" || verse.Verb.Token == "inv")
        {
          var inv = string.Join(", ", _agentState.State.Things.Select(t => t.Value.Name));
          result = CommandResult.SuccessfulResult($"inventory: {inv}");
        }
        else if (verse.Verb.Token == "put" || verse.Verb.Token == "drop")
        {
          if (verse.DirectObject == null)
            return CommandResult.ErrorResult($"Verb get requires arguments"); // NOTE: This should be handled in another place.

          GrainPointer pointer;
          if (!_agentState.State.Things.TryGetValue(verse.DirectObject.Token, out pointer))
          {
            return CommandResult.ErrorResult($"You do not have thing: {verse.DirectObject.Token}");
          }

          await GrainFactory.GetGrain<IRoomGrain>(_agentState.State.RoomId)
                            .Cast<IRoomAdministrationGrain>()
                            .AddObject(new ObjectPointer(pointer.Key, pointer.Name));
          _agentState.State.Things = _agentState.State.Things.Where(t => t.Value.Key != pointer.Key)
                                                             .ToDictionary(d => d.Value.Name, d => d.Value);
          await _agentState.WriteStateAsync();
          result = CommandResult.SuccessfulResult($"dropped {verse.DirectObject.Token}");
        }
        else
        {
          result = await GrainFactory.GetGrain<IRoomGrain>(_agentState.State.RoomId)
                                     .ExecuteCommand(new AgentPointer(_agentState.State.AgentPointer.Key,
                                                                      _agentState.State.AgentPointer.Name), verse);
          if (result.Success && result.Objects?.Count > 0)
          {
            foreach (var o in result.Objects)
            {
              _agentState.State.Things.Add(o.Name, o);
            }
            await _agentState.WriteStateAsync();
          }

          if (result.Success && result.NewRoom != null)
          {
            _agentState.State.RoomId = result.NewRoom.Key;
            await TransferRoom(_agentState.State.RoomId);
            await _agentState.WriteStateAsync();
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
      return result;
    }

    public Task<string> GetName()
    {
      return Task.FromResult(_agentState.State.AgentPointer.Name);
    }

    public Task<Guid> GetRoom()
    {
      return Task.FromResult(_agentState.State.RoomId);
    }
  }
}