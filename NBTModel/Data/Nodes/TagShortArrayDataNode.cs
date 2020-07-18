using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagShortArrayDataNode : TagDataNode
    {
        public TagShortArrayDataNode(TagNodeShortArray tag)
            : base(tag)
        {
        }

        protected new TagNodeShortArray Tag => base.Tag as TagNodeShortArray;

        public override bool CanEditNode
        {
#if WINDOWS
            get { return true; }
#else
            get { return false; }
#endif
        }

        public override string NodeDisplay => NodeDisplayPrefix + Tag.Data.Length + " shorts";

        public override bool EditNode()
        {
            return EditShortHexValue(Tag);
        }
    }
}