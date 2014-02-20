using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagStringDataNode : TagDataNode
    {
        public TagStringDataNode (TagNodeString tag)
            : base(tag)
        { }

        protected new TagNodeString Tag
        {
            get { return base.Tag as TagNodeString; }
        }

        public override bool Parse (string value)
        {
            Tag.Data = value;
            IsDataModified = true;

            return true;
        }

        public override bool EditNode ()
        {
            return EditStringValue(Tag);
        }

        public override string NodeDisplay
        {
            get { return NodeDisplayPrefix + Tag.ToString().Replace('\n', (char)0x00B6); }
        }
    }
}
