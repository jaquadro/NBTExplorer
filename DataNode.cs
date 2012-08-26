using Substrate.Core;
using Substrate.Nbt;

namespace NBTExplorer
{
    public class DataNode
    {
        public DataNode ()
        {
        }

        public DataNode (DataNode parent)
        {
            Parent = parent;
        }

        public DataNode Parent { get; set; }

        private bool _modified;
        public bool Modified
        {
            get { return _modified; }
            set
            {
                if (value && Parent != null) {
                    Parent.Modified = value;
                }
                _modified = value;
            }
        }

        public bool Expanded { get; set; }
    }

    public class NbtDataNode : DataNode
    {
        public NbtDataNode ()
        {
        }

        public NbtDataNode (DataNode parent)
            : base(parent)
        {
        }

        public NbtTree Tree { get; set; }
    }

    public class RegionChunkData : NbtDataNode
    {
        public RegionChunkData (RegionFile file, int x, int z)
            : this(null, file, x, z)
        {
        }

        public RegionChunkData (DataNode parent, RegionFile file, int x, int z)
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

    public class RegionData : DataNode
    {
        public RegionData (string path)
            : this(null, path)
        {
        }

        public RegionData (DataNode parent, string path)
            : base(parent)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }

    public class NbtFileData : NbtDataNode
    {
        public NbtFileData (string path, CompressionType cztype)
            : this(null, path, cztype)
        {
        }

        public NbtFileData (DataNode parent, string path, CompressionType cztype)
            : base(parent)
        {
            Path = path;
            CompressionType = cztype;
        }

        public string Path { get; private set; }
        public CompressionType CompressionType { get; private set; }
    }

    public class DirectoryData : DataNode
    {
        public DirectoryData (string path)
            : this(null, path)
        {
        }

        public DirectoryData (DataNode parent, string path)
            : base(parent)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}
