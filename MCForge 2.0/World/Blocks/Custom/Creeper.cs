namespace MCForge.World.Blocks
{
    public class Creeper : Block
    {
        public override string Name
        {
            get { return "creeper"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.TNT; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}