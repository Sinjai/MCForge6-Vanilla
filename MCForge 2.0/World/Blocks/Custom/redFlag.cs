namespace MCForge.World.Blocks
{
    public class redFlag : Block
    {
        public override string Name
        {
            get { return "redflag"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.RED_CLOTH; }
        }
        public override byte Permission
        {
            get { return 80; }
        }
    }
}