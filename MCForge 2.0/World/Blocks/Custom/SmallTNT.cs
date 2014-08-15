namespace MCForge.World.Blocks
{
    public class SmallTNT : Block
    {
        public override string Name
        {
            get { return "smalltnt"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.TNT; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}