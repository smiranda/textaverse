using System;
using Orleans.Concurrency;

namespace Textaverse.Models
{
  [Immutable] // Note: This avoids data copies during grain messaging (if possible)
  public class ObjectPointer
  {
    public ObjectPointer()
    {
    }

    public ObjectPointer(Guid key, string name)
    {
      Key = key;
      Name = name;
    }

    public Guid Key { get; set; }
    public string Name { get; set; }
  }
}