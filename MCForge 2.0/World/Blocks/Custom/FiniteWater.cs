namespace MCForge.World.Blocks
{
    public class FiniteWater : Block
    {
        public override string Name
        {
            get { return "finitewater"; }
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