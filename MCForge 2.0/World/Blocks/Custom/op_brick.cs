namespace MCForge.World.Blocks
{
    public class OPBrick : Block
    {
        public override string Name
        {
            get { return "op_brick"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.BRICK; }
        }
        public override byte Permission
        {
            get { return 80; }
        }
    }
}