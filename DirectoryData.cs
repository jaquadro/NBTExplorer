namespace NBTExplorer
{
    public class DirectoryData : DataNode
    {
        public DirectoryData(string path)
            : this(null, path)
        {
        }

        public DirectoryData(DataNode parent, string path)
            : base(parent)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}