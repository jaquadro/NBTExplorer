namespace NBTExplorer
{
    public class RegionData : DataNode
    {
        public RegionData(string path)
            : this(null, path)
        {
        }

        public RegionData(DataNode parent, string path)
            : base(parent)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}