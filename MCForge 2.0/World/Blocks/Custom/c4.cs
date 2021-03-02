namespace MCForge.World.Blocks
{
    public class C4 : Block
    {
        public override string Name
        {
            get { return "c4"; }
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