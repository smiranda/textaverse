using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
      if (!string.IsNullOrEmpty(_agentState.State?.RoomId))
      {
        var stream = streamProvider.GetStream<ChatMessage>(new Guid(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(_agentState.State.RoomId)).Take(16).ToArray()), "RoomChat.Out");
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
      _agentChatOutStream = streamProvider.GetStream<ChatMessage>(new Guid(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(this.GetPrimaryKeyString())).Take(16).ToArray()), "AgentChat.Out");

      await base.OnActivateAsync();
    }

    private async Task OnNextChatMessage(ChatMessage message, StreamSequenceToken token)
    {
      // Forward room chat into agent chat
      Console.WriteLine("(msg) " + _agentState.State.AgentPointer.Name);

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
    public async Task Configure(string name, string roomId)
    {
      _agentState.State = new AgentState(roomId, new AgentPointer { Key = this.GetPrimaryKeyString(), Name = name });
      await _agentState.WriteStateAsync();
    }

    public async Task TransferRoom(string roomId)
    {
      if (_roomChatOutSubscription != null)
      {
        await _roomChatOutSubscription.UnsubscribeAsync();
      }
      var streamProvider = GetStreamProvider("SMSProvider");
      var stream = streamProvider.GetStream<ChatMessage>(new Guid(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(roomId)).Take(16).ToArray()), "RoomChat.Out");
      Console.WriteLine("(subs) " + _agentState.State.AgentPointer.Name);

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
        if (verse.Verb.Token == "create" ) {
          if(verse.DirectObject.Token == "room") {
            string idValue, nameValue, descriptionValue;
            if(!verse.Properties.TryGetValue("id", out idValue))
            {
              return CommandResult.ErrorResult($"'{verse.DirectObject.Token}' missing id property"); // NOTE: This should be handled in another place.
            }
            if (!verse.Properties.TryGetValue("name", out nameValue))
            {
              return CommandResult.ErrorResult($"'{verse.DirectObject.Token}' missing name property"); // NOTE: This should be handled in another place.
            }
            if (!verse.Properties.TryGetValue("description", out descriptionValue))
            {
              return CommandResult.ErrorResult($"'{verse.DirectObject.Token}' missing description property"); // NOTE: This should be handled in another place.
            }

            var room = GrainFactory.GetGrain<IRoomGrain>(idValue);
            await room.Cast<IRoomAdministrationGrain>()
                      .Configure(nameValue, descriptionValue);
            result = CommandResult.SuccessfulResult($"created room {idValue}");
          } else if (verse.DirectObject.Token == "passage") {
            string sourceValue, targetValue, nameValue;
            if (!verse.Properties.TryGetValue("name", out nameValue))
            {
              return CommandResult.ErrorResult($"'{verse.DirectObject.Token}' missing name property"); // NOTE: This should be handled in another place.
            }
            if (!verse.Properties.TryGetValue("source", out sourceValue))
            {
              return CommandResult.ErrorResult($"'{verse.DirectObject.Token}' missing source property"); // NOTE: This should be handled in another place.
            }
            if (!verse.Properties.TryGetValue("target", out targetValue))
            {
              return CommandResult.ErrorResult($"'{verse.DirectObject.Token}' missing target property"); // NOTE: This should be handled in another place.
            }

            var room1 = GrainFactory.GetGrain<IRoomGrain>(sourceValue);
            var room2 = GrainFactory.GetGrain<IRoomGrain>(targetValue);

            await room1.Cast<IRoomAdministrationGrain>().AddPassage(
              new PassagePointer(nameValue,
                    new GrainPointer(room1.GetPrimaryKeyString(),
                                      await room1.GetName(),
                                      GrainType.Room),
                    new GrainPointer(room2.GetPrimaryKeyString(),
                                      await room2.GetName(),
                                      GrainType.Room)));

            await room2.Cast<IRoomAdministrationGrain>().AddPassage(
              new PassagePointer(nameValue,
                    new GrainPointer(room2.GetPrimaryKeyString(),
                                      await room2.GetName(),
                                      GrainType.Room),
                    new GrainPointer(room1.GetPrimaryKeyString(),
                                      await room1.GetName(),
                                      GrainType.Room)));
            result = CommandResult.SuccessfulResult($"created passage");
          } else {
            return CommandResult.ErrorResult($"Cannot create '{verse.DirectObject.Token}'");
          }
        }
        else  if (verse.Verb.Token == "inventory" || verse.Verb.Token == "inv")
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

    public Task<string> GetRoom()
    {
      return Task.FromResult(_agentState.State.RoomId);
    }
  }
}