using System;
using System.Collections.Generic;
using System.Linq;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  public class AgentInRoomState
  {
    public AgentInRoomState()
    {
    }

    public AgentInRoomState(AgentPointer agentPointer)
    {
      AgentPointer = agentPointer;
      Status = AgentInRoomStatus.Active;
    }

    public void BeginAgentTransaction(string sourceRoom, string targetRoom)
    {
      Status = AgentInRoomStatus.Transient;
      TransientSourceRoom = sourceRoom;
      TransientTargetRoom = targetRoom;
    }
    public void CancelAgentTransaction()
    {
      Status = AgentInRoomStatus.Active;
      TransientSourceRoom = null;
      TransientTargetRoom = null;
    }
    public AgentPointer AgentPointer { get; set; }
    public AgentInRoomStatus Status { get; set; }
    public string TransientSourceRoom { get; set; } // valid iff Status = Transient
    public string TransientTargetRoom { get; set; } // valid iff Status = Transient
  }
}