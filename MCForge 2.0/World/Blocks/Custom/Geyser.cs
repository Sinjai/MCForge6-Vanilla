namespace MCForge.World.Blocks
{
    public class Geyser : Block
    {
        public override string Name
        {
            get { return "geyser"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.WATER; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}