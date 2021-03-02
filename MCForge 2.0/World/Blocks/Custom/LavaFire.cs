namespace MCForge.World.Blocks
{
    public class LavaFire : Block
    {
        public override string Name
        {
            get { return "lavafire"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.ACTIVE_LAVA; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}