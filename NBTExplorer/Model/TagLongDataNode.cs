using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagLongDataNode : TagDataNode
    {
        public TagLongDataNode (TagNodeLong tag)
            : base(tag)
        { }

        public override bool EditNode ()
        {
            return EditScalarValue(Tag);
        }
    }
}
