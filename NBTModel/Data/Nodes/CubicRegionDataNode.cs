using NBTModel.Interop;
using System.IO;
using System.Text.RegularExpressions;

namespace NBTExplorer.Model
{
    public class CubicRegionDataNode : DataNode
    {
        private static readonly Regex _namePattern = new Regex(@"^r2(\.-?\d+){3}\.(mcr|mca)$");
        private readonly string _path;
        private CubicRegionFile _region;

        private CubicRegionDataNode(string path)
        {
            _path = path;
        }

        protected override NodeCapabilities Capabilities => NodeCapabilities.Search
                                                            | NodeCapabilities.Refresh;

        public override bool HasUnexpandedChildren => !IsExpanded;

        public override bool IsContainerType => true;

        public override string NodePathName => Path.GetFileName(_path);

        public override string NodeDisplay => Path.GetFileName(_path);

        public static CubicRegionDataNode TryCreateFrom(string path)
        {
            return new CubicRegionDataNode(path);
        }

        public static bool SupportedNamePattern(string path)
        {
            path = Path.GetFileName(path);
            return _namePattern.IsMatch(path);
        }

        protected override void ExpandCore()
        {
            try
            {
                if (_region == null)
                    _region = new CubicRegionFile(_path);

                for (var x = 0; x < 32; x++)
                    for (var z = 0; z < 32; z++)
                        if (_region.HasChunk(x, z)) Nodes.Add(new RegionChunkDataNode(_region, x, z));
            }
            catch
            {
                if (FormRegistry.MessageBox != null)
                    FormRegistry.MessageBox("Not a valid cubic region file.");
            }
        }

        protected override void ReleaseCore()
        {
            if (_region != null)
                _region.Close();
            _region = null;
            Nodes.Clear();
        }

        public override bool RefreshNode()
        {
            var expandSet = BuildExpandSet(this);
            Release();
            RestoreExpandSet(this, expandSet);

            return expandSet != null;
        }
    }
}