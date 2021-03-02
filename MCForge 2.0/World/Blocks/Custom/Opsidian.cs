namespace MCForge.World.Blocks
{
    public class Opsidian : Block
    {
        public override string Name
        {
            get { return "opsidian"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.OBSIDIAN; }
        }
        public override byte Permission
        {
            get { return 80; }
        }
    }
}