using Textaverse.Silo;
using Orleans;
using System;
using Textaverse.GrainInterfaces;
using Textaverse.Parser;
using Textaverse.Models;

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

var room1 = client.GetGrain<IRoomGrain>(0);
await room1.Cast<IRoomAdministrationGrain>().Configure("Hall", "A simple Hall");
var objId = Guid.Parse("e1277f65-7d75-49cf-83e6-b9d206ec2441");
var obj1 = client.GetGrain<IObjectGrain>(objId);
await obj1.Configure("orb");
var op = new ObjectPointer(objId, await obj1.GetName());
//await room1.Cast<IRoomAdministrationGrain>().AddObject(op);

var player = client.GetGrain<IAgentGrain>(Guid.NewGuid());
await player.Configure("Robot", room1.GetPrimaryKeyLong());

string command = args[0];

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
