using System;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class TagListDataNode : TagDataNode.Container
    {
        private ListTagContainer _container;

        public TagListDataNode (TagNodeList tag)
            : base(tag)
        {
            _container = new ListTagContainer(tag);
        }

        protected new TagNodeList Tag
        {
            get { return base.Tag as TagNodeList; }
            set { base.Tag = value; }
        }

        protected override void ExpandCore ()
        {
            foreach (TagNode tag in Tag) {
                TagDataNode node = TagDataNode.CreateFromTag(tag);
                if (node != null)
                    Nodes.Add(node);
            }
        }

        public override bool CanCreateTag (TagType type)
        {
            if (Tag.Count > 0)
                return Tag.ValueType == type;
            else
                return Enum.IsDefined(typeof(TagType), type) && type != TagType.TAG_END;
        }

        public override bool CanPasteIntoNode
        {
            get
            {
                if (NbtClipboardController.ContainsData) {
                    NbtClipboardData data = NbtClipboardController.CopyFromClipboard();
                    if (data == null)
                        return false;

                    if (data.Node != null && data.Node.GetTagType() == Tag.ValueType)
                        return true;
                }

                return false;
            }
        }

        public override bool CreateNode (TagType type)
        {
            if (!CanCreateTag(type))
                return false;

            if (Tag.Count == 0) {
                Tag.ChangeValueType(type);
            }

            AppendTag(TagDataNode.DefaultTag(type));
            return true;
        }

        public override bool PasteNode ()
        {
            if (!CanPasteIntoNode)
                return false;

            NbtClipboardData clipboard = NbtClipboardController.CopyFromClipboard();
            if (clipboard == null || clipboard.Node == null)
                return false;

            AppendTag(clipboard.Node);
            return true;
        }

        public override bool IsOrderedContainer
        {
            get { return true; }
        }

        public override IOrderedTagContainer OrderedTagContainer
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

        private void AppendTag (TagNode tag)
        {
            _container.InsertTag(tag, _container.TagCount);
            IsDataModified = true;

            if (IsExpanded) {
                TagDataNode node = TagDataNode.CreateFromTag(tag);
                if (node != null)
                    Nodes.Add(node);
            }
        }
    }
}
