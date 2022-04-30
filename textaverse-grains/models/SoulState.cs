using System;
using System.Collections.Generic;
using Textaverse.Models;

namespace Textaverse.Grains
{
  public class SoulState
  {
    public SoulState()
    {
    }
    public SoulState(AgentPointer agentPointer)
    {
      AgentPointer = agentPointer;
    }
    public AgentPointer AgentPointer;
  }
}