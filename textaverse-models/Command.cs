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
                   List<DirectObject> directObjects = null,
                   IndirectObject indirectObject = null,
                   Preposition preposition = null,
                   string quote = null)
    {
      Verb = verb;
      Adverb = adverb;
      DirectObjects = directObjects;
      IndirectObject = indirectObject;
      Preposition = preposition;
      Quote = quote;
    }

    public override string ToString()
    {
      var v = Verb?.Token;
      var a = Adverb?.Token;
      var dos = DirectObjects != null ? string.Join("+", DirectObjects?.Select(o => o.Token)) : null;
      var io = IndirectObject?.Token;
      var p = Preposition?.Token;
      var q = Quote;
      var st = v;
      if (a != null)
        st += $" [{a}]";

      st += " (";
      if (io != null)
        st += st.Last() != '(' ? ", " + io : io;
      if (p != null)
        st += st.Last() != '(' ? ", " + p : p;
      if (dos?.Length > 0)
        st += st.Last() != '(' ? ", " + dos : dos;
      if (q != null)
        st += st.Last() != '(' ? ", " + q : q;

      st += ')';
      return st;
    }

    public Verb Verb { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Adverb Adverb { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<DirectObject> DirectObjects { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IndirectObject IndirectObject { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Preposition Preposition { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Quote { get; set; }
  }
}