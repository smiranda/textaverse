using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  public class ClockObjectGrain : Grain, IClockObjectGrain
  {
    protected readonly IPersistentState<ObjectState> _objectState;

    public ClockObjectGrain([PersistentState("objectState", "objectStateStore")] IPersistentState<ObjectState> objectState)
    {
      _objectState = objectState;
    }
    public Task Configure(string name)
    {
      _objectState.State.Name = name;
      return Task.CompletedTask;
    }

    public virtual Task<CommandResult> ExecuteCommand(Command verse)
    {
      if (verse.Verb.Token == "read")
      {
        var result = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " ODT";
        return Task.FromResult(CommandResult.SuccessfulResult(result));
      }
      else
      {
        return Task.FromResult(CommandResult.ErrorResult("Command not recognized"));
      }
    }

    public Task<GrainPointer> GetLocation()
    {
      throw new System.NotImplementedException();
    }
    public Task<string> GetName()
    {
      return Task.FromResult(_objectState.State.Name);
    }
  }
}