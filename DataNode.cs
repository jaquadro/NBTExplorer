namespace NBTExplorer
{
    public class DataNode
    {
        public DataNode()
        {
        }

        public DataNode(DataNode parent)
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
                if (value && Parent != null)
                {
                    Parent.Modified = value;
                }
                _modified = value;
            }
        }

        public bool Expanded { get; set; }
    }
}