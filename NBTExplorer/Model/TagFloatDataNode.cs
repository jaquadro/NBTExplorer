using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagFloatDataNode : TagDataNode
    {
        public TagFloatDataNode (TagNodeFloat tag)
            : base(tag)
        { }

        public override bool EditNode ()
        {
            return EditScalarValue(Tag);
        }
    }
}
