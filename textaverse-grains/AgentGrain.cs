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
    private IRoomGrain _roomGrain;
    public Task Configure(string name, IRoomGrain room)
    {
      _agentPointer = new AgentPointer { Key = this.GetPrimaryKey(), Name = name };
      _roomGrain = room;
      return Task.CompletedTask;
    }

    public async Task<CommandResult> ExecuteCommand(Command verse)
    {
      return await _roomGrain.ExecuteCommand(verse);
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