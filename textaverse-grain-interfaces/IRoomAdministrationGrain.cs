using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using Textaverse.Models;

namespace Textaverse.GrainInterfaces
{
  /// <summary>
  /// The universe is composed by a graph of nodes. Rooms are nodes.
  /// </summary>
  public interface IRoomAdministrationGrain : IGrainWithGuidKey
  {
    Task Configure(string name, string description);
    Task AddObject(ObjectPointer obj);
    Task AddPassage(PassagePointer passage);
  }
}
