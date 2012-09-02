using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public abstract class TagDataNode : DataNode
    {
        public abstract class Container : TagDataNode, IMetaTagContainer
        {
            protected Container (TagNode tag)
                : base(tag)
            { }

            #region ITagContainer

            public virtual int TagCount
            {
                get { return 0; }
            }

            public virtual bool IsNamedContainer
            {
                get { return false; }
            }

            public virtual bool IsOrderedContainer
            {
                get { return false; }
            }

            public virtual INamedTagContainer NamedTagContainer
            {
                get { return null; }
            }

            public virtual IOrderedTagContainer OrderedTagContainer
            {
                get { return null; }
            }

            public virtual bool DeleteTag (TagNode tag)
            {
                return false;
            }

            #endregion

            protected override NodeCapabilities Capabilities
            {
                get
                {
                    return NodeCapabilities.Copy
                        | NodeCapabilities.CreateTag
                        | NodeCapabilities.Cut
                        | NodeCapabilities.Delete
                        | NodeCapabilities.PasteInto
                        | (TagParent.IsNamedContainer ? NodeCapabilities.Rename : NodeCapabilities.None)
                        | (TagParent.IsOrderedContainer ? NodeCapabilities.Reorder : NodeCapabilities.None)
                        | NodeCapabilities.Search;
                }
            }

            public override bool HasUnexpandedChildren
            {
                get { return !IsExpanded && TagCount > 0; }
            }

            public override string NodeDisplay
            {
                get { return NodeDisplayPrefix + TagCount + ((TagCount == 1) ? " entry" : " entries"); }
            }
        }

        private static Dictionary<TagType, Type> _tagRegistry;

        static TagDataNode ()
        {
            _tagRegistry = new Dictionary<TagType, Type>();
            _tagRegistry[TagType.TAG_BYTE] = typeof(TagByteDataNode);
            _tagRegistry[TagType.TAG_BYTE_ARRAY] = typeof(TagByteArrayDataNode);
            _tagRegistry[TagType.TAG_COMPOUND] = typeof(TagCompoundDataNode);
            _tagRegistry[TagType.TAG_DOUBLE] = typeof(TagDoubleDataNode);
            _tagRegistry[TagType.TAG_FLOAT] = typeof(TagFloatDataNode);
            _tagRegistry[TagType.TAG_INT] = typeof(TagIntDataNode);
            _tagRegistry[TagType.TAG_INT_ARRAY] = typeof(TagIntArrayDataNode);
            _tagRegistry[TagType.TAG_LIST] = typeof(TagListDataNode);
            _tagRegistry[TagType.TAG_LONG] = typeof(TagLongDataNode);
            _tagRegistry[TagType.TAG_SHORT] = typeof(TagShortDataNode);
            _tagRegistry[TagType.TAG_STRING] = typeof(TagStringDataNode);
        }

        static public TagDataNode CreateFromTag (TagNode tag)
        {
            if (tag == null || !_tagRegistry.ContainsKey(tag.GetTagType()))
                return null;

            return Activator.CreateInstance(_tagRegistry[tag.GetTagType()], tag) as TagDataNode;
        }

        static public TagNode DefaultTag (TagType type)
        {
            switch (type) {
                case TagType.TAG_BYTE:
                    return new TagNodeByte(0);
                case TagType.TAG_BYTE_ARRAY:
                    return new TagNodeByteArray(new byte[0]);
                case TagType.TAG_COMPOUND:
                    return new TagNodeCompound();
                case TagType.TAG_DOUBLE:
                    return new TagNodeDouble(0);
                case TagType.TAG_FLOAT:
                    return new TagNodeFloat(0);
                case TagType.TAG_INT:
                    return new TagNodeInt(0);
                case TagType.TAG_INT_ARRAY:
                    return new TagNodeIntArray(new int[0]);
                case TagType.TAG_LIST:
                    return new TagNodeList(TagType.TAG_BYTE);
                case TagType.TAG_LONG:
                    return new TagNodeLong(0);
                case TagType.TAG_SHORT:
                    return new TagNodeShort(0);
                case TagType.TAG_STRING:
                    return new TagNodeString("");
                default:
                    return new TagNodeByte(0);
            }
        }

        private TagNode _tag;

        protected TagDataNode (TagNode tag)
        {
            _tag = tag;
        }

        protected IMetaTagContainer TagParent
        {
            get { return base.Parent as IMetaTagContainer; }
        }

        protected TagNode Tag
        {
            get { return _tag; }
            set
            {
                if (_tag.GetTagType() == value.GetTagType())
                    _tag = value;
            }
        }

        protected override NodeCapabilities Capabilities
        {
            get
            {
                return NodeCapabilities.Copy
                    | NodeCapabilities.Cut
                    | NodeCapabilities.Delete
                    | NodeCapabilities.Edit
                    | (TagParent.IsNamedContainer ? NodeCapabilities.Rename : NodeCapabilities.None)
                    | (TagParent.IsOrderedContainer ? NodeCapabilities.Reorder : NodeCapabilities.None);
            }
        }

        public override bool CanMoveNodeUp
        {
            get
            {
                if (TagParent.IsOrderedContainer)
                    return TagParent.OrderedTagContainer.GetTagIndex(Tag) > 0;
                return false;
            }
        }

        public override bool CanMoveNodeDown
        {
            get
            {
                if (TagParent.IsOrderedContainer)
                    return TagParent.OrderedTagContainer.GetTagIndex(Tag) < (TagParent.TagCount - 1);
                return false;
            }
        }

        public override string NodeName
        {
            get
            {
                if (TagParent == null || !TagParent.IsNamedContainer)
                    return null;

                return TagParent.NamedTagContainer.GetTagName(Tag);
            }
        }

        protected string NodeDisplayPrefix
        {
            get
            {
                string name = NodeName;
                return String.IsNullOrEmpty(name) ? "" : name + ": ";
            }
        }

        public override string NodeDisplay
        {
            get { return NodeDisplayPrefix + Tag.ToString(); }
        }

        public override bool DeleteNode ()
        {
            if (CanDeleteNode) {
                TagParent.DeleteTag(Tag);
                return Parent.Nodes.Remove(this);
            }

            return false;
        }

        public override bool RenameNode ()
        {
            if (CanRenameNode && TagParent.IsNamedContainer) {
                EditName form = new EditName(TagParent.NamedTagContainer.GetTagName(Tag));
                form.InvalidNames.AddRange(TagParent.NamedTagContainer.TagNamesInUse);
                if (form.ShowDialog() == DialogResult.OK && form.IsModified) {
                    if (TagParent.NamedTagContainer.RenameTag(Tag, form.TagName)) {
                        IsModified = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool CopyNode ()
        {
            if (CanCopyNode) {
                NbtClipboardData clip = new NbtClipboardData(NodeName, Tag);
                clip.CopyToClipboard();
                return true;
            }

            return false;
        }

        public override bool CutNode ()
        {
            if (CanCutNode) {
                NbtClipboardData clip = new NbtClipboardData(NodeName, Tag);
                clip.CopyToClipboard();

                TagParent.DeleteTag(Tag);
                Parent.Nodes.Remove(this);
                return true;
            }

            return false;
        }

        public override bool ChangeRelativePosition (int offset)
        {
            if (CanReoderNode) {
                int curIndex = TagParent.OrderedTagContainer.GetTagIndex(Tag);
                int newIndex = curIndex + offset;

                if (newIndex < 0 || newIndex >= TagParent.OrderedTagContainer.TagCount)
                    return false;

                TagParent.OrderedTagContainer.DeleteTag(Tag);
                TagParent.OrderedTagContainer.InsertTag(Tag, newIndex);

                DataNode parent = Parent;
                parent.Nodes.Remove(this);
                parent.Nodes.Insert(newIndex, this);
                parent.IsModified = true;
                return true;
            }

            return false;
        }

        protected bool EditScalarValue (TagNode tag)
        {
            EditValue form = new EditValue(tag);
            if (form.ShowDialog() == DialogResult.OK) {
                IsModified = true;
                return true;
            }
            else
                return false;
        }

        protected bool EditStringValue (TagNode tag)
        {
            EditString form = new EditString(tag.ToTagString().Data);
            if (form.ShowDialog() == DialogResult.OK) {
                tag.ToTagString().Data = form.StringValue;

                IsModified = true;
                return true;
            }
            else
                return false;
        }

        protected bool EditByteHexValue (TagNode tag)
        {
            HexEditor form = new HexEditor(NodeName, tag.ToTagByteArray().Data, 1);
            if (form.ShowDialog() == DialogResult.OK && form.Modified) {
                Array.Copy(form.Data, tag.ToTagByteArray().Data, tag.ToTagByteArray().Length);

                IsModified = true;
                return true;
            }
            else
                return false;
        }

        protected bool EditIntHexValue (TagNode tag)
        {
            TagNodeIntArray iatag = tag.ToTagIntArray();
            byte[] data = new byte[iatag.Length * 4];
            for (int i = 0; i < iatag.Length; i++) {
                byte[] buf = BitConverter.GetBytes(iatag.Data[i]);
                Array.Copy(buf, 0, data, 4 * i, 4);
            }

            HexEditor form = new HexEditor(NodeName, data, 4);
            if (form.ShowDialog() == DialogResult.OK && form.Modified) {
                for (int i = 0; i < iatag.Length; i++) {
                    iatag.Data[i] = BitConverter.ToInt32(form.Data, i * 4);
                }

                IsModified = true;
                return true;
            }
            else
                return false;
        }
    }
}
