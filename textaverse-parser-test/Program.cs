namespace Textaverse.Parser
{
  using System;
  using System.IO;
  using Textaverse.Models;

  public class Program
  {
    static void Main(string[] args)
    {
      Verse verse = null;
      var parser = new VerseParser();
      if (args[0] == "--file")
      {
        // dotnet run --file test.vrs
        verse = parser.Parse(File.ReadAllText(args[1]));
      }
      else
      {
        // dotnet run 'attack monster with axe, then drink water from well; attack human, then shout ""Death for all humans !"", then drink water.'
        verse = parser.Parse(args[0]);
      }
      Console.WriteLine(verse);
    }
  }
}
