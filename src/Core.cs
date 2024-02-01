using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

[assembly: ModInfo(name: "Auto Close", modID: "autoclose", Side = "Server")]

namespace AutoClose;

public class Core : ModSystem
{
    public static ConfigAutoClose ConfigAutoClose { get; private set; }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        api.RegisterBlockBehaviorClass("AutoClose", typeof(BlockBehaviorAutoClose));
        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        ConfigAutoClose = ModConfig.ReadConfig<ConfigAutoClose>(api, "AutoClose.json");

        foreach (Block block in api.World.Blocks)
        {
            if (!IsAutoCloseCompatible(block))
            {
                continue;
            }

            block.BlockBehaviors = block.BlockBehaviors.Append(new BlockBehaviorAutoClose(block));
            // if (block.CreativeInventoryTabs.Length != 0) block.CreativeInventoryTabs = block.CreativeInventoryTabs.Append("autoclose");
        }
    }

    public static bool IsAutoCloseCompatible(Block block)
    {
        return block.HasBehavior<BlockBehaviorDoor>();
        // || block is BlockFenceGate
        // || block is BlockFenceGateRoughHewn
        // || block is BlockTrapdoor
    }
}