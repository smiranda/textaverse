using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using System;
using System.IO;
using System.Reflection;
using Textaverse.Silo;

var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

// Configure the host
using var host = Host.CreateDefaultBuilder()
    .UseOrleans(siloBuilder =>
    {
      siloBuilder
        .UseLocalhostClustering()
        .UseDashboard(options =>
        {
          options.Username = "guest";
          options.Password = "guest";
          options.Host = "*";
          options.Port = 8080;
          options.HostSelf = true;
          options.CounterUpdateIntervalMs = 10000;
        })
        .AddMemoryGrainStorageAsDefault()
        .AddMemoryGrainStorage(name: "roomStateStore");
    })
    .Build();

// Start the host
await host.StartAsync();

Console.WriteLine("Booting up Textaverse ...");

// Initialize the game world
var client = host.Services.GetRequiredService<IGrainFactory>();
var bootLoader = new BootLoader(client);
await bootLoader.Load();

Console.WriteLine("Boot load completed. Ready for client connections.");

// Exit when any key is pressed
Console.WriteLine("Press any key to exit.");
Console.ReadKey();
await host.StopAsync();

return 0;
