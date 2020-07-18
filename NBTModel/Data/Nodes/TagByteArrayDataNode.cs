using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagByteArrayDataNode : TagDataNode
    {
        public TagByteArrayDataNode(TagNodeByteArray tag)
            : base(tag)
        {
        }

        protected new TagNodeByteArray Tag => base.Tag as TagNodeByteArray;

        public override bool CanEditNode
        {
#if WINDOWS
            get { return true; }
#else
            get { return false; }
#endif
        }

        public override string NodeDisplay => NodeDisplayPrefix + Tag.Data.Length + " bytes";

        public override bool EditNode()
        {
            return EditByteHexValue(Tag);
        }
    }
}