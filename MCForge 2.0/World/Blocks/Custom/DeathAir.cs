namespace MCForge.World.Blocks
{
    public class DeathAir : Block
    {
        public override string Name
        {
            get { return "deathair"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.AIR; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}