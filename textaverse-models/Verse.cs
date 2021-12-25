using System;
using System.Collections.Generic;

namespace Textaverse.Models
{
  /// <summary>
  /// A verse is a textaverse script, e.g., a sequence of commands.
  /// </summary>
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
  }
}
