using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagStringDataNode : TagDataNode
    {
        public TagStringDataNode (TagNodeString tag)
            : base(tag)
        { }

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
