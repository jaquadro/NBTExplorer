using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagLongArrayDataNode : TagDataNode
    {
        public TagLongArrayDataNode(TagNodeLongArray tag)
            : base(tag)
        {
        }

        protected new TagNodeLongArray Tag => base.Tag as TagNodeLongArray;

        public override bool CanEditNode
        {
#if WINDOWS
            get { return true; }
#else
            get { return false; }
#endif
        }

        public override string NodeDisplay => NodeDisplayPrefix + Tag.Data.Length + " long integers";

        public override bool EditNode()
        {
            return EditLongHexValue(Tag);
        }
    }
}