namespace MCForge.World.Blocks
{
    public class TNTExplosion : Block
    {
        public override string Name
        {
            get { return "tntexplosion"; }
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