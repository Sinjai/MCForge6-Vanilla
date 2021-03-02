namespace MCForge.World.Blocks
{
    public class Snake : Block
    {
        public override string Name
        {
            get { return "snake"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.BLACK_CLOTH; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}