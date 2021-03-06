using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using Textaverse.Models;

namespace Textaverse.GrainInterfaces
{
  /// <summary>
  /// The universe is composed by a graph of nodes. Rooms are nodes.
  /// </summary>
  public interface IRoomGrain : IGrainWithStringKey
  {
    Task<string> Description();
    Task<string> GetName();
    Task<IEnumerable<AgentPointer>> ListAgents();
    Task<IEnumerable<ObjectPointer>> ListObjects();
    Task<IEnumerable<PassagePointer>> ListPassages();
    Task<CommandResult> ExecuteCommand(AgentPointer agent, Command verse);
    Task<IRoomGrain> ChangeRoom(PassagePointer passagePointer);
  }
}
