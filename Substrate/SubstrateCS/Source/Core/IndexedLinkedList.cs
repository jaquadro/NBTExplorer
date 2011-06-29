using System;
using System.Collections;
using System.Collections.Generic;

namespace Substrate.Core
{
    internal class IndexedLinkedList<T> : ICollection<T>, ICollection
    {
        private LinkedList<T> _list;
        private Dictionary<T, LinkedListNode<T>> _index;

        public T First
        {
            get { return _list.First.Value; }
        }

        public T Last
        {
            get { return _list.Last.Value; }
        }

        public IndexedLinkedList ()
        {
            _list = new LinkedList<T>();
            _index = new Dictionary<T, LinkedListNode<T>>();
        }

        public void AddFirst (T value)
        {
            LinkedListNode<T> node = _list.AddFirst(value);
            _index.Add(value, node);
        }

        public void AddLast (T value)
        {
            LinkedListNode<T> node = _list.AddLast(value);
            _index.Add(value, node);
        }

        public void RemoveFirst ()
        {
            _index.Remove(_list.First.Value);
            _list.RemoveFirst();
        }

        public void RemoveLast ()
        {
            _index.Remove(_list.First.Value);
            _list.RemoveLast();
        }

        #region ICollection<T> Members

        public void Add (T item)
        {
            AddLast(item);
        }

        public void Clear ()
        {
            _index.Clear();
            _list.Clear();
        }

        public bool Contains (T item)
        {
            return _index.ContainsKey(item);
        }

        public void CopyTo (T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove (T value)
        {
            LinkedListNode<T> node;
            if (_index.TryGetValue(value, out node))
            {
                _index.Remove(value);
                _list.Remove(node);
                return true;
            }

            return false;
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo (Array array, int index)
        {
            (_list as ICollection).CopyTo(array, index);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return (_list as ICollection).SyncRoot; }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator ()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}
