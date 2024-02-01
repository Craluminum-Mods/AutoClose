using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace AutoClose;

public class ConfigAutoClose : IModConfig
{
    public readonly string Comment = "Delay in milliseconds";
    public readonly int DefaultDelay = 3000;
    public Dictionary<string, int> Delay { get; set; } = new();

    public ConfigAutoClose(ICoreAPI api, ConfigAutoClose previousConfig = null)
    {
        if (previousConfig != null)
        {
            foreach ((string key, int value) in previousConfig.Delay)
            {
                if (!Delay.ContainsKey(key))
                {
                    Delay.Add(key, value);
                }
            }
        }

        if (api != null)
        {
            FillDefault(api);
        }
    }

    private void FillDefault(ICoreAPI api)
    {
        foreach (Block key in api.World.Blocks.Where(Core.IsAutoCloseCompatible).ToList())
        {
            if (!Delay.ContainsKey(key.Code.ToString()))
            {
                Delay.Add(key.Code.ToString(), DefaultDelay);
            }
        }
    }
}