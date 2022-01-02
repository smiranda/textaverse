namespace Textaverse.Models
{
  public class PassagePointer
  {
    public PassagePointer()
    {
    }

    public PassagePointer(string name, GrainPointer source, GrainPointer target)
    {
      Name = name;
      Source = source;
      Target = target;
    }

    public string Name { get; set; }
    public GrainPointer Source { get; set; }
    public GrainPointer Target { get; set; }
  }
}