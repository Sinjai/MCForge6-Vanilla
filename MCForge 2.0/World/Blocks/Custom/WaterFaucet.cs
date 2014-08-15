namespace MCForge.World.Blocks
{
    public class WaterFaucet : Block
    {
        public override string Name
        {
            get { return "waterfaucet"; }
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