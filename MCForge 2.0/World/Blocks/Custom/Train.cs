namespace MCForge.World.Blocks
{
    public class Train : Block
    {
        public override string Name
        {
            get { return "train"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.CYAN_CLOTH; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}