namespace MCForge.World.Blocks
{
    public class Firework : Block
    {
        public override string Name
        {
            get { return "firework"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.IRON_BLOCK; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}