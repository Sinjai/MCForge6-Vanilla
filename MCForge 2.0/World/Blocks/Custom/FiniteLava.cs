namespace MCForge.World.Blocks
{
    public class FiniteLava : Block
    {
        public override string Name
        {
            get { return "finitelava"; }
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