using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Textaverse.Models
{
  /// <summary>
  /// An Command is a structured request to a room, agent or object to behave in some way.
  /// </summary>
  public class Command
  {
    public Command(Verb verb,
                   Adverb adverb = null,
                   DirectObject directObject = null,
                   List<IndirectObject> indirectObjects = null,
                   Preposition preposition = null,
                   string quote = null)
    {
      Verb = verb;
      Adverb = adverb;
      DirectObject = directObject;
      IndirectObjects = indirectObjects;
      Preposition = preposition;
      Quote = quote;
    }

    public override string ToString()
    {
      var v = Verb?.Token;
      var a = Adverb?.Token;
      var ios = IndirectObjects != null ? string.Join("+", IndirectObjects?.Select(o => o.Token)) : null;
      var dob = DirectObject?.Token;
      var p = Preposition?.Token;
      var q = Quote;
      var st = v;
      if (a != null)
        st += $" [{a}]";

      st += " (";
      if (dob != null)
        st += st.Last() != '(' ? ", " + dob : dob;
      if (p != null)
        st += st.Last() != '(' ? ", " + p : p;
      if (ios?.Length > 0)
        st += st.Last() != '(' ? ", " + ios : ios;
      if (q != null)
        st += st.Last() != '(' ? ", " + q : q;

      st += ')';
      return st;
    }

    public Verb Verb { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Adverb Adverb { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DirectObject DirectObject { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<IndirectObject> IndirectObjects { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Preposition Preposition { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Quote { get; set; }
  }
}
