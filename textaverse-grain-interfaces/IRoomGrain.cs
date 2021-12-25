using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using Textaverse.Models;

namespace Textaverse.GrainInterfaces
{
  /// <summary>
  /// The universe is composed by a graph of nodes. Rooms are nodes.
  /// </summary>
  public interface IRoomGrain : IGrainWithIntegerKey
  {
    Task<string> Description();
    Task<IEnumerable<AgentPointer>> ListAgents();
    Task<IEnumerable<ObjectPointer>> ListObjects();
    Task<IEnumerable<PassagePointer>> ListPassages();
    Task ExecuteAgentVerse(Verse verse, AgentPointer agentPointer);
    Task ExecuteRoomVerse(Verse verse);
    Task<IRoomGrain> ChangeRoom(PassagePointer passagePointer);
  }
}
