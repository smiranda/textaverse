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
await room1.Cast<IRoomAdministrationGrain>().AddObject(op);

var objId2 = Guid.Parse("66ee60fc-9f68-4f5b-a8e2-c2c1e7659649");
var obj2 = client.GetGrain<IForthObjectGrain>(objId2);
await obj2.Configure("forth-interpreter");
var op2 = new ObjectPointer(objId2, await obj2.GetName());
await room1.Cast<IRoomAdministrationGrain>().AddObject(op2);


var player = client.GetGrain<IAgentGrain>(Guid.NewGuid());
await player.Configure("Robot", room1.GetPrimaryKeyLong());

Console.WriteLine("You are in:");
Console.WriteLine(await room1.Description());

var exit = false;
try
{
  do
  {
    try
    {
      string command = Console.ReadLine();
      if (command == ("exit"))
      {
        exit = true;
        break;
      }
      var parser = new VerseParser();
      var commands = parser.Parse(command).Commands;
      foreach (var cmd in commands)
      {
        var result = await player.ExecuteCommand(cmd);
        Console.WriteLine(result.Message);
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
  } while (!exit);
}
finally
{
  await client.Close();
}
