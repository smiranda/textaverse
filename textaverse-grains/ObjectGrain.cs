using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  /// <summary>
  /// A standard object.
  /// </summary>
  public class ObjectGrain : Grain, IObjectGrain
  {
    protected readonly IPersistentState<ObjectState> _objectState;

    public ObjectGrain([PersistentState("objectState", "objectStateStore")] IPersistentState<ObjectState> objectState)
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
      throw new System.NotImplementedException();
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