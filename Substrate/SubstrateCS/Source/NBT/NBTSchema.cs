using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.NBT
{
    [Flags]
    public enum NBTOptions
    {
        OPTIONAL = 0x1,
        CREATE_ON_MISSING = 0x2,
    }

    public abstract class NBTSchemaNode
    {
        private string _name;
        private NBTOptions _options;

        public string Name
        {
            get { return _name; }
        }

        public NBTOptions Options
        {
            get { return _options; }
        }

        public NBTSchemaNode (string name)
        {
            _name = name;
        }

        public NBTSchemaNode (string name, NBTOptions options)
        {
            _name = name;
            _options = options;
        }

        public virtual TagValue BuildDefaultTree ()
        {
            return null;
        }
    }

    public sealed class NBTScalerNode : NBTSchemaNode
    {
        private TagType _type;

        public TagType Type
        {
            get { return _type; }
        }

        public NBTScalerNode (string name, TagType type)
            : base(name)
        {
            _type = type;
        }

        public NBTScalerNode (string name, TagType type, NBTOptions options)
            : base(name, options)
        {
            _type = type;
        }

        public override TagValue BuildDefaultTree ()
        {
            switch (_type) {
                case TagType.TAG_STRING:
                    return new TagString();

                case TagType.TAG_BYTE:
                    return new TagByte();

                case TagType.TAG_SHORT:
                    return new TagShort();

                case TagType.TAG_INT:
                    return new TagInt();

                case TagType.TAG_LONG:
                    return new TagLong();

                case TagType.TAG_FLOAT:
                    return new TagFloat();

                case TagType.TAG_DOUBLE:
                    return new TagDouble();
            }

            return null;
        }
    }

    public sealed class NBTStringNode : NBTSchemaNode
    {
        private string _value = "";
        private int _length;

        public int Length
        {
            get { return _length; }
        }

        public string Value
        {
            get { return _value; }
        }

        public NBTStringNode (string name, NBTOptions options)
            : base(name, options)
        {
        }

        public NBTStringNode (string name, string value)
            : base(name)
        {
            _value = value;
        }

        public NBTStringNode (string name, int length)
            : base(name)
        {
            _length = length;
        }

        public override TagValue BuildDefaultTree ()
        {
            if (_value.Length > 0) {
                return new TagString(_value);
            }

            return new TagString();
        }
    }

    public sealed class NBTArrayNode : NBTSchemaNode
    {
        private int _length;

        public int Length
        {
            get { return _length; }
        }

        public NBTArrayNode (string name)
            : base(name)
        {
            _length = 0;
        }

        public NBTArrayNode (string name, NBTOptions options)
            : base(name, options)
        {
            _length = 0;
        }

        public NBTArrayNode (string name, int length)
            : base(name)
        {
            _length = length;
        }

        public NBTArrayNode (string name, int length, NBTOptions options)
            : base(name, options)
        {
            _length = length;
        }

        public override TagValue BuildDefaultTree ()
        {
            return new TagByteArray(new byte[_length]);
        }
    }

    public sealed class NBTListNode : NBTSchemaNode
    {
        private TagType _type;
        private int _length;
        private NBTSchemaNode _subschema;

        public int Length
        {
            get { return _length; }
        }

        public TagType Type
        {
            get { return _type; }
        }

        public NBTSchemaNode SubSchema
        {
            get { return _subschema; }
        }

        public NBTListNode (string name, TagType type)
            : base(name)
        {
            _type = type;
        }

        public NBTListNode (string name, TagType type, NBTOptions options)
            : base(name, options)
        {
            _type = type;
        }

        public NBTListNode (string name, TagType type, int length)
            : base(name)
        {
            _type = type;
            _length = length;
        }

        public NBTListNode (string name, TagType type, int length, NBTOptions options)
            : base(name, options)
        {
            _type = type;
            _length = length;
        }

        public NBTListNode (string name, TagType type, NBTSchemaNode subschema)
            : base(name)
        {
            _type = type;
            _subschema = subschema;
        }

        public NBTListNode (string name, TagType type, NBTSchemaNode subschema, NBTOptions options)
            : base(name, options)
        {
            _type = type;
            _subschema = subschema;
        }

        public NBTListNode (string name, TagType type, int length, NBTSchemaNode subschema)
            : base(name)
        {
            _type = type;
            _length = length;
            _subschema = subschema;
        }

        public NBTListNode (string name, TagType type, int length, NBTSchemaNode subschema, NBTOptions options)
            : base(name, options)
        {
            _type = type;
            _length = length;
            _subschema = subschema;
        }

        public override TagValue BuildDefaultTree ()
        {
            if (_length == 0) {
                return new TagList(_type);
            }

            TagList list = new TagList(_type);
            for (int i = 0; i < _length; i++) {
                list.Add(_subschema.BuildDefaultTree());
            }

            return list;
        }
    }

    public sealed class NBTCompoundNode : NBTSchemaNode, ICollection<NBTSchemaNode>
    {
        private List<NBTSchemaNode> _subnodes;

        #region ICollection<NBTSchemaNode> Members

        public void Add (NBTSchemaNode item)
        {
            _subnodes.Add(item);
        }

        public void Clear ()
        {
            _subnodes.Clear();
        }

        public bool Contains (NBTSchemaNode item)
        {
            return _subnodes.Contains(item);
        }

        public void CopyTo (NBTSchemaNode[] array, int arrayIndex)
        {
            _subnodes.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _subnodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove (NBTSchemaNode item)
        {
            return _subnodes.Remove(item);
        }

        #endregion

        #region IEnumerable<NBTSchemaNode> Members

        public IEnumerator<NBTSchemaNode> GetEnumerator ()
        {
            return _subnodes.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return _subnodes.GetEnumerator();
        }

        #endregion

        public NBTCompoundNode ()
            : base("")
        {
            _subnodes = new List<NBTSchemaNode>();
        }

        public NBTCompoundNode (NBTOptions options)
            : base("", options)
        {
            _subnodes = new List<NBTSchemaNode>();
        }

        public NBTCompoundNode (string name)
            : base(name)
        {
            _subnodes = new List<NBTSchemaNode>();
        }

        public NBTCompoundNode (string name, NBTOptions options)
            : base(name, options)
        {
            _subnodes = new List<NBTSchemaNode>();
        }

        public NBTCompoundNode (string name, NBTSchemaNode subschema)
            : base(name)
        {
            _subnodes = new List<NBTSchemaNode>();

            NBTCompoundNode schema = subschema as NBTCompoundNode;
            if (schema == null) {
                return;
            }

            foreach (NBTSchemaNode node in schema._subnodes)
            {
                _subnodes.Add(node);
            }
        }

        public NBTCompoundNode (string name, NBTSchemaNode subschema, NBTOptions options)
            : base(name, options)
        {
            _subnodes = new List<NBTSchemaNode>();

            NBTCompoundNode schema = subschema as NBTCompoundNode;
            if (schema == null) {
                return;
            }

            foreach (NBTSchemaNode node in schema._subnodes) {
                _subnodes.Add(node);
            }
        }

        public NBTCompoundNode MergeInto (NBTCompoundNode tree)
        {
            foreach (NBTSchemaNode node in _subnodes) {
                NBTSchemaNode f = tree._subnodes.Find(n => n.Name == node.Name);
                if (f != null) {
                    continue;
                }
                tree.Add(node);
            }

            return tree;
        }

        public override TagValue BuildDefaultTree ()
        {
            TagCompound list = new TagCompound();
            foreach (NBTSchemaNode node in _subnodes) {
                list[node.Name] = node.BuildDefaultTree();
            }

            return list;
        }
    }
}
