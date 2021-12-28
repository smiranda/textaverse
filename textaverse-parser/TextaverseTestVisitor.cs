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

      var indirectObjectToken = ctx.indirectobject() != null ? new IndirectObject(ctx.indirectobject()?.adjectivatedNoun()?.noun()?.WORD().ToString().ToLowerInvariant(),
                                                                                  null/*ctx.indirectobject()?.adjectivatedNoun()?.adjective()?.WORD().ToString()*/)
                                                             : null;

      var directObjects = ctx.@object() != null ? ctx.@object()?.adjectivatedNoun()?
                                                  .Select(n => new DirectObject(n.noun()?.WORD()?.ToString().ToLowerInvariant(),
                                                                                null/*n.adjective()?.WORD()?.ToString()*/ ))
                                                  .ToList()
                                                : null;

      var command = new Command(new Verb(verbToken),
                                adverb,
                                directObjects,
                                indirectObjectToken,
                                preposition,
                                quote);

      return new Verse(new List<Command>() { command });
    }
  }
}
