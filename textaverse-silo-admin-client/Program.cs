using Textaverse.Silo;
using Orleans;
using System;
using Textaverse.GrainInterfaces;
using Textaverse.Parser;

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

Console.WriteLine("Write a command: ");
string command = Console.ReadLine();

try
{
  var parser = new VerseParser();
  var commands = parser.Parse(command).Commands;
  foreach (var cmd in commands)
  {
    var result = await player.ExecuteCommand(cmd);
    Console.WriteLine(result.Message);
  }

  Console.WriteLine(await room1.Description());
}
finally
{
  await client.Close();
}
