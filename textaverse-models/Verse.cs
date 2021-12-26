using System;
using System.Collections.Generic;
using System.Linq;
using Orleans.Concurrency;

namespace Textaverse.Models
{
  /// <summary>
  /// A verse is a textaverse script, e.g., a sequence of commands.
  /// </summary>
  [Immutable] // Note: This avoids data copies during grain messaging (if possible)
  public class Verse
  {
    public Verse()
    {
      Commands = new List<Command>();
    }

    public Verse(List<Command> commands)
    {
      Commands = commands;
    }

    public List<Command> Commands { get; set; }

    public override string ToString()
    {
      return string.Join('\n', Commands.Select(c => c.ToString()));
    }
  }
}
