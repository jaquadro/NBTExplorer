using System;
using System.Collections.Generic;

namespace NBTExplorer.Model
{
    public class DataNodeCollection : IList<DataNode>
    {
        private List<DataNode> _nodes;
        private DataNode _parent;

        internal DataNodeCollection (DataNode parent)
        {
            _parent = parent;
            _nodes = new List<DataNode>();
        }

        public int IndexOf (DataNode item)
        {
            return _nodes.IndexOf(item);
        }

        public void Insert (int index, DataNode item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Parent != null)
                throw new ArgumentException("The item is already assigned to another DataNode.");

            item.Parent = _parent;

            _nodes.Insert(index, item);
        }

        public void RemoveAt (int index)
        {
            if (index < 0 || index >= _nodes.Count)
                throw new ArgumentOutOfRangeException("index");

            DataNode node = _nodes[index];
            node.Parent = null;

            _nodes.RemoveAt(index);
        }

        DataNode IList<DataNode>.this[int index]
        {
            get { return _nodes[index]; }
            set
            {
                if (index < 0 || index > _nodes.Count)
                    throw new ArgumentOutOfRangeException("index");
                if (value == null)
                    throw new ArgumentNullException("item");
                if (value.Parent != null)
                    throw new ArgumentException("The item is already assigned to another DataNode.");

                _nodes[index].Parent = null;
                _nodes[index] = value;
                _nodes[index].Parent = _parent;
            }
        }

        public void Add (DataNode item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Parent != null)
                throw new ArgumentException("The item is already assigned to another DataNode.");

            item.Parent = _parent;

            _nodes.Add(item);
        }

        public void Clear ()
        {
            foreach (DataNode node in _nodes)
                node.Parent = null;

            _nodes.Clear();
        }

        public bool Contains (DataNode item)
        {
            return _nodes.Contains(item);
        }

        public void CopyTo (DataNode[] array, int arrayIndex)
        {
            _nodes.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _nodes.Count; }
        }

        bool ICollection<DataNode>.IsReadOnly
        {
            get { return (_nodes as IList<DataNode>).IsReadOnly; }
        }

        public bool Remove (DataNode item)
        {
            if (_nodes.Contains(item))
                item.Parent = null;

            return _nodes.Remove(item);
        }

        public IEnumerator<DataNode> GetEnumerator ()
        {
            return _nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return _nodes.GetEnumerator();
        }
    }
}
