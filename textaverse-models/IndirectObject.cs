using Newtonsoft.Json;

namespace Textaverse.Models
{
  public class IndirectObject
  {
    public IndirectObject(string token, string adjective = null)
    {
      Token = token;
      Adjective = adjective;
    }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Adjective { get; set; }
    public string Token { get; set; }
  }
}