namespace MCForge.World.Blocks
{
    public class DeathWater : Block
    {
        public override string Name
        {
            get { return "deathwater"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.WATER; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}