namespace MCForge.World.Blocks
{
    public class RocketHead : Block
    {
        public override string Name
        {
            get { return "rockethead"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.GLASS; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}