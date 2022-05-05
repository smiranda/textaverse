using System;
using Orleans;
using System.Threading.Tasks;
using Textaverse.Models;

namespace Textaverse.GrainInterfaces
{
  /// <summary>
  /// An object is a passive entity onto which actions can be executed.
  /// They can be present inside Rooms or in Agent's posession.
  /// </summary>
  public interface IObjectGrain : IGrainWithStringKey
  {
    Task Configure(string name);
    Task<string> GetName();
    Task<GrainPointer> GetLocation();
    Task<CommandResult> ExecuteCommand(Command verse);
  }
}
