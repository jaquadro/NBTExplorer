using NBTModel.Interop;
using Substrate.Nbt;
using System;
using System.Collections.Generic;

namespace NBTExplorer.Model
{
    public abstract class TagDataNode : DataNode
    {
        private static readonly Dictionary<TagType, Type> _tagRegistry;

        private TagNode _tag;

        static TagDataNode()
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
            _tagRegistry[TagType.TAG_LONG_ARRAY] = typeof(TagLongArrayDataNode);
            _tagRegistry[TagType.TAG_SHORT] = typeof(TagShortDataNode);
            _tagRegistry[TagType.TAG_SHORT_ARRAY] = typeof(TagShortArrayDataNode);
            _tagRegistry[TagType.TAG_STRING] = typeof(TagStringDataNode);
        }

        protected TagDataNode(TagNode tag)
        {
            _tag = tag;
        }

        protected IMetaTagContainer TagParent => Parent as IMetaTagContainer;

        public TagNode Tag
        {
            get => _tag;
            protected set
            {
                if (_tag.GetTagType() == value.GetTagType())
                    _tag = value;
            }
        }

        protected override NodeCapabilities Capabilities => NodeCapabilities.Copy
                                                            | NodeCapabilities.Cut
                                                            | NodeCapabilities.Delete
                                                            | NodeCapabilities.Edit
                                                            | (TagParent != null && TagParent.IsNamedContainer
                                                                ? NodeCapabilities.Rename
                                                                : NodeCapabilities.None)
                                                            | (TagParent != null && TagParent.IsOrderedContainer
                                                                ? NodeCapabilities.Reorder
                                                                : NodeCapabilities.None);

        public override bool CanMoveNodeUp
        {
            get
            {
                if (TagParent != null && TagParent.IsOrderedContainer)
                    return TagParent.OrderedTagContainer.GetTagIndex(Tag) > 0;
                return false;
            }
        }

