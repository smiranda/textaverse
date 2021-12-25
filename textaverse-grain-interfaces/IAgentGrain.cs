﻿using System;
using Orleans;
using System.Threading.Tasks;
using Textaverse.Models;

namespace Textaverse.GrainInterfaces
{
  /// <summary>
  /// An agent is an entity which can execute actions in a room, change rooms,
  /// interact with objects or other agents. It can be player-controlled or not.
  /// </summary>
  public interface IAgentGrain : IGrainWithGuidKey
  {
    Task Configure(string name, IRoomGrain room);
    Task<string> GetName();
    Task<IRoomGrain> GetRoom();
    Task Execute(Verse verse);
  }
}