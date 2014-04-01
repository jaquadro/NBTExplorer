using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NBTModel.Interop;

namespace NBTExplorer.Model
{
    public class CubicRegionDataNode : DataNode
    {
        private string _path;
        private CubicRegionFile _region;

        private static Regex _namePattern = new Regex(@"^r2(\.-?\d+){3}\.(mcr|mca)$");

        private CubicRegionDataNode (string path)
        {
            _path = path;
        }

        public static CubicRegionDataNode TryCreateFrom (string path)
        {
            return new CubicRegionDataNode(path);
        }

        public static bool SupportedNamePattern (string path)
        {
            path = Path.GetFileName(path);
            return _namePattern.IsMatch(path);
        }

        protected override NodeCapabilities Capabilities
        {
            get
            {
                return NodeCapabilities.Search
                    | NodeCapabilities.Refresh;
            }
        }

        public override bool HasUnexpandedChildren
        {
            get { return !IsExpanded; }
        }

        public override bool IsContainerType
        {
            get { return true; }
        }

        public override string NodePathName
        {
            get { return Path.GetFileName(_path); }
        }

        public override string NodeDisplay
        {
            get { return Path.GetFileName(_path); }
        }

        protected override void ExpandCore ()
        {
            try {
                if (_region == null)
                    _region = new CubicRegionFile(_path);

                for (int x = 0; x < 32; x++) {
                    for (int z = 0; z < 32; z++) {
                        if (_region.HasChunk(x, z)) {
                            Nodes.Add(new RegionChunkDataNode(_region, x, z));
                        }
                    }
                }
            }
            catch {
                if (FormRegistry.MessageBox != null)
                    FormRegistry.MessageBox("Not a valid cubic region file.");
            }
        }

        protected override void ReleaseCore ()
        {
            if (_region != null)
                _region.Close();
            _region = null;
            Nodes.Clear();
        }

        public override bool RefreshNode ()
        {
            Dictionary<string, object> expandSet = BuildExpandSet(this);
            Release();
            RestoreExpandSet(this, expandSet);

            return expandSet != null;
        }
    }
}
