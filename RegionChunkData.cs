using Substrate.Core;

namespace NBTExplorer
{
    public class RegionChunkData : NbtDataNode
    {
        public RegionChunkData(RegionFile file, int x, int z)
            : this(null, file, x, z)
        {
        }

        public RegionChunkData(DataNode parent, RegionFile file, int x, int z)
            : base(parent)
        {
            Region = file;
            X = x;
            Z = z;
        }

        public RegionFile Region { get; private set; }
        public int X { get; private set; }
        public int Z { get; private set; }
    }
}