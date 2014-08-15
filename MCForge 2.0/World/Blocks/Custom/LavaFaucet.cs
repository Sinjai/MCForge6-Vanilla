namespace MCForge.World.Blocks
{
    public class LavaFaucet : Block
    {
        public override string Name
        {
            get { return "lavafaucet"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.LAVA; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}