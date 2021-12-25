using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Newtonsoft.Json;

namespace Textaverse.Silo
{
  public class BootLoader
  {
    private readonly IGrainFactory _client;

    public BootLoader(IGrainFactory client)
    {
      _client = client;
    }
    public async Task Load()
    {
      await Task.Run(() => { });
    }
  }
}
