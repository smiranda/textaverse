namespace Textaverse.Grains
{
  public class RoomState
  {
    public RoomState(string name, string description)
    {
      Name = name;
      Description = description;
    }

    public string Name { get; set; }
    public string Description { get; set; }
  }
}