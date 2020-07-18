using NBTModel.Interop;
using Substrate.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NBTExplorer.Model
{
    public class RegionFileDataNode : DataNode
    {
        private static readonly Regex _namePattern = new Regex(@"^r\.(-?\d+)\.(-?\d+)\.(mcr|mca)$");
        private readonly List<RegionKey> _deleteQueue = new List<RegionKey>();
        private readonly string _path;
        private RegionFile _region;

        private RegionFileDataNode(string path)
        {
            _path = path;
        }

        protected override NodeCapabilities Capabilities => NodeCapabilities.Search
                                                            | NodeCapabilities.Refresh;

        public override bool HasUnexpandedChildren => !IsExpanded;

        public override bool IsContainerType => true;

        public override string NodePathName => Path.GetFileName(_path);

        public override string NodeDisplay => Path.GetFileName(_path);

        public static RegionFileDataNode TryCreateFrom(string path)
        {
            return new RegionFileDataNode(path);
        }

        public static bool SupportedNamePattern(string path)
        {
            path = Path.GetFileName(path);
            return _namePattern.IsMatch(path);
        }

        public static bool RegionCoordinates(string path, out int rx, out int rz)
        {
            rx = 0;
            rz = 0;

            var match = _namePattern.Match(path);
            if (match.Success && match.Groups.Count > 3)
            {
                rx = int.Parse(match.Groups[1].Value);
                rz = int.Parse(match.Groups[2].Value);
            }

            return match.Success;
        }

        protected override void ExpandCore()
        {
            try
            {
                if (_region == null)
                    _region = new RegionFile(_path);

                for (var x = 0; x < 32; x++)
                    for (var z = 0; z < 32; z++)
                        if (_region.HasChunk(x, z)) Nodes.Add(new RegionChunkDataNode(_region, x, z));
            }
            catch (Exception e)
            {
                if (FormRegistry.MessageBox != null)
                    FormRegistry.MessageBox("Not a valid region file.");
            }
        }

        protected override void ReleaseCore()
        {
            if (_region != null)
                _region.Close();
            _region = null;
            Nodes.Clear();
        }

        protected override void SaveCore()
        {
            foreach (var key in _deleteQueue)
                if (_region.HasChunk(key.X, key.Z))
                    _region.DeleteChunk(key.X, key.Z);

            _deleteQueue.Clear();
        }

        public override bool RefreshNode()
        {
            var expandSet = BuildExpandSet(this);
            Release();
            RestoreExpandSet(this, expandSet);

            return expandSet != null;
        }

        public void QueueDeleteChunk(int rx, int rz)
        {
            var key = new RegionKey(rx, rz);
            if (!_deleteQueue.Contains(key))
                _deleteQueue.Add(key);
        }
    }
}