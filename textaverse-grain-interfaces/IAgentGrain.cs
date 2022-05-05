using System;
using Orleans;
using System.Threading.Tasks;
using Textaverse.Models;

namespace Textaverse.GrainInterfaces
{
  /// <summary>
  /// An agent is an entity which can execute actions in a room, change rooms,
  /// interact with objects or other agents. It can be player-controlled or not.
  /// </summary>
  public interface IAgentGrain : IGrainWithStringKey
  {
    Task Configure(string name, string roomId);
    Task<string> GetName();
    Task<string> GetRoom();
    Task<CommandResult> ExecuteCommand(Command verse);
    Task TransferRoom(string roomId);
  }
}
