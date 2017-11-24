using System;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagLongArrayDataNode : TagDataNode
    {
        public TagLongArrayDataNode(TagNodeLongArray tag)
            : base(tag)
        { }

        protected new TagNodeLongArray Tag
        {
            get { return base.Tag as TagNodeLongArray; }
        }

        public override bool CanEditNode
        {
#if WINDOWS
            get { return true; }
#else
            get { return false; }
#endif
        }

        public override bool EditNode()
        {
            return EditLongHexValue(Tag);
        }

        public override string NodeDisplay
        {
            get { return NodeDisplayPrefix + Tag.Data.Length + " long integers"; }
        }
    }
}
