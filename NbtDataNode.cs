using Substrate.Nbt;

namespace NBTExplorer
{
    public class NbtDataNode : DataNode
    {
        public NbtDataNode()
        {
        }

        public NbtDataNode(DataNode parent)
            : base(parent)
        {
        }

        public NbtTree Tree { get; set; }
    }
}