namespace Textaverse.Models
{
  public class ChatMessage
  {
    public ChatMessage()
    {
    }

    public ChatMessage(string text)
    {
      Text = text;
    }

    public string Text { get; set; }
  }
}