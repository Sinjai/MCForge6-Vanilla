namespace MCForge.World.Blocks
{
    public class ZombieBody : Block
    {
        public override string Name
        {
            get { return "zombie"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.MOSSY_COBBLESTONE; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}