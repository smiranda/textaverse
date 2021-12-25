﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  /// <summary>
  /// A standard room.
  /// </summary>
  public class RoomGrain : Grain, IRoomGrain, IRoomConnectivityGrain, IRoomAdministrationGrain
  {
    private RoomState _roomState;

    public Task Configure(string name, string description)
    {
      _roomState = new RoomState(name, description);
      return Task.CompletedTask;
    }
    public Task<IRoomGrain> ChangeRoom(PassagePointer passagePointer)
    {
      throw new System.NotImplementedException();
    }

    public Task<string> Description()
    {
      return Task.FromResult(_roomState.Description);
    }

    public Task ExecuteAgentVerse(Verse verse, AgentPointer agentPointer)
    {
      throw new System.NotImplementedException();
    }

    public Task ExecuteRoomVerse(Verse verse)
    {
      throw new System.NotImplementedException();
    }

    public Task<IEnumerable<AgentPointer>> ListAgents()
    {
      throw new System.NotImplementedException();
    }

    public Task<IEnumerable<ObjectPointer>> ListObjects()
    {
      throw new System.NotImplementedException();
    }

    public Task<IEnumerable<PassagePointer>> ListPassages()
    {
      throw new System.NotImplementedException();
    }

    public Task TransferAgent(AgentPointer agentPointer)
    {
      throw new System.NotImplementedException();
    }
  }
}