namespace MCForge.World.Blocks
{
    public class RocketStart : Block
    {
        public override string Name
        {
            get { return "rocketstart"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.GOLD_BLOCK; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}