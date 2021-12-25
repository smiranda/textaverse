using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  /// <summary>
  /// A standard grain.
  /// </summary>
  public class AgentGrain : Grain, IAgentGrain
  {
    private AgentPointer _agentPointer;
    public Task Configure(string name, IRoomGrain room)
    {
      _agentPointer = new AgentPointer { Key = this.GetPrimaryKey(), Name = name };
      return Task.CompletedTask;
    }

    public Task Execute(Verse verse)
    {
      throw new System.NotImplementedException();
    }

    public Task<string> GetName()
    {
      return Task.FromResult(_agentPointer.Name);
    }

    public Task<IRoomGrain> GetRoom()
    {
      throw new System.NotImplementedException();
    }
  }
}