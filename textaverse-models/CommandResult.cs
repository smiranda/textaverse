using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Textaverse.Models
{
  /// <summary>
  /// An Command Result is the result of a command.
  /// </summary>
  public class CommandResult
  {
    public CommandResult()
    {
    }

    public static CommandResult SuccessfulResult(string message)
    {
      return new CommandResult { Success = true, Message = message };
    }
    public static CommandResult SuccessfulResult(string message, List<GrainPointer> objects)
    {
      return new CommandResult { Success = true, Message = message, Objects = objects };
    }
    public static CommandResult ErrorResult(string message)
    {
      return new CommandResult { Success = false, Message = message };
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Message { get; set; }
    public bool Success { get; set; }

    // TODO: Here we need to add everything that could result from a command. How to structure ?
    // Possible results
    // 1. An object transferred to command callee 
    // 2. A change to a property of the callee
    //   2.1. Health
    //   2.2. Money
    //   2.3. Skills ?
    // 3. A more general Command ? Maybe just return another command ! And execute this command on the agent.
    //
    // Basic version - optional objects + message
    public List<GrainPointer> Objects;
  }
}
