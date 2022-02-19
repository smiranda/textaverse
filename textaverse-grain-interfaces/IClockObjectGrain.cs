using System;
using Orleans;
using System.Threading.Tasks;
using Textaverse.Models;

namespace Textaverse.GrainInterfaces
{
  /// <summary>
  /// A clock object.
  /// </summary>
  public interface IClockObjectGrain : IObjectGrain, IGrainWithGuidKey
  {
  }
}
