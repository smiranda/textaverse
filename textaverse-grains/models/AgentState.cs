using System;
using System.Collections.Generic;
using Textaverse.Models;

namespace Textaverse.Grains
{
  public class AgentState
  {
    public AgentState()
    {
    }

    public AgentState(string roomId, AgentPointer agentPointer)
    {
      RoomId = roomId;
      AgentPointer = agentPointer;
      Things = new Dictionary<string, GrainPointer>();
    }

    public string RoomId { get; set; }
    public AgentPointer AgentPointer;
    public Dictionary<string, GrainPointer> Things { get; set; }
  }
}