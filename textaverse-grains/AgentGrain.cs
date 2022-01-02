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
  /// A standard grain.
  /// </summary>
  public class AgentGrain : Grain, IAgentGrain
  {
    private readonly IPersistentState<AgentState> _agentState;

    public AgentGrain([PersistentState("agentState", "agentStateStore")] IPersistentState<AgentState> agentState)
    {
      _agentState = agentState;
    }

    public Task Configure(string name, Guid roomId)
    {
      _agentState.State = new AgentState(roomId, new AgentPointer { Key = this.GetPrimaryKey(), Name = name });
      return Task.CompletedTask;
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