using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagIntArrayDataNode : TagDataNode
    {
        public TagIntArrayDataNode(TagNodeIntArray tag)
            : base(tag)
        {
        }

        protected new TagNodeIntArray Tag => base.Tag as TagNodeIntArray;

        public override bool CanEditNode
        {
#if WINDOWS
            get { return true; }
#else
            get { return false; }
#endif
        }

        public override string NodeDisplay => NodeDisplayPrefix + Tag.Data.Length + " integers";

        public override bool EditNode()
        {
            return EditIntHexValue(Tag);
        }
    }
}