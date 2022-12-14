using static AutoClose.Utils.SlidingDoorUtils;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using Vintagestory.API.Client;
using Vintagestory.API.Util;

namespace AutoClose
{
  public class BlockBehaviorAutoClose : BlockBehavior
  {
    public BlockBehaviorAutoClose(Block block) : base(block) { }

    public override string GetPlacedBlockInfo(IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
    {
      return base.GetPlacedBlockInfo(world, pos, forPlayer)
      + "\n" + string.Format(Lang.Get("blockinfo-AutoCloseDelay", GetDelay(world) / 1000.00));
    }

    private int GetDelay(IWorldAccessor world)
    {
      if (block.Class is "Gate") return world.Config.GetInt("AutoClose_Gate_DelayMs");
      if (block.Class is "Portcullis") return world.Config.GetInt("AutoClose_Portcullis_DelayMs");
      if (block.Class is "Drawbridge") return world.Config.GetInt("AutoClose_Drawbridge_DelayMs");
      if (block.Class is "SlidingDoor") return world.Config.GetInt("AutoClose_SlidingDoor_DelayMs");
      if (block is BlockFenceGate or BlockFenceGateRoughHewn) return world.Config.GetInt("AutoClose_FenceGate_DelayMs");
      if (block is BlockBaseDoor and not BlockFenceGate or BlockFenceGateRoughHewn) return world.Config.GetInt("AutoClose_Door_DelayMs");
      if (block is BlockTrapdoor) return world.Config.GetInt("AutoClose_Trapdoor_DelayMs");
      return 0;
    }

    private static BlockSelection GetSelectionFromPosition(BlockPos pos) => new() { Position = pos };
    private static bool HasOpenedState(Block block) => block.Variant?["state"] == "opened";

    private void RegisterCallback(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, BlockPos newSlidingDoorPos, int millisecondDelay)
    {
      world.RegisterCallbackUnique((world, pos, dt) => TryAutoClose(byPlayer, blockSel, newSlidingDoorPos), blockSel.Position, millisecondDelay);
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
    {
      handling = EnumHandling.PreventSubsequent;
      blockSel.Block = world.BlockAccessor.GetBlock(blockSel.Position);

      if (byPlayer?.Entity.Controls.CtrlKey == true) return base.OnBlockInteractStart(world, byPlayer, blockSel, ref handling);

      if ((blockSel.Block is BlockBaseDoor door && !door.IsOpened())
      || (blockSel.Block is BlockTrapdoor && HasOpenedState(blockSel.Block))
      || (blockSel.Block.Class is "Drawbridge" or "Portcullis" or "Gate" && !HasOpenedState(blockSel.Block)))
      {
        RegisterCallback(world, byPlayer, blockSel, null, GetDelay(world));
      }
      if (blockSel.Block.Class is "SlidingDoor" && !HasOpenedState(blockSel.Block))
      {
        RegisterCallback(world, byPlayer, GetSelectionFromPosition(NewPos(blockSel)), NewPos(blockSel), GetDelay(world));
      }
      return base.OnBlockInteractStart(world, byPlayer, blockSel, ref handling);
    }

    private void TryAutoClose(IPlayer byPlayer, BlockSelection blockSel, BlockPos newSlidingDoorPos)
    {
      blockSel.Block = byPlayer.Entity.World.BlockAccessor.GetBlock(blockSel.Position);
      if ((blockSel.Block is BlockBaseDoor door && door.IsOpened())
        || (blockSel.Block is BlockTrapdoor && HasOpenedState(blockSel.Block))
        || ((blockSel.Block.Class is "Drawbridge" or "Portcullis" or "Gate") && HasOpenedState(blockSel.Block)))
      {
        blockSel.Block.OnBlockInteractStart(byPlayer.Entity.World, byPlayer, blockSel);
      }
      if (blockSel.Block.Class is "SlidingDoor" && HasOpenedState(blockSel.Block))
      {
        blockSel.Block.OnBlockInteractStart(byPlayer.Entity.World, byPlayer, GetSelectionFromPosition(newSlidingDoorPos));
      }
    }

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer, ref EnumHandling handling)
    {
      return new WorldInteraction[] {
        new WorldInteraction()
        {
          ActionLangCode = string.Format("{0}: {1}", Lang.Get("blockhelp-door-openclose"), Lang.Get("blockhelp-WithoutAutoClosing")),
          HotKeyCode = "ctrl",
          MouseButton = EnumMouseButton.Right,
        }
      }.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer, ref handling));
    }
  }
}
