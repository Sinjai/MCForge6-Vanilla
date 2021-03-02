namespace MCForge.World.Blocks
{
    public class BigTNT : Block
    {
        public override string Name
        {
            get { return "bigtnt"; }
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