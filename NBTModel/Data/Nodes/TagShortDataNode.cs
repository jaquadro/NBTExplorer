using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagShortDataNode : TagDataNode
    {
        public TagShortDataNode (TagNodeShort tag)
            : base(tag)
        { }

        protected new TagNodeShort Tag
        {
            get { return base.Tag as TagNodeShort; }
        }

        public override bool Parse (string value)
        {
            short data;
            if (!short.TryParse(value, out data))
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
