namespace MCForge.World.Blocks
{
    public class Fish : Block
    {
        public override string Name
        {
            get { return "fish"; }
        }
        public override byte VisibleBlock
        {
            get { return BlockList.GOLD_BLOCK; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}