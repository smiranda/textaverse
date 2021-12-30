using System;
using Orleans.Concurrency;

namespace Textaverse.Models
{
  [Immutable] // Note: This avoids data copies during grain messaging (if possible)
  public class GrainPointer
  {
    public GrainPointer()
    {
    }

    public GrainPointer(Guid key, string name, GrainType type)
    {
      Key = key;
      Name = name;
      Type = type;
    }

    public Guid Key { get; set; }
    public string Name { get; set; }
    public GrainType Type { get; set; }
  }
}