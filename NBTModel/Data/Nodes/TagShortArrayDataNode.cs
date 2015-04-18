using System;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagShortArrayDataNode : TagDataNode
    {
        public TagShortArrayDataNode (TagNodeShortArray tag)
            : base(tag)
        { }

        protected new TagNodeShortArray Tag
        {
            get { return base.Tag as TagNodeShortArray; }
        }

        public override bool CanEditNode
        {
#if WINDOWS
            get { return true; }
#else
            get { return false; }
#endif
        }

        public override bool EditNode ()
        {
            return EditShortHexValue(Tag);
        }

        public override string NodeDisplay
        {
            get { return NodeDisplayPrefix + Tag.Data.Length + " shorts"; }
        }
    }
}
