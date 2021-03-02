namespace MCForge.World.Blocks
{
    public class ActiveDeathWater : Block
    {
        public override string Name
        {
            get { return "activedeathwater"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.ACTIVE_WATER; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}