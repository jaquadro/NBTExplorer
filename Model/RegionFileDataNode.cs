using System.IO;
using System.Windows.Forms;
using Substrate.Core;

namespace NBTExplorer.Model
{
    public class RegionFileDataNode : DataNode
    {
        private string _path;
        private RegionFile _region;

        private RegionFileDataNode (string path)
        {
            _path = path;
        }

        public static RegionFileDataNode TryCreateFrom (string path)
        {
            return new RegionFileDataNode(path);
        }

        protected override NodeCapabilities Capabilities
        {
            get
            {
                return NodeCapabilities.Search;
            }
        }

        public override bool HasUnexpandedChildren
        {
            get { return !IsExpanded; }
        }

        public override string NodeDisplay
        {
            get { return Path.GetFileName(_path); }
        }

        protected override void ExpandCore ()
        {
            try {
                if (_region == null)
                    _region = new RegionFile(_path);

                for (int x = 0; x < 32; x++) {
                    for (int z = 0; z < 32; z++) {
                        if (_region.HasChunk(x, z)) {
                            Nodes.Add(new RegionChunkDataNode(_region, x, z));
                        }
                    }
                }
            }
            catch {
                MessageBox.Show("Not a valid region file.", "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override void ReleaseCore ()
        {
            _region = null;
            Nodes.Clear();
        }
    }
}
