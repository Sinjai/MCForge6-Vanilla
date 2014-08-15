namespace MCForge.World.Blocks
{
    public class NukeTNT : Block
    {
        public override string Name
        {
            get { return "nuketnt"; }
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