using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace AutoClose;

public class BlockBehaviorAutoClose : BlockBehavior
{
    public BlockBehaviorAutoClose(Block block) : base(block) { }

    public static int GetDelay(Block block)
    {
        if (Core.ConfigAutoClose.Delay.ContainsKey(block.Code.ToString()))
        {
            return Core.ConfigAutoClose.Delay[block.Code.ToString()];
        }
        return Core.ConfigAutoClose.DefaultDelay;
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
    {
        handling = EnumHandling.PassThrough;
        world.RegisterCallbackUnique((world, pos, time) => TryAutoClose(world, blockSel, time), blockSel?.Position, GetDelay(this.block));
        return base.OnBlockInteractStart(world, byPlayer, blockSel, ref handling);
    }

    public override void Activate(IWorldAccessor world, Caller caller, BlockSelection blockSel, ITreeAttribute activationArgs, ref EnumHandling handling)
    {
        handling = EnumHandling.PassThrough;
        if (activationArgs?.GetAsBool("isAutoClose") == true)
        {
            return;
        }
        world.RegisterCallbackUnique((world, pos, time) => TryAutoClose(world, blockSel, time), blockSel?.Position, GetDelay(this.block));
    }

    private void TryAutoClose(IWorldAccessor world, BlockSelection blockSel, float time)
    {
        Caller caller = new Caller()
        {
            Player = world.NearestPlayer(blockSel.Position.X, blockSel.Position.Y, blockSel.Position.Z)
        };

        TreeAttribute activationArgs = new();
        activationArgs.SetBool("opened", false);
        activationArgs.SetBool("isAutoClose", true);

        BEBehaviorDoor behavior = world.BlockAccessor.GetBlockEntity(blockSel?.Position)?.GetBehavior<BEBehaviorDoor>();
        if (behavior != null && behavior.Opened == true)
        {
            this.block.Activate(world, caller, blockSel, activationArgs);
        }
    }
}
