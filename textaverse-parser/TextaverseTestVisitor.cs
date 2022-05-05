namespace Textaverse.Parser
{
  using Antlr4.Runtime.Misc;
  using System.Collections.Generic;
  using System.Linq;
  using Textaverse.Models;
  public class TextaverseTestVisitor : TextaverseBaseVisitor<Verse>
  {
    public override Verse VisitFile([NotNull] TextaverseParser.FileContext ctx)
    {
      var fileVerse = new Verse();
      foreach (var s in ctx.sentence())
      {
        fileVerse.Commands.AddRange(Visit(s).Commands);
      }
      return fileVerse;
    }
    public override Verse VisitSentence([NotNull] TextaverseParser.SentenceContext ctx)
    {
      var sentenceVerse = new Verse();
      foreach (var c in ctx.command())
      {
        sentenceVerse.Commands.AddRange(Visit(c).Commands);
      }
      return sentenceVerse;
    }
    public override Verse VisitCommand([NotNull] TextaverseParser.CommandContext ctx)
    {
      var verbToken = ctx.predicate()?.verb()?.WORD()?.ToString().ToLowerInvariant();
      var adverb = ctx.adverb() != null ? new Adverb(ctx.adverb()?.WORD()?.ToString().ToLowerInvariant()) : null;
      var preposition = ctx.PREPOSITION() != null ? new Preposition(ctx.PREPOSITION()?.ToString().ToLowerInvariant()) : null;
      string quote = ctx.quotedarg()?.ANYWORDQUOTED()?.ToString();

      Dictionary<string, string> properties = null;
      var pnames = ctx.propname()?.Select(pn => pn.WORD()?.ToString()).ToArray();
      var pvalues = ctx.quotedvalue()?.Select(pn => 
        pn.ANYWORDQUOTED() != null ?
          pn.ANYWORDQUOTED()?.ToString() : pn.WORD()?.ToString()
          ).ToArray();
      
      if(pnames.Count() > 0) {
        properties = new Dictionary<string,string>();
        for(int i = 0; i < pnames.Count(); i+=1){
          properties[pnames[i]] = pvalues[i];
        }
      }

      var directObject = ctx.@object() != null ? new DirectObject(ctx.@object()?.adjectivatedNoun()?.noun()?.WORD().ToString().ToLowerInvariant(),
                                                                                  null/*ctx.@object()?.adjectivatedNoun()?.adjective()?.WORD().ToString()*/)
                                                             : null;

      var indirectObjects = ctx.indirectobject() != null ? ctx.indirectobject()?.adjectivatedNoun()?
                                                  .Select(n => new IndirectObject(n.noun()?.WORD()?.ToString().ToLowerInvariant(),
                                                                                  null/*n.adjective()?.WORD()?.ToString()*/ ))
                                                  .ToList()
                                                : null;

      var command = new Command(new Verb(verbToken),
                                adverb,
                                directObject,
                                indirectObjects,
                                preposition,
                                quote,
                                properties);

      return new Verse(new List<Command>() { command });
    }
  }
}
