using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagDoubleDataNode : TagDataNode
    {
        public TagDoubleDataNode (TagNodeDouble tag)
            : base(tag)
        { }

        protected new TagNodeDouble Tag
        {
            get { return base.Tag as TagNodeDouble; }
        }

        public override bool Parse (string value)
        {
            double data;
            if (!double.TryParse(value, out data))
                return false;

            Tag.Data = data;
            IsDataModified = true;

            return true;
        }

        public override bool EditNode ()
        {
            return EditScalarValue(Tag);
        }
    }
}
