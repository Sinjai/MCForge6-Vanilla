namespace MCForge.World.Blocks
{
    public class ActiveDeathLava : Block
    {
        public override string Name
        {
            get { return "activedeathlava"; }
        }
        public override byte VisibleBlock
        {
            get { return Block.BlockList.ACTIVE_LAVA; }
        }
        public override byte Permission
        {
            get { return 50; }
        }
    }
}