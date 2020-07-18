using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagByteDataNode : TagDataNode
    {
        public TagByteDataNode(TagNodeByte tag)
            : base(tag)
        {
        }

        protected new TagNodeByte Tag => base.Tag as TagNodeByte;

        public override string NodeDisplay => NodeDisplayPrefix + unchecked((sbyte)Tag.Data);

        public override bool Parse(string value)
        {
            byte data;
            if (!byte.TryParse(value, out data))
                return false;

            Tag.Data = data;
            IsDataModified = true;

            return true;
        }

        public override bool EditNode()
        {
            return EditScalarValue(Tag);
        }
    }
}