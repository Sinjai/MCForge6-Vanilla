namespace MCForge.World.Blocks
{
    public class Bird : Block
    {
        public override string Name
        {
            get { return "bird"; }
        }
        public override byte VisibleBlock
        {
            get { return BlockList.BLUE_CLOTH; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}