namespace MCForge.World.Blocks
{
    public class OpStone : Block
    {
        public override string Name
        {
            get { return "op_stone"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.STONE; }
        }
        public override byte Permission
        {
            get { return 80; }
        }
    }
}