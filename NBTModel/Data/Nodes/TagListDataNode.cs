using NBTModel.Interop;
using Substrate.Nbt;
using System;

namespace NBTExplorer.Model
{
    public class TagListDataNode : TagDataNode.Container
    {
        private readonly ListTagContainer _container;

        public TagListDataNode(TagNodeList tag)
            : base(tag)
        {
            _container = new ListTagContainer(tag, res => IsDataModified = true);
        }

        public new TagNodeList Tag
        {
            get => base.Tag as TagNodeList;
            set => base.Tag = value;
        }

        public override bool CanPasteIntoNode
        {
            get
            {
                if (NbtClipboardController.ContainsData)
                {
                    var data = NbtClipboardController.CopyFromClipboard();
                    if (data == null)
                        return false;

                    if (data.Node != null && (data.Node.GetTagType() == Tag.ValueType || Tag.Count == 0))
                        return true;
                }

                return false;
            }
        }

        public override bool IsOrderedContainer => true;

        public override IOrderedTagContainer OrderedTagContainer => _container;

        public override int TagCount => _container.TagCount;

        protected override void ExpandCore()
        {
            foreach (var tag in Tag)
            {
                var node = CreateFromTag(tag);
                if (node != null)
                    Nodes.Add(node);
            }
        }

        public override bool CanCreateTag(TagType type)
        {
            if (Tag.Count > 0)
                return Tag.ValueType == type;
            return Enum.IsDefined(typeof(TagType), type) && type != TagType.TAG_END;
        }

        public override bool CreateNode(TagType type)
        {
            if (!CanCreateTag(type))
                return false;

            if (Tag.Count == 0) Tag.ChangeValueType(type);

            AppendTag(DefaultTag(type));
            return true;
        }

        public override bool PasteNode()
        {
            if (!CanPasteIntoNode)
                return false;

            var clipboard = NbtClipboardController.CopyFromClipboard();
            if (clipboard == null || clipboard.Node == null)
                return false;

            if (Tag.Count == 0) Tag.ChangeValueType(clipboard.Node.GetTagType());

            AppendTag(clipboard.Node);
            return true;
        }

        public override bool DeleteTag(TagNode tag)
        {
            return _container.DeleteTag(tag);
        }

        public override void Clear()
        {
            if (TagCount == 0)
                return;

            Nodes.Clear();
            Tag.Clear();

            IsDataModified = true;
        }

        public bool AppendTag(TagNode tag)
        {
            if (tag == null || !CanCreateTag(tag.GetTagType()))
                return false;

            _container.InsertTag(tag, _container.TagCount);
            IsDataModified = true;

            if (IsExpanded)
            {
                var node = CreateFromTag(tag);
                if (node != null)
                    Nodes.Add(node);
            }

            return true;
        }
    }
}