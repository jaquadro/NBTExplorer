using System.IO;

namespace NBTExplorer.Model
{
    public class DirectoryDataNode : DataNode
    {
        private string _path;

        public DirectoryDataNode (string path)
        {
            _path = path;
        }

        protected override NodeCapabilities Capabilities
        {
            get
            {
                return NodeCapabilities.Search;
            }
        }

        public override string NodeDisplay
        {
            get { return Path.GetFileName(_path); }
        }

        public override bool HasUnexpandedChildren
        {
            get { return !IsExpanded; }
        }

        protected override void ExpandCore ()
        {
            foreach (string dirpath in Directory.GetDirectories(_path)) {
                Nodes.Add(new DirectoryDataNode(dirpath));
            }

            foreach (string filepath in Directory.GetFiles(_path)) {
                string ext = Path.GetExtension(filepath);
                DataNode node = null;

                switch (ext) {
                    case ".mcr":
                    case ".mca":
                        node = RegionFileDataNode.TryCreateFrom(filepath);
                        break;

                    case ".dat":
                    case ".nbt":
                    case ".schematic":
                        node = NbtFileDataNode.TryCreateFrom(filepath);
                        break;
                }

                if (node != null)
                    Nodes.Add(node);
            }
        }

        protected override void ReleaseCore ()
        {
            Nodes.Clear();
        }
    }
}