        public override bool CanMoveNodeDown
        {
            get
            {
                if (TagParent != null && TagParent.IsOrderedContainer)
                    return TagParent.OrderedTagContainer.GetTagIndex(Tag) < TagParent.TagCount - 1;
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

        public override string NodePathName
        {
            get
            {
                if (Parent is Container)
                {
                    var container = Parent as Container;
                    if (container.IsOrderedContainer)
                        return container.OrderedTagContainer.GetTagIndex(Tag).ToString();
                }

                return base.NodePathName;
            }
        }

        protected string NodeDisplayPrefix
        {
            get
            {
                var name = NodeName;
                return string.IsNullOrEmpty(name) ? "" : name + ": ";
            }
        }

        public override string NodeDisplay => NodeDisplayPrefix + Tag;

        public static TagDataNode CreateFromTag(TagNode tag)
        {
            if (tag == null || !_tagRegistry.ContainsKey(tag.GetTagType()))
                return null;

            return Activator.CreateInstance(_tagRegistry[tag.GetTagType()], tag) as TagDataNode;
        }

        public static TagNode DefaultTag(TagType type)
        {
            switch (type)
            {
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

                case TagType.TAG_LONG_ARRAY:
                    return new TagNodeLongArray(new long[0]);

                case TagType.TAG_SHORT:
                    return new TagNodeShort(0);

                case TagType.TAG_SHORT_ARRAY:
                    return new TagNodeShortArray(new short[0]);

                case TagType.TAG_STRING:
                    return new TagNodeString("");

                default:
                    return new TagNodeByte(0);
            }
        }

        public virtual bool Parse(string value)
        {
            return false;
        }

        public override bool DeleteNode()
        {
            if (CanDeleteNode && TagParent != null)
            {
                TagParent.DeleteTag(Tag);
                IsParentModified = true;
                return Parent.Nodes.Remove(this);
            }

            return false;
        }

        public override bool RenameNode()
        {
            if (CanRenameNode && TagParent != null && TagParent.IsNamedContainer && FormRegistry.EditString != null)
            {
                var data = new RestrictedStringFormData(TagParent.NamedTagContainer.GetTagName(Tag));
                data.RestrictedValues.AddRange(TagParent.NamedTagContainer.TagNamesInUse);

                if (FormRegistry.RenameTag(data))
                    if (TagParent.NamedTagContainer.RenameTag(Tag, data.Value))
                    {
                        IsDataModified = true;
                        return true;
                    }
            }

            return false;
        }

        public override bool CopyNode()
        {
            if (CanCopyNode)
            {
                NbtClipboardController.CopyToClipboard(new NbtClipboardData(NodeName, Tag));
                return true;
            }

            return false;
        }

        public override bool CutNode()
        {
            if (CanCutNode && TagParent != null)
            {
                NbtClipboardController.CopyToClipboard(new NbtClipboardData(NodeName, Tag));

                TagParent.DeleteTag(Tag);
                IsParentModified = true;
                Parent.Nodes.Remove(this);
                return true;
            }

            return false;
        }

        public override bool ChangeRelativePosition(int offset)
        {
            if (CanReoderNode && TagParent != null)
            {
                var curIndex = TagParent.OrderedTagContainer.GetTagIndex(Tag);
                var newIndex = curIndex + offset;

                if (newIndex < 0 || newIndex >= TagParent.OrderedTagContainer.TagCount)
                    return false;

                TagParent.OrderedTagContainer.DeleteTag(Tag);
                TagParent.OrderedTagContainer.InsertTag(Tag, newIndex);

                var parent = Parent;
                parent.Nodes.Remove(this);
                parent.Nodes.Insert(newIndex, this);
                IsParentModified = true;
                return true;
            }

            return false;
        }

        protected bool EditScalarValue(TagNode tag)
        {
            if (FormRegistry.EditTagScalar != null)
                if (FormRegistry.EditTagScalar(new TagScalarFormData(tag)))
                {
                    IsDataModified = true;
                    return true;
                }
            return false;
        }

        protected bool EditStringValue(TagNode tag)
        {
            if (FormRegistry.EditString != null)
            {
                var data = new StringFormData(tag.ToTagString().Data);
                if (FormRegistry.EditString(data))
                {
                    tag.ToTagString().Data = data.Value;
                    IsDataModified = true;
                    return true;
                }
            }
            return false;
        }

        protected bool EditByteHexValue(TagNode tag)
        {
            if (FormRegistry.EditByteArray != null)
            {
                var byteData = new byte[tag.ToTagByteArray().Length];
                Array.Copy(tag.ToTagByteArray().Data, byteData, byteData.Length);

                var data = new ByteArrayFormData
                {
                    NodeName = NodeName,
                    BytesPerElement = 1,
                    Data = byteData
                };

                if (FormRegistry.EditByteArray(data))
                {
                    tag.ToTagByteArray().Data = data.Data;
                    //Array.Copy(data.Data, tag.ToTagByteArray().Data, tag.ToTagByteArray().Length);
                    IsDataModified = true;
                    return true;
                }
            }

            return false;
        }

        protected bool EditShortHexValue(TagNode tag)
        {
            if (FormRegistry.EditByteArray != null)
            {
                var iatag = tag.ToTagShortArray();
                var byteData = new byte[iatag.Length * 2];
                for (var i = 0; i < iatag.Length; i++)
                {
                    var buf = BitConverter.GetBytes(iatag.Data[i]);
                    Array.Copy(buf, 0, byteData, 2 * i, 2);
                }

                var data = new ByteArrayFormData
                {
                    NodeName = NodeName,
                    BytesPerElement = 2,
                    Data = byteData
                };

                if (FormRegistry.EditByteArray(data))
                {
                    iatag.Data = new short[data.Data.Length / 2];
                    for (var i = 0; i < iatag.Length; i++) iatag.Data[i] = BitConverter.ToInt16(data.Data, i * 2);

                    IsDataModified = true;
                    return true;
                }
            }

            return false;
        }

        protected bool EditIntHexValue(TagNode tag)
        {
            if (FormRegistry.EditByteArray != null)
            {
                var iatag = tag.ToTagIntArray();
                var byteData = new byte[iatag.Length * 4];
                for (var i = 0; i < iatag.Length; i++)
                {
                    var buf = BitConverter.GetBytes(iatag.Data[i]);
                    Array.Copy(buf, 0, byteData, 4 * i, 4);
                }

                var data = new ByteArrayFormData
                {
                    NodeName = NodeName,
                    BytesPerElement = 4,
                    Data = byteData
                };

                if (FormRegistry.EditByteArray(data))
                {
                    iatag.Data = new int[data.Data.Length / 4];
                    for (var i = 0; i < iatag.Length; i++) iatag.Data[i] = BitConverter.ToInt32(data.Data, i * 4);

                    IsDataModified = true;
                    return true;
                }
            }

            return false;
        }

        protected bool EditLongHexValue(TagNode tag)
        {
            if (FormRegistry.EditByteArray != null)
            {
                var latag = tag.ToTagLongArray();
                var byteData = new byte[latag.Length * 8];
                for (var i = 0; i < latag.Length; i++)
                {
                    var buf = BitConverter.GetBytes(latag.Data[i]);
                    Array.Copy(buf, 0, byteData, 8 * i, 8);
                }

                var data = new ByteArrayFormData
                {
                    NodeName = NodeName,
                    BytesPerElement = 8,
                    Data = byteData
                };

                if (FormRegistry.EditByteArray(data))
                {
                    latag.Data = new long[data.Data.Length / 8];
                    for (var i = 0; i < latag.Length; i++)
                        latag.Data[i] = BitConverter.ToInt64(data.Data, i * 8);

                    IsDataModified = true;
                    return true;
                }
            }

            return false;
        }

        public virtual void SyncTag()
        {
        }

        public abstract class Container : TagDataNode, IMetaTagContainer
        {
            protected Container(TagNode tag)
                : base(tag)
            {
            }

            protected override NodeCapabilities Capabilities => NodeCapabilities.Copy
                                                                | NodeCapabilities.CreateTag
                                                                | NodeCapabilities.Cut
                                                                | NodeCapabilities.Delete
                                                                | NodeCapabilities.PasteInto
                                                                | (TagParent != null && TagParent.IsNamedContainer
                                                                    ? NodeCapabilities.Rename
                                                                    : NodeCapabilities.None)
                                                                | (TagParent != null && TagParent.IsOrderedContainer
                                                                    ? NodeCapabilities.Reorder
                                                                    : NodeCapabilities.None)
                                                                | NodeCapabilities.Search;

            public override bool HasUnexpandedChildren => !IsExpanded && TagCount > 0;

            public override bool IsContainerType => true;

            public override string NodeDisplay =>
                NodeDisplayPrefix + TagCount + (TagCount == 1 ? " entry" : " entries");

            #region ITagContainer

            public virtual int TagCount => 0;

            public virtual bool IsNamedContainer => false;

            public virtual bool IsOrderedContainer => false;

            public virtual INamedTagContainer NamedTagContainer => null;

            public virtual IOrderedTagContainer OrderedTagContainer => null;

            public virtual bool DeleteTag(TagNode tag)
            {
                return false;
            }

            public virtual void Clear()
            {
            }

            #endregion ITagContainer
        }
    }
}