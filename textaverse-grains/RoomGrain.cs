using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  /// <summary>
  /// A standard room.
  /// </summary>
  public class RoomGrain : Grain, IRoomGrain, IRoomConnectivityGrain, IRoomAdministrationGrain
  {
    private readonly IPersistentState<RoomState> _roomState;
    private Orleans.Streams.IAsyncStream<ChatMessage> _roomChatOutStream;

    public RoomGrain([PersistentState("roomState", "roomStateStore")] IPersistentState<RoomState> roomState)
    {
      _roomState = roomState;
    }

    public override async Task OnActivateAsync()
    {
      var streamProvider = GetStreamProvider("SMSProvider");
      _roomChatOutStream = streamProvider.GetStream<ChatMessage>(this.GetPrimaryKey(), "RoomChat.Out");
      await base.OnActivateAsync();
    }

    public async Task Configure(string name, string description)
    {
      _roomState.State = new RoomState(name, description);
      await _roomState.WriteStateAsync();
    }
    public Task<IRoomGrain> ChangeRoom(PassagePointer passagePointer)
    {
      throw new System.NotImplementedException();
    }

    public Task<string> Description()
    {
      return Task.FromResult(_roomState.State.Description);
    }

    public async Task<CommandResult> ExecuteCommand(AgentPointer agent, Command verse)
    {
      try
      {
        if (verse.Verb.Token == "list" || verse.Verb.Token == "ls")
        {
          var things = string.Join("\n", _roomState.State.Things.Select(t => " - " + t.Value.Name));
          var passages = string.Join("\n", _roomState.State.Passages.Select(t => " - " + t.Value.Name));
          var outstr = new StringBuilder();
          outstr.Append($"things: \n{things}\n");
          outstr.Append($"passages: \n{passages}\n");
          var output = outstr.ToString();
          return CommandResult.SuccessfulResult(output);
        }
        else if (verse.Verb.Token == "shout")
        { // NOTE: bad ideia to do it like this - but it's just a draft to start exploring
          await _roomChatOutStream.OnNextAsync(new ChatMessage(verse.Quote));
          return CommandResult.SuccessfulResult($"You shout: {verse.Quote}");
        }
        else if (verse.Verb.Token == "type")
        {
          if (verse.DirectObject == null)
            return CommandResult.ErrorResult($"Verb type requires arguments"); // NOTE: This should be handled in another place.

          var pointers = new List<GrainPointer>();
          GrainPointer pointer;
          if (!_roomState.State.Things.TryGetValue(verse.DirectObject.Token, out pointer))
          {
            return CommandResult.ErrorResult($"Thing not here: {verse.DirectObject.Token}");
          }
          if (pointer.Type != GrainType.Object)
            return CommandResult.ErrorResult($"Verb type requires an Object argument"); // NOTE: This should be handled in another place.

          // NOTE: CANNOT CHOOSE TYPE HARDCODED HERE THIS IS WRONG
          var obj = GrainFactory.GetGrain<IForthObjectGrain>(pointer.Key);
          var result = await obj.ExecuteCommand(verse);
          return result;
        }
        else if (verse.Verb.Token == "get")
        {
          if (verse.DirectObject == null)
            return CommandResult.ErrorResult($"Verb get requires arguments"); // NOTE: This should be handled in another place.

          var pointers = new List<GrainPointer>();
          GrainPointer pointer;
          if (!_roomState.State.Things.TryGetValue(verse.DirectObject.Token, out pointer))
          {
            return CommandResult.ErrorResult($"Thing not here: {verse.DirectObject.Token}");
          }
          _roomState.State.Things = _roomState.State.Things.Where(t => t.Value.Key != pointer.Key)
                                                           .ToDictionary(d => d.Value.Name, d => d.Value);
          pointers.Add(pointer);
          await _roomState.WriteStateAsync();
          return CommandResult.SuccessfulResult($"You get {string.Join(", ", pointers.Select(p => p.Name))}",
                                                pointers);
        }
        else if (verse.Verb.Token == "read")
        {
          if (verse.DirectObject == null)
            return CommandResult.ErrorResult($"Verb read requires arguments"); // NOTE: This should be handled in another place.

          var pointers = new List<GrainPointer>();
          GrainPointer pointer;
          if (!_roomState.State.Things.TryGetValue(verse.DirectObject.Token, out pointer))
          {
            return CommandResult.ErrorResult($"Thing not here: {verse.DirectObject.Token}");
          }
          if (pointer.Type != GrainType.Object)
            return CommandResult.ErrorResult($"Verb read requires an Object argument"); // NOTE: This should be handled in another place.

          // NOTE: CANNOT CHOOSE TYPE HARDCODED HERE THIS IS WRONG
          var obj = GrainFactory.GetGrain<IClockObjectGrain>(pointer.Key);
          var result = await obj.ExecuteCommand(verse);
          return result;
        }
        else if (verse.Verb.Token == "move" || verse.Verb.Token == "go")
        {
          if (verse.DirectObject == null)
            return CommandResult.ErrorResult($"Verb move/go requires arguments"); // NOTE: This should be handled in another place.

          PassagePointer pointer;
          if (!_roomState.State.Passages.TryGetValue(verse.DirectObject.Token, out pointer))
          {
            return CommandResult.ErrorResult($"Passage not here: {verse.DirectObject.Token}");
          }
          var passageTarget = GrainFactory.GetGrain<IRoomGrain>(pointer.Target.Key);

          var roomAgentl =
              _roomState.State.Agents.Where(a => a.Value.AgentPointer.Key == agent.Key);

          if (roomAgentl.Count() == 0)
          {
            // Cannot happen. How to react ?
            return CommandResult.ErrorResult($"Agent not here");
          }
          var roomAgente = roomAgentl.First();
          var roomAgent = roomAgente.Value;
          var roomAgentk = roomAgente.Key;

          // Begin agent transaction (marks as transient to recover from failures)
          try
          {
            roomAgent.BeginAgentTransaction(this.GetPrimaryKey(),
                                            pointer.Target.Key);
            await _roomState.WriteStateAsync();
            // Transfer to the other room
            await passageTarget.Cast<IRoomConnectivityGrain>().TransferAgent(agent);
            // Remove from this room
            _roomState.State.Agents.Remove(roomAgentk);
            await _roomState.WriteStateAsync();

            // Success
            return CommandResult.SuccessfulResult($"You entered: {pointer.Target.Name}",
                                                  pointer.Target);
          }
          catch
          {
            roomAgent.CancelAgentTransaction();
            throw;
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
      throw new System.NotImplementedException();
    }

    public Task<IEnumerable<AgentPointer>> ListAgents()
    {
      throw new System.NotImplementedException();
    }

    public Task<IEnumerable<ObjectPointer>> ListObjects()
    {
      throw new System.NotImplementedException();
    }

    public Task<IEnumerable<PassagePointer>> ListPassages()
    {
      throw new System.NotImplementedException();
    }

    public async Task TransferAgent(AgentPointer agentPointer)
    {
      _roomState.State.Agents.Add(agentPointer.Name,
                                  new AgentInRoomState(agentPointer));
      await _roomState.WriteStateAsync();
    }

    public Task<string> GetName()
    {
      return Task.FromResult(_roomState.State.Name);
    }

    public async Task AddObject(ObjectPointer obj)
    {
      _roomState.State.Things.Add(obj.Name, new GrainPointer(obj.Key, obj.Name, GrainType.Object));
      await _roomState.WriteStateAsync();
    }

    public async Task AddPassage(PassagePointer passage)
    {
      _roomState.State.Passages.Add(passage.Name, passage);
      await _roomState.WriteStateAsync();
    }
  }
}