namespace MCForge.World.Blocks
{
    public class fastdeathlava : Block
    {
        public override string Name
        {
            get { return "fastdeathlava"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.ACTIVE_LAVA; }
        }
        public override byte Permission
        {
            get { return 80; }
        }
    }
}