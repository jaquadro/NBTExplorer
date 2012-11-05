using System;
using System.Collections.Generic;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagCompoundDataNode : TagDataNode.Container
    {
        private CompoundTagContainer _container;

        public TagCompoundDataNode (TagNodeCompound tag)
            : base(tag)
        {
            _container = new CompoundTagContainer(tag);
        }

        protected new TagNodeCompound Tag
        {
            get { return base.Tag as TagNodeCompound; }
        }

        protected override void ExpandCore ()
        {
            var list = new SortedList<TagKey, TagNode>();
            foreach (var item in Tag) {
                list.Add(new TagKey(item.Key, item.Value.GetTagType()), item.Value);
            }

            foreach (var item in list) {
                TagDataNode node = TagDataNode.CreateFromTag(item.Value);
                if (node != null)
                    Nodes.Add(node);
            }
        }

        public override bool CanCreateTag (TagType type)
        {
            return Enum.IsDefined(typeof(TagType), type) && type != TagType.TAG_END;
        }

        public override bool CanPasteIntoNode
        {
            get { return NbtClipboardData.ContainsData; }
        }

        public override bool CreateNode (TagType type)
        {
            if (!CanCreateTag(type))
                return false;

            if (FormRegistry.CreateNode != null) {
                CreateTagFormData data = new CreateTagFormData() {
                    TagType = type, HasName = true,
                };
                data.RestrictedNames.AddRange(_container.TagNamesInUse);

                if (FormRegistry.CreateNode(data)) {
                    AddTag(data.TagNode, data.TagName);
                    return true;
                }
            }

            return false;
        }

        public override bool PasteNode ()
        {
            if (!CanPasteIntoNode)
                return false;

            NbtClipboardData clipboard = NbtClipboardData.CopyFromClipboard();
            if (clipboard.Node == null)
                return false;

            string name = clipboard.Name;
            if (String.IsNullOrEmpty(name))
                name = "UNNAMED";

            AddTag(clipboard.Node, MakeUniqueName(name));
            return true;
        }

        public override bool IsNamedContainer
        {
            get { return true; }
        }

        public override INamedTagContainer NamedTagContainer
        {
            get { return _container; }
        }

        public override int TagCount
        {
            get { return _container.TagCount; }
        }

        public override bool DeleteTag (TagNode tag)
        {
            return _container.DeleteTag(tag);
        }

        private void AddTag (TagNode tag, string name)
        {
            _container.AddTag(tag, name);
            IsModified = true;

            if (IsExpanded) {
                TagDataNode node = TagDataNode.CreateFromTag(tag);
                if (node != null)
                    Nodes.Add(node);
            }
        }

        private string MakeUniqueName (string name)
        {
            List<string> names = new List<string>(_container.TagNamesInUse);
            if (!names.Contains(name))
                return name;

            int index = 1;
            while (names.Contains(MakeCandidateName(name, index)))
                index++;

            return MakeCandidateName(name, index);
        }

        private string MakeCandidateName (string name, int index)
        {
            return name + " (Copy " + index + ")";
        }
    }
}
