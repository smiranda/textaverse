using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Textaverse.GrainInterfaces;
using Textaverse.Models;

namespace Textaverse.Grains
{
  /// <summary>
  /// A standard grain.
  /// </summary>
  public class SoulGrain : Grain, ISoulGrain
  {
    private readonly IPersistentState<SoulState> _soulState;
    private StreamSubscriptionHandle<ChatMessage> _agentchatOutSubscription;

    public SoulGrain([PersistentState("agentState", "agentStateStore")] IPersistentState<SoulState> agentState)
    {
      _soulState = agentState;
    }
    public override async Task OnActivateAsync()
    {
      // On grain activation, recover any subscription handle which was already registered
      var streamProvider = GetStreamProvider("SMSProvider");
      if (_soulState.State?.AgentPointer?.Key != null)
      {
        var stream = streamProvider.GetStream<ChatMessage>(new Guid(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(_soulState.State.AgentPointer.Key)).Take(16).ToArray()), "AgentChat.Out");
        var subscriptionHandles = await stream.GetAllSubscriptionHandles();
        if (subscriptionHandles?.Count > 0)
        {
          // We're only listening to one room at a time.
          _agentchatOutSubscription = subscriptionHandles.First();
          await _agentchatOutSubscription.ResumeAsync(OnNextChatMessage,
                                                     OnErrorChatMessage,
                                                     OnCompletedChatMessage);
        }
      }
      await base.OnActivateAsync();
    }

    private async Task OnNextChatMessage(ChatMessage message, StreamSequenceToken token)
    {
      var agentId = _soulState?.State?.AgentPointer?.Key;
      if(agentId != null && message?.Speaker?.Key != agentId) {
        Console.WriteLine("(Soul hears) " + message.Text);
        var agent = GrainFactory.GetGrain<IAgentGrain>(agentId);
        await agent.ExecuteCommand( new Command(new Verb("shout"),quote:"Silence !"));
      }
    }
    private async Task OnErrorChatMessage(Exception e)
    {
      await Task.CompletedTask;
    }
    private async Task OnCompletedChatMessage()
    {
      await Task.CompletedTask;
    }
    public async Task Configure(AgentPointer agentPointer)
    {
      _soulState.State = new SoulState(agentPointer);
      await _soulState.WriteStateAsync();

      if (_agentchatOutSubscription != null)
      {
        await _agentchatOutSubscription.UnsubscribeAsync();
      }
      var streamProvider = GetStreamProvider("SMSProvider");
      var stream = streamProvider.GetStream<ChatMessage>(new Guid(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(_soulState.State.AgentPointer.Key)).Take(16).ToArray()), "AgentChat.Out");

      _agentchatOutSubscription = await stream.SubscribeAsync(OnNextChatMessage,
                                                             OnErrorChatMessage,
                                                             OnCompletedChatMessage);
    }
  }
}