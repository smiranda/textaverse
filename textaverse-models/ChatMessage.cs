namespace Textaverse.Models
{
  public class ChatMessage
  {
    public ChatMessage()
    {
    }

    public ChatMessage(string text, AgentPointer speaker)
    {
      Text = text;
      Speaker = speaker;
    }

    public string Text { get; set; }
    public AgentPointer Speaker { get; }
  }
}