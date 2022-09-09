using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace AutoClose.Utils
{
  public static class SlidingDoorUtils
  {
    public static bool IsSlidingDoor(Block block) => block.Class is "SlidingDoor";

    public static BlockPos NewPos(BlockSelection blockSel)
    {
      var posY = 0;
      if (blockSel.Block.Variant["part"] == "top") posY = 0;
      if (blockSel.Block.Variant["part"] == "bottom") posY = 1;
      return NewDoorPos(blockSel.Block, blockSel.Position).AddCopy(0, posY, 0);
    }

    public static BlockPos NewDoorPos(Block block, BlockPos pos) => GetNewDoorPos(block, pos);

    public static BlockPos GetNewDoorPos(Block block, BlockPos pos)
    {
      return !((block.Variant?["knobOrientation"] == "left") ^ block.Variant?["state"] == "opened")
        ? pos.AddCopy(BlockFacing.FromCode(block.Variant?["horizontalorientation"]).GetCCW())
        : pos.AddCopy(BlockFacing.FromCode(block.Variant?["horizontalorientation"]).GetCW());
    }
  }
}