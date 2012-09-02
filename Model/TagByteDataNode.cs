using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagByteDataNode : TagDataNode
    {
        public TagByteDataNode (TagNodeByte tag)
            : base(tag)
        { }

        protected new TagNodeByte Tag
        {
            get { return base.Tag as TagNodeByte; }
        }

        public override bool EditNode ()
        {
            return EditScalarValue(Tag);
        }

        public override string NodeDisplay
        {
            get { return NodeDisplayPrefix + unchecked((sbyte)Tag.Data).ToString(); }
        }
    }
}
