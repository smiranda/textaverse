using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using Textaverse.Models;

namespace Textaverse.GrainInterfaces
{
  /// <summary>
  /// The universe is composed by a graph of nodes. Rooms are nodes.
  /// </summary>
  public interface IRoomAdministrationGrain : IGrainWithIntegerKey
  {
    Task Configure(string name, string description);
  }
}
