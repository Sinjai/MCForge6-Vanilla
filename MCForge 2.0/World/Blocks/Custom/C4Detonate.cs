namespace MCForge.World.Blocks
{
    public class C4Det : Block
    {
        public override string Name
        {
            get { return "c4det"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.RED_CLOTH; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}