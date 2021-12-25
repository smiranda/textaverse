using System;
using System.Collections.Generic;

namespace Textaverse.Models
{
  /// <summary>
  /// An verse is a structured request to a room, agent or object to behave in some way.
  /// </summary>
  public class Verse
  {
    public Verb Verb;
    public Dictionary<string, string> Arguments; // TODO: Maybe object ? To cast later when Verb is known.
  }
}
