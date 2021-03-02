namespace MCForge.World.Blocks
{
    public class LavaDown : Block
    {
        public override string Name
        {
            get { return "lavadown"; }
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