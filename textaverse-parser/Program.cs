namespace Textaverse.Parser
{
  using Antlr4.Runtime;
  using Antlr4.Runtime.Misc;
  using Antlr4.Runtime.Tree;
  using Newtonsoft.Json.Linq;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
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
  public class Program
  {
    static void Main(string[] args)
    {
      if (args[0] == "--file")
      {
        // dotnet run --file test.vrs
        Try(File.ReadAllText(args[1]));
      }
      else
      {
        // dotnet run 'attack monster with axe, then drink water from well; attack human, then shout ""Death for all humans !"", then drink water.'
        Try(args[0]);
      }
    }

    static void Try(string input)
    {
      var str = new AntlrInputStream(input);
      var lexer = new TextaverseLexer(str);
      var tokens = new CommonTokenStream(lexer);
      var parser = new TextaverseParser(tokens);
      var listener_lexer = new ErrorListener<int>();
      var listener_parser = new ErrorListener<IToken>();
      lexer.AddErrorListener(listener_lexer);
      parser.AddErrorListener(listener_parser);

      TextaverseParser.FileContext ctx = parser.file();
      var visitor = new TextaverseTestVisitor();
      var r = visitor.Visit(ctx);
      if (listener_lexer.had_error || listener_parser.had_error)
        System.Console.WriteLine("error in parse.");
      else
      {
        System.Console.WriteLine("parse completed.");
        //Console.WriteLine(JObject.FromObject(r).ToString());
        Console.WriteLine(r.ToString());
      }
    }
  }
}
