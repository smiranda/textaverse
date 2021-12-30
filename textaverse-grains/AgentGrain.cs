using System;
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
    private long _roomId;
    public Task Configure(string name, long roomId)
    {
      _agentPointer = new AgentPointer { Key = this.GetPrimaryKey(), Name = name };
      _roomId = roomId;
      return Task.CompletedTask;
    }

    public async Task<CommandResult> ExecuteCommand(Command verse)
    {
      Console.WriteLine("CMD AGNT");
      return await GrainFactory.GetGrain<IRoomGrain>(_roomId).ExecuteCommand(verse);
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