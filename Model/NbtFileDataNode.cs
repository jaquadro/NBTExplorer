using System.IO;
using Substrate.Core;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class NbtFileDataNode : DataNode, IMetaTagContainer
    {
        private NbtTree _tree;
        private string _path;
        private CompressionType _compressionType;

        private CompoundTagContainer _container;

        private NbtFileDataNode (string path, CompressionType compressionType)
        {
            _path = path;
            _compressionType = compressionType;
            _container = new CompoundTagContainer(new TagNodeCompound());
        }

        public static NbtFileDataNode TryCreateFrom (string path)
        {
            return TryCreateFrom(path, CompressionType.GZip)
                ?? TryCreateFrom(path, CompressionType.None);
        }

        private static NbtFileDataNode TryCreateFrom (string path, CompressionType compressionType)
        {
            try {
                NBTFile file = new NBTFile(path);
                NbtTree tree = new NbtTree();
                tree.ReadFrom(file.GetDataInputStream(compressionType));

                if (tree.Root == null)
                    return null;

                return new NbtFileDataNode(path, compressionType);
            }
            catch {
                return null;
            }
        }

        protected override NodeCapabilities Capabilities
        {
            get
            {
                return NodeCapabilities.CreateTag
                    | NodeCapabilities.PasteInto
                    | NodeCapabilities.Search;
            }
        }

        public override string NodeName
        {
            get { return Path.GetFileName(_path); }
        }

        public override string NodeDisplay
        {
            get { return NodeName; }
        }

        public override bool HasUnexpandedChildren
        {
            get { return !IsExpanded; }
        }

        protected override void ExpandCore ()
        {
            if (_tree == null) {
                NBTFile file = new NBTFile(_path);
                _tree = new NbtTree();
                _tree.ReadFrom(file.GetDataInputStream(_compressionType));

                if (_tree.Root != null)
                    _container = new CompoundTagContainer(_tree.Root);
            }

            foreach (TagNode tag in _tree.Root.Values) {
                TagDataNode node = TagDataNode.CreateFromTag(tag);
                if (node != null)
                    Nodes.Add(node);
            }
        }

        protected override void ReleaseCore ()
        {
            _tree = null;
            Nodes.Clear();
        }

        public bool IsNamedContainer
        {
            get { return true; }
        }

        public bool IsOrderedContainer
        {
            get { return false; }
        }

        public INamedTagContainer NamedTagContainer
        {
            get { return _container; }
        }

        public IOrderedTagContainer OrderedTagContainer
        {
            get { return null; }
        }

        public int TagCount
        {
            get { return _container.TagCount; }
        }

        public bool DeleteTag (TagNode tag)
        {
            return _container.DeleteTag(tag);
        }
    }
}
