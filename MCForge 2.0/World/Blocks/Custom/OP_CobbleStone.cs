namespace MCForge.World.Blocks
{
    public class OPCobbleStone : Block
    {
        public override string Name
        {
            get { return "op_cobblestone"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.COBBLESTONE; }
        }
        public override byte Permission
        {
            get { return 80; }
        }
    }
}