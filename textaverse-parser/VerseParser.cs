namespace Textaverse.Parser
{
  using Antlr4.Runtime;
  using Textaverse.Models;
  public class VerseParser
  {
    public Verse Parse(string input)
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
      {
        throw new VerseParsingException();
      }

      return r;
    }
  }
}
