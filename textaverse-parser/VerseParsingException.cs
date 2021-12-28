using System;
namespace Textaverse.Parser
{
  public class VerseParsingException : Exception
  {
    public VerseParsingException()
    {
    }
    public VerseParsingException(string message)
        : base(message)
    {
    }
    public VerseParsingException(string message, Exception inner)
        : base(message, inner)
    {
    }
  }
}