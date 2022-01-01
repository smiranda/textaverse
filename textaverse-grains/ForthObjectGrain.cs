using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotForth;
using Ketchup.Pizza.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Runtime;
using Telegraph.Net;
using Telegraph.Net.Models;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  /// <summary>
  /// A standard object.
  /// </summary>

  public class ForthObjectGrain : Grain, IForthObjectGrain
  {
    private static HttpClient _client = new HttpClient();
    protected readonly IPersistentState<ObjectState> _objectState;
    public ForthObjectGrain([PersistentState("objectState", "objectStateStore")] IPersistentState<ObjectState> objectState)
    {
      _objectState = objectState;
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

          var forth = new Forth();
          forth.LoadWord("key", new CompiledWord(async (Forth forth, TextWriter output) =>
          {
            await Task.Run(() =>
            {
              var keyLength = 4096;
              RSA rsa = RSA.Create(keyLength);
              string privateKeyData = "";
              string publicKeyData = "";
              privateKeyData = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
              publicKeyData = Convert.ToBase64String(rsa.ExportRSAPublicKey());
              forth.Stack.Push(new StackEntry(privateKeyData));
              forth.Stack.Push(new StackEntry(publicKeyData));
            });
          }));

          forth.LoadWord("get", new CompiledWord(async (Forth forth, TextWriter output) =>
          {
            var publicKeyData = forth.Stack.Pop().Token;
            var privateKeyData = forth.Stack.Pop().Token;
            var userId = forth.Stack.Pop().Token;
            var coalite = await GetCoalite(userId, publicKeyData, privateKeyData);
            forth.Stack.Push(new StackEntry(coalite));
            // return also the private and public keys for user examination
            forth.Stack.Push(new StackEntry(privateKeyData));
            forth.Stack.Push(new StackEntry(publicKeyData));
          }));

          forth.LoadWord("pop", new CompiledWord(async (Forth forth, TextWriter output) =>
          {
            await Task.Run(() =>
            {
              forth.Stack.Pop();
            });
          }));

          forth.LoadWord("telacc", new CompiledWord(async (Forth forth, TextWriter output) =>
          {
            var telclient = new TelegraphClient();
            var accountName = forth.Stack.Pop().Token;
            var acc = await telclient.CreateAccountAsync(accountName);
            forth.Stack.Push(new StackEntry(acc.AccessToken));
            forth.Stack.Push(new StackEntry(JObject.FromObject(acc).ToString(Formatting.None)));
          }));

          // coalite accname key get pop pop accname telacc pop .

          forth.LoadWord("telpost", new CompiledWord(async (Forth forth, TextWriter output) =>
           {
             var telclient = new TelegraphClient();
             var accessToken = forth.Stack.Pop().Token;
             var body = forth.Stack.Pop().Token;
             var title = forth.Stack.Pop().Token;
             var tokenclient = telclient.GetTokenClient(accessToken);
             var nodes = new List<NodeElement>();
             nodes.Add(
               new NodeElement("p", null, body)
             );
             var page = await tokenclient.CreatePageAsync(title,
                                                          nodes.ToArray(),
                                                          returnContent: true);
             forth.Stack.Push(new StackEntry(page.Url));
           }));

          var twr = new StringWriter();
          await forth.Run(verse.Quote.Trim('\''), twr);
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

    private async Task<string> GetCoalite(string inputId, string publicKeyData, string privateKeyData)
    {
      var coaliteServerUri = "https://coaliter.ketchup.pizza";
      var userTag = inputId;
      var coaliteAction = CoaliteAction.CLAIM;
      var mintPayload = "";

      RSA rsa = RSA.Create();
      int bytesRead;
      rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyData), out bytesRead);

      var response = await _client.GetStringAsync($"{coaliteServerUri}/coalite");
      var coaliteResource = JToken.Parse(response).ToObject<CoaliteResource>();


      var request = coaliteResource.CreateActionRequest(rsa, coaliteAction, mintPayload, publicKeyData, userTag);

      var buffer = Encoding.UTF8.GetBytes(request.GetAsSignablePayload());
      var signature = Convert.FromBase64String(request.Signature);
      var clientRsa = RSA.Create();
      int bytesReadPk;
      var pk = request.SignerPublicKey.Split(' ').OrderByDescending(s => s.Length).FirstOrDefault();
      clientRsa.ImportRSAPublicKey(Convert.FromBase64String(pk), out bytesReadPk);

      // Check integrity of the signature of own request.
      if (!clientRsa.VerifyData(buffer, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
      {
        throw new Exception("Could not verify own signature. Something wrong with client keys.");
      }

      var requestStr = JToken.FromObject(request).ToString(Newtonsoft.Json.Formatting.None);
      var requestContent = new StringContent(requestStr,
                                             Encoding.UTF8, "application/json");
      var result = await _client.PostAsync($"{coaliteServerUri}/coalite/action", requestContent);
      var finalResult = await result.Content.ReadAsStringAsync();
      return finalResult;
    }
  }
}