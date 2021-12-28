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

    public CommandResult(string message)
    {
      Message = message;
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Message { get; set; }
  }
}
