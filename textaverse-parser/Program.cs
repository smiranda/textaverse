// Template generated code from Antlr4BuildTasks.Template v 8.17
namespace textaverse_parser
{
  using Antlr4.Runtime;
  using Antlr4.Runtime.Misc;
  using Antlr4.Runtime.Tree;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;

  public class VerseFile
  {
    public VerseFile()
    {
      Verbs = new List<string>();
    }

    public List<string> Verbs { get; set; }
  }

  public class TextaverseTestVisitor : TextaverseBaseVisitor<VerseFile>
  {
    private VerseFile _verseFile = new VerseFile();
    public override VerseFile VisitCommand([NotNull] TextaverseParser.CommandContext ctx)
    {
      Console.WriteLine($"{ctx.predicate()?.verb()?.WORD()}" +
                        $"({string.Join('+', ctx.indirectobject()?.@object()?.noun()?.Select(n => n.WORD().ToString()) ?? new string[] { })}, " +
                        $"{ctx.PREPOSITION()}, {string.Join('+', ctx.@object()?.noun()?.Select(n => n.WORD().ToString()) ?? new string[] { })}, " +
                        $"{ctx.quotedarg()?.ANYWORDQUOTED()}) ");
      return _verseFile;

    }
  }
  public class Program
  {
    static void Main(string[] args)
    {
      if (args[0] == "--file" || args[0] == "-f")
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
        System.Console.WriteLine("parse completed.");
    }
  }
}
