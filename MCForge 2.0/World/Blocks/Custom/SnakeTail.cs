namespace MCForge.World.Blocks
{
    public class SnakeTail : Block
    {
        public override string Name
        {
            get { return "snaketail"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.COAL_ORE; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}