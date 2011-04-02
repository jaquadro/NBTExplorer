using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map.NBT
{
    public abstract class NBTSchemaNode
    {
        private string _name;

        public string Name
        {
            get { return _name; }
        }

        public NBTSchemaNode (string name)
        {
            _name = name;
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
    }

    public class NBTStringNode : NBTSchemaNode
    {
        private string _value;
        private int _length;

        public int Length
        {
            get { return _length; }
        }

        public string Value
        {
            get { return _value; }
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

        public NBTArrayNode (string name, int length)
            : base(name)
        {
            _length = length;
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

        public NBTListNode (string name, NBT_Type type, int length)
            : base(name)
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

        public NBTListNode (string name, NBT_Type type, int length, NBTSchemaNode subschema)
            : base(name)
        {
            _type = type;
            _length = length;
            _subschema = subschema;
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

        public NBTCompoundNode (string name)
            : base(name)
        {
            _subnodes = new List<NBTSchemaNode>();
        }

        public NBTCompoundNode MergeInto (NBTCompoundNode tree)
        {
            foreach (NBTSchemaNode node in _subnodes) {
                NBTSchemaNode f = tree._subnodes.Find(n => n.Name == node.Name);
                if (f != null) {
                    tree.Remove(f);
                }
                tree.Add(node);
            }

            return tree;
        }
    }
}
