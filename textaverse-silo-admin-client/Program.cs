using Textaverse.Silo;
using Orleans;
using System;
using Textaverse.GrainInterfaces;
using Textaverse.Parser;
using Textaverse.Models;
using Orleans.Hosting;
using Orleans.Streams;
using System.Threading.Tasks;

var create = args.Length > 0 && args[0] == "--create";

using var client = new ClientBuilder()
                      .UseLocalhostClustering()
                      .AddSimpleMessageStreamProvider("SMSProvider")
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

var room1Id = Guid.Parse("b4d1a6f0-752b-4d3b-a8d4-e878a381b884");
var room1 = client.GetGrain<IRoomGrain>(room1Id);

if(create) {
  await room1.Cast<IRoomAdministrationGrain>().Configure("Hall", "A simple Hall");

  var room2Id = Guid.Parse("f215ead8-eb61-4c38-b403-87c2d938f36b");
  var room2 = client.GetGrain<IRoomGrain>(room2Id);
  await room2.Cast<IRoomAdministrationGrain>().Configure("DarkRoom", "A small dark room");

  await room1.Cast<IRoomAdministrationGrain>().AddPassage(
    new PassagePointer("door",
                      new GrainPointer(room1.GetPrimaryKey(),
                                        await room1.GetName(),
                                        GrainType.Room),
                      new GrainPointer(room2.GetPrimaryKey(),
                                        await room2.GetName(),
                                        GrainType.Room)));

  await room2.Cast<IRoomAdministrationGrain>().AddPassage(
    new PassagePointer("door",
                      new GrainPointer(room2.GetPrimaryKey(),
                                        await room2.GetName(),
                                        GrainType.Room),
                      new GrainPointer(room1.GetPrimaryKey(),
                                        await room1.GetName(),
                                        GrainType.Room)));

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

  var objId3 = Guid.Parse("10a3f1ca-a94b-44b1-a821-9a89faaadc4f");
  var obj3 = client.GetGrain<IForthObjectGrain>(objId3);
  await obj3.Configure("odt-clock");
  var op3 = new ObjectPointer(objId3, await obj3.GetName());
  await room1.Cast<IRoomAdministrationGrain>().AddObject(op3);
}

var playerId = Guid.NewGuid();
var player = client.GetGrain<IAgentGrain>(playerId);
await player.Configure("Robot" + playerId, room1.GetPrimaryKey());
await room1.Cast<IRoomConnectivityGrain>().TransferAgent(new AgentPointer(player.GetPrimaryKey(),
                                                                          await player.GetName()));
await player.TransferRoom(room1.GetPrimaryKey());

Console.WriteLine("You are in:");
Console.WriteLine(await room1.Description());

var streamProvider = client.GetStreamProvider("SMSProvider");
var chatStream = streamProvider.GetStream<ChatMessage>(playerId, "AgentChat.Out");
await chatStream.SubscribeAsync<ChatMessage>(async (data, token) =>
  await Task.Run(() => { Console.WriteLine($"(you hear) {data.Text}"); }));

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
