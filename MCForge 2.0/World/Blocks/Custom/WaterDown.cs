namespace MCForge.World.Blocks
{
    public class WaterDown : Block
    {
        public override string Name
        {
            get { return "waterdown"; }
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