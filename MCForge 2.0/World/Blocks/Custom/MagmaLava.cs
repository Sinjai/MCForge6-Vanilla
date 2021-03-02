namespace MCForge.World.Blocks
{
    public class MagmaLava : Block
    {
        public override string Name
        {
            get { return "magmalava"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.LAVA; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}