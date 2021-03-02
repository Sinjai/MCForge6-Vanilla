namespace MCForge.World.Blocks
{
    public class ZombieHead : Block
    {
        public override string Name
        {
            get { return "zombiehead"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.CYAN_GREEN_CLOTH; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}