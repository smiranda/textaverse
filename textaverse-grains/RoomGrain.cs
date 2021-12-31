using System;
using System.Collections.Generic;
using System.Linq;
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

    public RoomGrain([PersistentState("roomState", "roomStateStore")] IPersistentState<RoomState> roomState)
    {
      _roomState = roomState;
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

    public async Task<CommandResult> ExecuteCommand(Command verse)
    {
      try
      {
        if (verse.Verb.Token == "list" || verse.Verb.Token == "ls")
        {
          var ls = string.Join(", ", _roomState.State.Things.Select(t => t.Value.Name));
          return CommandResult.SuccessfulResult($"things: {ls}");
        }
        else if (verse.Verb.Token == "shout")
        { // NOTE: bad ideia to do it like this - but it's just a draft to start exploring
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

    public Task TransferAgent(AgentPointer agentPointer)
    {
      throw new System.NotImplementedException();
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
  }
}