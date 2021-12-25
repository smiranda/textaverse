using Textaverse.Silo;
using Orleans;
using System;
using Textaverse.GrainInterfaces;

using var client = new ClientBuilder()
    .UseLocalhostClustering()
    .Build();

await client.Connect();

Console.WriteLine(@"
████████╗███████╗██╗░░██╗████████╗░█████╗░██╗░░░██╗███████╗██████╗░░██████╗███████╗
╚══██╔══╝██╔════╝╚██╗██╔╝╚══██╔══╝██╔══██╗██║░░░██║██╔════╝██╔══██╗██╔════╝██╔════╝
░░░██║░░░█████╗░░░╚███╔╝░░░░██║░░░███████║╚██╗░██╔╝█████╗░░██████╔╝╚█████╗░█████╗░░
░░░██║░░░██╔══╝░░░██╔██╗░░░░██║░░░██╔══██║░╚████╔╝░██╔══╝░░██╔══██╗░╚═══██╗██╔══╝░░
░░░██║░░░███████╗██╔╝╚██╗░░░██║░░░██║░░██║░░╚██╔╝░░███████╗██║░░██║██████╔╝███████╗
░░░╚═╝░░░╚══════╝╚═╝░░╚═╝░░░╚═╝░░░╚═╝░░╚═╝░░░╚═╝░░░╚══════╝╚═╝░░╚═╝╚═════╝░╚══════╝");

Console.WriteLine();
Console.WriteLine("Write the name of a new agent: ");
string name = Console.ReadLine();

var room1 = client.GetGrain<IRoomGrain>(0);
await room1.Cast<IRoomAdministrationGrain>().Configure("Hall", "A simple Hall");

var player = client.GetGrain<IAgentGrain>(Guid.NewGuid());
await player.Configure(name, room1);

try
{
  Console.WriteLine(await room1.Description());
}
finally
{
  await client.Close();
}
