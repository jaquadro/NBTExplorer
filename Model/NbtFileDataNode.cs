using System.IO;
using System.Text.RegularExpressions;
using Substrate.Core;
using Substrate.Nbt;
using System.Collections.Generic;

namespace NBTExplorer.Model
{
    public class NbtFileDataNode : DataNode, IMetaTagContainer
    {
        private NbtTree _tree;
        private string _path;
        private CompressionType _compressionType;

        private CompoundTagContainer _container;

        private static Regex _namePattern = new Regex(@"\.(dat|nbt|schematic)$");

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

        public static bool SupportedNamePattern (string path)
        {
            path = Path.GetFileName(path);
            return _namePattern.IsMatch(path);
        }

        protected override NodeCapabilities Capabilities
        {
            get
            {
                return NodeCapabilities.CreateTag
                    | NodeCapabilities.PasteInto
                    | NodeCapabilities.Search
                    | NodeCapabilities.Refresh;
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

        protected override void SaveCore ()
        {
            NBTFile file = new NBTFile(_path);
            using (Stream str = file.GetDataOutputStream(_compressionType)) {
                _tree.WriteTo(str);
            }
        }

        public override bool RefreshNode ()
        {
            Dictionary<string, object> expandSet = BuildExpandSet(this);
            Release();
            RestoreExpandSet(this, expandSet);

            return true;
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
