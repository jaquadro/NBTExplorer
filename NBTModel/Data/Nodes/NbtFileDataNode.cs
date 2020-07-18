using NBTModel.Interop;
using Substrate.Core;
using Substrate.Nbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NBTExplorer.Model
{
    public class NbtFileDataNode : DataNode, IMetaTagContainer
    {
        private static readonly Regex _namePattern = new Regex(@"\.(dat|nbt|schematic|dat_mcr|dat_old|bpt|rc)$");
        private readonly CompressionType _compressionType;
        private readonly string _path;

        private CompoundTagContainer _container;
        private NbtTree _tree;

        private NbtFileDataNode(string path, CompressionType compressionType)
        {
            _path = path;
            _compressionType = compressionType;
            _container = new CompoundTagContainer(new TagNodeCompound());
        }

        protected override NodeCapabilities Capabilities => NodeCapabilities.CreateTag
                                                            | NodeCapabilities.PasteInto
                                                            | NodeCapabilities.Search
                                                            | NodeCapabilities.Refresh
                                                            | NodeCapabilities.Rename;

        public override string NodeName => Path.GetFileName(_path);

        public override string NodePathName => Path.GetFileName(_path);

        public override string NodeDisplay
        {
            get
            {
                if (_tree != null && _tree.Root != null)
                    if (!string.IsNullOrEmpty(_tree.Name))
                        return NodeName + " [" + _tree.Name + ": " + _tree.Root.Count + " entries]";
                    else
                        return NodeName + " [" + _tree.Root.Count + " entries]";
                return NodeName;
            }
        }

        public override bool HasUnexpandedChildren => !IsExpanded;

        public override bool IsContainerType => true;

        public override bool CanRenameNode => _tree != null;

        public override bool CanPasteIntoNode =>
            _tree != null && _tree.Root != null && NbtClipboardController.ContainsData;

        public bool IsNamedContainer => true;

        public bool IsOrderedContainer => false;

        public INamedTagContainer NamedTagContainer => _container;

        public IOrderedTagContainer OrderedTagContainer => null;

        public int TagCount => _container.TagCount;

        public bool DeleteTag(TagNode tag)
        {
            return _container.DeleteTag(tag);
        }

        public static NbtFileDataNode TryCreateFrom(string path)
        {
            return TryCreateFrom(path, CompressionType.GZip)
                   ?? TryCreateFrom(path, CompressionType.None);
        }

        private static NbtFileDataNode TryCreateFrom(string path, CompressionType compressionType)
        {
            try
            {
                var file = new NBTFile(path);
                var tree = new NbtTree();
                tree.ReadFrom(file.GetDataInputStream(compressionType));

                if (tree.Root == null)
                    return null;

                return new NbtFileDataNode(path, compressionType);
            }
            catch
            {
                return null;
            }
        }

        public static bool SupportedNamePattern(string path)
        {
            path = Path.GetFileName(path);
            return _namePattern.IsMatch(path);
        }

        protected override void ExpandCore()
        {
            if (_tree == null)
            {
                var file = new NBTFile(_path);
                _tree = new NbtTree();
                _tree.ReadFrom(file.GetDataInputStream(_compressionType));

                if (_tree.Root != null) _container = new CompoundTagContainer(_tree.Root);
            }

            var list = new SortedList<TagKey, TagNode>();
            foreach (var item in _tree.Root) list.Add(new TagKey(item.Key, item.Value.GetTagType()), item.Value);

            foreach (var tag in list.Values)
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
            var file = new NBTFile(_path);
            using (var str = file.GetDataOutputStream(_compressionType))
            {
                _tree.WriteTo(str);
            }
        }

        public override bool RefreshNode()
        {
            var expandSet = BuildExpandSet(this);
            Release();
            RestoreExpandSet(this, expandSet);

            return expandSet != null;
        }

        public override bool RenameNode()
        {
            if (CanRenameNode && FormRegistry.EditString != null)
            {
                var data = new RestrictedStringFormData(_tree.Name ?? "")
                {
                    AllowEmpty = true
                };

                if (FormRegistry.RenameTag(data))
                    if (_tree.Name != data.Value)
                    {
                        _tree.Name = data.Value;
                        IsDataModified = true;
                        return true;
                    }
            }

            return false;
        }

        public override bool CanCreateTag(TagType type)
        {
            return _tree != null && _tree.Root != null && Enum.IsDefined(typeof(TagType), type) &&
                   type != TagType.TAG_END;
        }

        public override bool CreateNode(TagType type)
        {
            if (!CanCreateTag(type))
                return false;

            if (FormRegistry.CreateNode != null)
            {
                var data = new CreateTagFormData
                {
                    TagType = type,
                    HasName = true
                };
                data.RestrictedNames.AddRange(_container.TagNamesInUse);

                if (FormRegistry.CreateNode(data))
                {
                    AddTag(data.TagNode, data.TagName);
                    return true;
                }
            }

            return false;
        }

        public override bool PasteNode()
        {
            if (!CanPasteIntoNode)
                return false;

            var clipboard = NbtClipboardController.CopyFromClipboard();
            if (clipboard == null || clipboard.Node == null)
                return false;

            var name = clipboard.Name;
            if (string.IsNullOrEmpty(name))
                name = "UNNAMED";

            AddTag(clipboard.Node, MakeUniqueName(name));
            return true;
        }

        private void AddTag(TagNode tag, string name)
        {
            _container.AddTag(tag, name);
            IsDataModified = true;

            if (IsExpanded)
            {
                var node = TagDataNode.CreateFromTag(tag);
                if (node != null)
                    Nodes.Add(node);
            }
        }

        private string MakeUniqueName(string name)
        {
            var names = new List<string>(_container.TagNamesInUse);
            if (!names.Contains(name))
                return name;

            var index = 1;
            while (names.Contains(MakeCandidateName(name, index)))
                index++;

            return MakeCandidateName(name, index);
        }

        private string MakeCandidateName(string name, int index)
        {
            return name + " (Copy " + index + ")";
        }
    }
}