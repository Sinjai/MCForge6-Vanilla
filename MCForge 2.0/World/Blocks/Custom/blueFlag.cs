namespace MCForge.World.Blocks
{
    public class blueFlag : Block
    {
        public override string Name
        {
            get { return "blueflag"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.BLUE_CLOTH; }
        }
        public override byte Permission
        {
            get { return 80; }
        }
    }
}