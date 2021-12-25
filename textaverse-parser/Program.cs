// Template generated code from Antlr4BuildTasks.Template v 8.17
namespace textaverse_parser
{
  using Antlr4.Runtime;
  using System.Text;

  public class Program
  {
    static void Main(string[] args)
    {
      // dotnet run 'attack monster with axe, then drink water from well; attack human, then shout ""Death for all humans !"", then drink water.'
      Try(args[0]);
    }

    static void Try(string input)
    {
      var str = new AntlrInputStream(input);
      System.Console.WriteLine(input);
      var lexer = new TextaverseLexer(str);
      var tokens = new CommonTokenStream(lexer);
      var parser = new TextaverseParser(tokens);
      var listener_lexer = new ErrorListener<int>();
      var listener_parser = new ErrorListener<IToken>();
      lexer.AddErrorListener(listener_lexer);
      parser.AddErrorListener(listener_parser);
      var tree = parser.file();
      if (listener_lexer.had_error || listener_parser.had_error)
        System.Console.WriteLine("error in parse.");
      else
        System.Console.WriteLine("parse completed.");
    }

    static string ReadAllInput(string fn)
    {
      var input = System.IO.File.ReadAllText(fn);
      return input;
    }
  }
}
