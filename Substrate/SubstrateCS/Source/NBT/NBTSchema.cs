using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.NBT
{
    [Flags]
    public enum SchemaOptions
    {
        OPTIONAL = 0x1,
        CREATE_ON_MISSING = 0x2,
    }

    public abstract class SchemaNode
    {
        private string _name;
        private SchemaOptions _options;

        public string Name
        {
            get { return _name; }
        }

        public SchemaOptions Options
        {
            get { return _options; }
        }

        public SchemaNode (string name)
        {
            _name = name;
        }

        public SchemaNode (string name, SchemaOptions options)
        {
            _name = name;
            _options = options;
        }

        public virtual TagNode BuildDefaultTree ()
        {
            return null;
        }
    }

    public sealed class SchemaNodeScaler : SchemaNode
    {
        private TagType _type;

        public TagType Type
        {
            get { return _type; }
        }

        public SchemaNodeScaler (string name, TagType type)
            : base(name)
        {
            _type = type;
        }

        public SchemaNodeScaler (string name, TagType type, SchemaOptions options)
            : base(name, options)
        {
            _type = type;
        }

        public override TagNode BuildDefaultTree ()
        {
            switch (_type) {
                case TagType.TAG_STRING:
                    return new TagNodeString();

                case TagType.TAG_BYTE:
                    return new TagNodeByte();

                case TagType.TAG_SHORT:
                    return new TagNodeShort();

                case TagType.TAG_INT:
                    return new TagNodeInt();

                case TagType.TAG_LONG:
                    return new TagNodeLong();

                case TagType.TAG_FLOAT:
                    return new TagNodeFloat();

                case TagType.TAG_DOUBLE:
                    return new TagNodeDouble();
            }

            return null;
        }
    }

    public sealed class SchemaNodeString : SchemaNode
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

        public SchemaNodeString (string name, SchemaOptions options)
            : base(name, options)
        {
        }

        public SchemaNodeString (string name, string value)
            : base(name)
        {
            _value = value;
        }

        public SchemaNodeString (string name, int length)
            : base(name)
        {
            _length = length;
        }

        public override TagNode BuildDefaultTree ()
        {
            if (_value.Length > 0) {
                return new TagNodeString(_value);
            }

            return new TagNodeString();
        }
    }

    public sealed class SchemaNodeArray : SchemaNode
    {
        private int _length;

        public int Length
        {
            get { return _length; }
        }

        public SchemaNodeArray (string name)
            : base(name)
        {
            _length = 0;
        }

        public SchemaNodeArray (string name, SchemaOptions options)
            : base(name, options)
        {
            _length = 0;
        }

        public SchemaNodeArray (string name, int length)
            : base(name)
        {
            _length = length;
        }

        public SchemaNodeArray (string name, int length, SchemaOptions options)
            : base(name, options)
        {
            _length = length;
        }

        public override TagNode BuildDefaultTree ()
        {
            return new TagNodeByteArray(new byte[_length]);
        }
    }

    public sealed class SchemaNodeList : SchemaNode
    {
        private TagType _type;
        private int _length;
        private SchemaNode _subschema;

        public int Length
        {
            get { return _length; }
        }

        public TagType Type
        {
            get { return _type; }
        }

        public SchemaNode SubSchema
        {
            get { return _subschema; }
        }

        public SchemaNodeList (string name, TagType type)
            : base(name)
        {
            _type = type;
        }

        public SchemaNodeList (string name, TagType type, SchemaOptions options)
            : base(name, options)
        {
            _type = type;
        }

        public SchemaNodeList (string name, TagType type, int length)
            : base(name)
        {
            _type = type;
            _length = length;
        }

        public SchemaNodeList (string name, TagType type, int length, SchemaOptions options)
            : base(name, options)
        {
            _type = type;
            _length = length;
        }

        public SchemaNodeList (string name, TagType type, SchemaNode subschema)
            : base(name)
        {
            _type = type;
            _subschema = subschema;
        }

        public SchemaNodeList (string name, TagType type, SchemaNode subschema, SchemaOptions options)
            : base(name, options)
        {
            _type = type;
            _subschema = subschema;
        }

        public SchemaNodeList (string name, TagType type, int length, SchemaNode subschema)
            : base(name)
        {
            _type = type;
            _length = length;
            _subschema = subschema;
        }

        public SchemaNodeList (string name, TagType type, int length, SchemaNode subschema, SchemaOptions options)
            : base(name, options)
        {
            _type = type;
            _length = length;
            _subschema = subschema;
        }

        public override TagNode BuildDefaultTree ()
        {
            if (_length == 0) {
                return new TagNodeList(_type);
            }

            TagNodeList list = new TagNodeList(_type);
            for (int i = 0; i < _length; i++) {
                list.Add(_subschema.BuildDefaultTree());
            }

            return list;
        }
    }

    public sealed class SchemaNodeCompound : SchemaNode, ICollection<SchemaNode>
    {
        private List<SchemaNode> _subnodes;

        #region ICollection<NBTSchemaNode> Members

        public void Add (SchemaNode item)
        {
            _subnodes.Add(item);
        }

        public void Clear ()
        {
            _subnodes.Clear();
        }

        public bool Contains (SchemaNode item)
        {
            return _subnodes.Contains(item);
        }

        public void CopyTo (SchemaNode[] array, int arrayIndex)
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

        public bool Remove (SchemaNode item)
        {
            return _subnodes.Remove(item);
        }

        #endregion

        #region IEnumerable<NBTSchemaNode> Members

        public IEnumerator<SchemaNode> GetEnumerator ()
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

        public SchemaNodeCompound ()
            : base("")
        {
            _subnodes = new List<SchemaNode>();
        }

        public SchemaNodeCompound (SchemaOptions options)
            : base("", options)
        {
            _subnodes = new List<SchemaNode>();
        }

        public SchemaNodeCompound (string name)
            : base(name)
        {
            _subnodes = new List<SchemaNode>();
        }

        public SchemaNodeCompound (string name, SchemaOptions options)
            : base(name, options)
        {
            _subnodes = new List<SchemaNode>();
        }

        public SchemaNodeCompound (string name, SchemaNode subschema)
            : base(name)
        {
            _subnodes = new List<SchemaNode>();

            SchemaNodeCompound schema = subschema as SchemaNodeCompound;
            if (schema == null) {
                return;
            }

            foreach (SchemaNode node in schema._subnodes)
            {
                _subnodes.Add(node);
            }
        }

        public SchemaNodeCompound (string name, SchemaNode subschema, SchemaOptions options)
            : base(name, options)
        {
            _subnodes = new List<SchemaNode>();

            SchemaNodeCompound schema = subschema as SchemaNodeCompound;
            if (schema == null) {
                return;
            }

            foreach (SchemaNode node in schema._subnodes) {
                _subnodes.Add(node);
            }
        }

        public SchemaNodeCompound MergeInto (SchemaNodeCompound tree)
        {
            foreach (SchemaNode node in _subnodes) {
                SchemaNode f = tree._subnodes.Find(n => n.Name == node.Name);
                if (f != null) {
                    continue;
                }
                tree.Add(node);
            }

            return tree;
        }

        public override TagNode BuildDefaultTree ()
        {
            TagNodeCompound list = new TagNodeCompound();
            foreach (SchemaNode node in _subnodes) {
                list[node.Name] = node.BuildDefaultTree();
            }

            return list;
        }
    }
}
