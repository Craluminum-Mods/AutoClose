using AutoClose.Configuration;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

[assembly: ModInfo("Auto Close",
  Authors = new[] { "Craluminum2413" })]

namespace AutoClose
{
  class AutoClose : ModSystem
  {
    public override void Start(ICoreAPI api)
    {
      base.Start(api);
      ModConfig.ReadConfig(api);
      api.RegisterBlockBehaviorClass("AutoClose", typeof(BlockBehaviorAutoClose));
      api.World.Logger.Event("started 'Auto Close' mod");
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
      foreach (var block in api.World.Blocks)
      {
        if (block is BlockBaseDoor || block is BlockTrapdoor
        || block.Class is "SlidingDoor"
        || block.Class is "Drawbridge" or "Gate" or "Portcullis")
        {
          block.CollectibleBehaviors = block.CollectibleBehaviors.Append(new BlockBehaviorAutoClose(block));
          block.BlockBehaviors = block.BlockBehaviors.Append(new BlockBehaviorAutoClose(block));
        }
      }
    }
  }
}