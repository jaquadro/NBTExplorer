using Substrate.Core;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class RegionChunkDataNode : DataNode, IMetaTagContainer
    {
        private readonly RegionFile _regionFile;
        private CompoundTagContainer _container;
        private NbtTree _tree;

        public RegionChunkDataNode(RegionFile regionFile, int x, int z)
        {
            _regionFile = regionFile;
            X = x;
            Z = z;
            _container = new CompoundTagContainer(new TagNodeCompound());
        }

        public int X { get; }

        public int Z { get; }

        protected RegionFileDataNode RegionParent => Parent as RegionFileDataNode;

        protected override NodeCapabilities Capabilities => NodeCapabilities.CreateTag
                                                            | NodeCapabilities.PasteInto
                                                            | NodeCapabilities.Search
                                                            | NodeCapabilities.Delete;

        public override bool HasUnexpandedChildren => !IsExpanded;

        public override string NodePathName => X + "." + Z;

        public override string NodeDisplay
        {
            get
            {
                var key = _regionFile.parseCoordinatesFromName();
                var absChunk = "";
                if (key != RegionKey.InvalidRegion)
                    absChunk = "        in world at (" + (key.X * 32 + X) + ", " + (key.Z * 32 + Z) + ")";

                return "Chunk [" + X + ", " + Z + "]" + absChunk;
            }
        }

        public override bool IsContainerType => true;

        public bool IsNamedContainer => true;

        public bool IsOrderedContainer => false;

        public INamedTagContainer NamedTagContainer => _container;

        public IOrderedTagContainer OrderedTagContainer => null;

        public int TagCount => _container.TagCount;

        public bool DeleteTag(TagNode tag)
        {
            return _container.DeleteTag(tag);
        }

        protected override void ExpandCore()
        {
            if (_tree == null)
            {
                _tree = new NbtTree();
                _tree.ReadFrom(_regionFile.GetChunkDataInputStream(X, Z));

                if (_tree.Root != null)
                    _container = new CompoundTagContainer(_tree.Root);
            }

            foreach (var tag in _tree.Root.Values)
            {
                var node = TagDataNode.CreateFromTag(tag);
                if (node != null)
                    Nodes.Add(node);
            }
        }

        protected override void ReleaseCore()
        {
            _tree = null;
            Nodes.Clear();
        }

        protected override void SaveCore()
        {
            using (var str = _regionFile.GetChunkDataOutputStream(X, Z))
            {
                _tree.WriteTo(str);
            }
        }

        public override bool DeleteNode()
        {
            if (CanDeleteNode && _regionFile.HasChunk(X, Z))
            {
                RegionParent.QueueDeleteChunk(X, Z);
                IsParentModified = true;
                return Parent.Nodes.Remove(this);
            }

            return false;
        }
    }
}