using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagShortDataNode : TagDataNode
    {
        public TagShortDataNode (TagNodeShort tag)
            : base(tag)
        { }

        public override bool EditNode ()
        {
            return EditScalarValue(Tag);
        }
    }
}
