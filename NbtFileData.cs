using Substrate.Core;

namespace NBTExplorer
{
    public class NbtFileData : NbtDataNode
    {
        public NbtFileData(string path, CompressionType cztype)
            : this(null, path, cztype)
        {
        }

        public NbtFileData(DataNode parent, string path, CompressionType cztype)
            : base(parent)
        {
            Path = path;
            CompressionType = cztype;
        }

        public string Path { get; private set; }
        public CompressionType CompressionType { get; private set; }
    }
}