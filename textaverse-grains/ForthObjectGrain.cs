using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DotForth;
using Orleans;
using Orleans.Runtime;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  /// <summary>
  /// A standard object.
  /// </summary>
  public class ForthObjectGrain : Grain, IForthObjectGrain
  {
    private Forth _forth;
    protected readonly IPersistentState<ObjectState> _objectState;
    public ForthObjectGrain([PersistentState("objectState", "objectStateStore")] IPersistentState<ObjectState> objectState)
    {
      _objectState = objectState;
      _forth = new Forth();
    }
    public Task Configure(string name)
    {
      _objectState.State.Name = name;
      return Task.CompletedTask;
    }
    public async Task<CommandResult> ExecuteCommand(Command verse)
    {
      try
      {
        if (verse.Verb.Token == "type")
        {
          if (verse.Quote == null)
          {
            return CommandResult.ErrorResult($"Verb 'type' requires a quote argument");
          }
          var twr = new StringWriter();
          await _forth.Run(verse.Quote.Trim('"'), twr);
          var result = twr.ToString();
          return CommandResult.SuccessfulResult(result);
        }
        else
        {
          return CommandResult.ErrorResult("Command not recognized");
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }
    public Task<GrainPointer> GetLocation()
    {
      throw new System.NotImplementedException();
    }

    public Task<string> GetName()
    {
      return Task.FromResult(_objectState.State.Name);
    }
  }
}