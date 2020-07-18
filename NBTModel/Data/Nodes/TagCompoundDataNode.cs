using NBTModel.Interop;
using Substrate.Nbt;
using System;
using System.Collections.Generic;

namespace NBTExplorer.Model
{
    public class TagCompoundDataNode : TagDataNode.Container
    {
        private readonly CompoundTagContainer _container;

        public TagCompoundDataNode(TagNodeCompound tag)
            : base(tag)
        {
            _container = new CompoundTagContainer(tag);
        }

        protected new TagNodeCompound Tag => base.Tag as TagNodeCompound;

        public override bool CanPasteIntoNode => NbtClipboardController.ContainsData;

        public override bool IsNamedContainer => true;

        public override INamedTagContainer NamedTagContainer => _container;

        public override int TagCount => _container.TagCount;

        protected override void ExpandCore()
        {
            var list = new SortedList<TagKey, TagNode>();
            foreach (var item in Tag) list.Add(new TagKey(item.Key, item.Value.GetTagType()), item.Value);

            foreach (var item in list)
            {
                var node = CreateFromTag(item.Value);
                if (node != null)
                    Nodes.Add(node);
            }
        }

        public override bool CanCreateTag(TagType type)
        {
            return Enum.IsDefined(typeof(TagType), type) && type != TagType.TAG_END;
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

        public override bool DeleteTag(TagNode tag)
        {
            return _container.DeleteTag(tag);
        }

        public bool ContainsTag(string name)
        {
            return _container.ContainsTag(name);
        }

        public override void SyncTag()
        {
            var lookup = new Dictionary<TagNode, TagDataNode>();
            foreach (TagDataNode node in Nodes)
                lookup[node.Tag] = node;

            foreach (var kvp in lookup)
                if (!Tag.Values.Contains(kvp.Key))
                    Nodes.Remove(kvp.Value);

            foreach (var tag in Tag.Values)
                if (!lookup.ContainsKey(tag))
                {
                    var newnode = CreateFromTag(tag);
                    if (newnode != null)
                    {
                        Nodes.Add(newnode);
                        newnode.Expand();
                    }
                }

            foreach (TagDataNode node in Nodes)
                node.SyncTag();
        }

        private void AddTag(TagNode tag, string name)
        {
            _container.AddTag(tag, name);
            IsDataModified = true;

            if (IsExpanded)
            {
                var node = CreateFromTag(tag);
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