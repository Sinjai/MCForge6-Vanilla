namespace MCForge.World.Blocks
{
    public class DeathLava : Block
    {
        public override string Name
        {
            get { return "deathlava"; }
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