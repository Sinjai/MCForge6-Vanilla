namespace MCForge.World.Blocks
{
    public class OpGlass : Block
    {
        public override string Name
        {
            get { return "op_glass"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.GLASS; }
        }
        public override byte Permission
        {
            get { return 80; }
        }
    }
}