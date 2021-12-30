using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  /// <summary>
  /// A standard object.
  /// </summary>
  public class ObjectGrain : Grain, IObjectGrain
  {
    private string _name;

    public Task Configure(string name)
    {
      _name = name;
      return Task.CompletedTask;
    }

    public Task<CommandResult> ExecuteCommand(Command verse)
    {
      throw new System.NotImplementedException();
    }

    public Task<GrainPointer> GetLocation()
    {
      throw new System.NotImplementedException();
    }

    public Task<string> GetName()
    {
      return Task.FromResult(_name);
    }
  }
}