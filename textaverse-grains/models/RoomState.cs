using System;
using System.Collections.Generic;
using System.Linq;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  public class RoomState
  {
    public RoomState()
    {
    }

    public RoomState(string name, string description)
    {
      Name = name;
      Description = description;
      Things = new Dictionary<string, GrainPointer>();
      Passages = new Dictionary<string, PassagePointer>();
      Agents = new Dictionary<string, AgentInRoomState>();
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<string, GrainPointer> Things { get; set; }
    public Dictionary<string, PassagePointer> Passages { get; set; }
    public Dictionary<string, AgentInRoomState> Agents { get; set; }
  }
}