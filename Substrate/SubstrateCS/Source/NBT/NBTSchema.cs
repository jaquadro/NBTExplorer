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

        public virtual NBT_Value BuildDefaultTree ()
        {
            return null;
        }
    }

    public class NBTScalerNode : NBTSchemaNode
    {
        private NBT_Type _type;

        public NBT_Type Type
        {
            get { return _type; }
        }

        public NBTScalerNode (string name, NBT_Type type)
            : base(name)
        {
            _type = type;
        }

        public NBTScalerNode (string name, NBT_Type type, NBTOptions options)
            : base(name, options)
        {
            _type = type;
        }

        public override NBT_Value BuildDefaultTree ()
        {
            switch (_type) {
                case NBT_Type.TAG_STRING:
                    return new NBT_String();

                case NBT_Type.TAG_BYTE:
                    return new NBT_Byte();

                case NBT_Type.TAG_SHORT:
                    return new NBT_Short();

                case NBT_Type.TAG_INT:
                    return new NBT_Int();

                case NBT_Type.TAG_LONG:
                    return new NBT_Long();

                case NBT_Type.TAG_FLOAT:
                    return new NBT_Float();

                case NBT_Type.TAG_DOUBLE:
                    return new NBT_Double();
            }

            return null;
        }
    }

    public class NBTStringNode : NBTSchemaNode
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

        public override NBT_Value BuildDefaultTree ()
        {
            if (_value.Length > 0) {
                return new NBT_String(_value);
            }

            return new NBT_String();
        }
    }

    public class NBTArrayNode : NBTSchemaNode
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

        public override NBT_Value BuildDefaultTree ()
        {
            return new NBT_ByteArray(new byte[_length]);
        }
    }

    public class NBTListNode : NBTSchemaNode
    {
        private NBT_Type _type;
        private int _length;
        private NBTSchemaNode _subschema;

        public int Length
        {
            get { return _length; }
        }

        public NBT_Type Type
        {
            get { return _type; }
        }

        public NBTSchemaNode SubSchema
        {
            get { return _subschema; }
        }

        public NBTListNode (string name, NBT_Type type)
            : base(name)
        {
            _type = type;
        }

        public NBTListNode (string name, NBT_Type type, NBTOptions options)
            : base(name, options)
        {
            _type = type;
        }

        public NBTListNode (string name, NBT_Type type, int length)
            : base(name)
        {
            _type = type;
            _length = length;
        }

        public NBTListNode (string name, NBT_Type type, int length, NBTOptions options)
            : base(name, options)
        {
            _type = type;
            _length = length;
        }

        public NBTListNode (string name, NBT_Type type, NBTSchemaNode subschema)
            : base(name)
        {
            _type = type;
            _subschema = subschema;
        }

        public NBTListNode (string name, NBT_Type type, NBTSchemaNode subschema, NBTOptions options)
            : base(name, options)
        {
            _type = type;
            _subschema = subschema;
        }

        public NBTListNode (string name, NBT_Type type, int length, NBTSchemaNode subschema)
            : base(name)
        {
            _type = type;
            _length = length;
            _subschema = subschema;
        }

        public NBTListNode (string name, NBT_Type type, int length, NBTSchemaNode subschema, NBTOptions options)
            : base(name, options)
        {
            _type = type;
            _length = length;
            _subschema = subschema;
        }

        public override NBT_Value BuildDefaultTree ()
        {
            if (_length == 0) {
                return new NBT_List(_type);
            }

            NBT_List list = new NBT_List(_type);
            for (int i = 0; i < _length; i++) {
                list.Add(_subschema.BuildDefaultTree());
            }

            return list;
        }
    }

    public class NBTCompoundNode : NBTSchemaNode, ICollection<NBTSchemaNode>
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

        public override NBT_Value BuildDefaultTree ()
        {
            NBT_Compound list = new NBT_Compound();
            foreach (NBTSchemaNode node in _subnodes) {
                list[node.Name] = node.BuildDefaultTree();
            }

            return list;
        }
    }
}
