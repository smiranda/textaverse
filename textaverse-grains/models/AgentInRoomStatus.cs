namespace Textaverse.Grains
{
  public enum AgentInRoomStatus
  {
    Active = 0, // Standard active agent in a room.
    Sleeping = 1, // Sleeping agent : Cannot be the target of actions
    Transient = 2, // Transient agent : Trying to change rooms. This status helps recover from system failures in case an agent was changing room while the server fails.
  }
}