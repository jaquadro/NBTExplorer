using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NBTExplorer.Utility
{
    public class SnapshotState<T> : IDisposable
    {
        private SnapshotList<T> _list;

        internal SnapshotState (SnapshotList<T> list)
        {
            _list = list;
            _list.Begin();
        }

        public void Dispose ()
        {
            _list.End();
            GC.SuppressFinalize(this);
        }
    }

    public class SnapshotList<T> : Collection<T>
    {
        private IList<T> _snapshot;
        private IList<T> _recycled;
        private int _snapshots;

        public SnapshotList ()
            : base(new ProxyList<T>())
        { }

        public SnapshotList (IList<T> list)
            : base(new ProxyList<T>(new List<T>(list)))
        { }

        public SnapshotList (int capacity)
            : base(new ProxyList<T>(new List<T>(capacity)))
        { }

        private new ProxyList<T> Items
        {
            get { return base.Items as ProxyList<T>; }
        }

        public IList<T> Begin ()
        {
            Modified();
            _snapshot = Items.InnerList;
            _snapshots++;
            return _snapshot;
        }

        public void End ()
        {
            _snapshots = Math.Max(0, _snapshots - 1);
            if (_snapshot == null)
                return;

            // The backing array was copied, keep around the old array
            if (_snapshot != Items.InnerList && _snapshots == 0) {
                _recycled = _snapshot;
                _recycled.Clear();
                //for (int i = 0, n = _recycled.Count; i < n; i++)
                //    _recycled[i] = default(T);
            }

            _snapshot = null;
        }

        public SnapshotState<T> Snapshot ()
        {
            return new SnapshotState<T>(this);
        }

        private void Modified ()
        {
            if (_snapshot == null || _snapshot != Items.InnerList)
                return;

            // Snapshot is in use, copy backing array to recycled array or create new backing array
            if (_recycled != null) {
                for (int i = 0; i < Count; i++)
                    _recycled.Add(Items[i]);
                Items.InnerList = _recycled;
                _recycled = null;
            }
            else
                Resize(Items.Count);
        }

        private void Resize (int newSize)
        {
            IList<T> oldList = Items.InnerList;
            List<T> newList = new List<T>(newSize);
            for (int i = 0, n = oldList.Count; i < n; i++)
                newList.Add(oldList[i]);

            Items.InnerList = newList;
        }

        protected override void InsertItem (int index, T item)
        {
            Modified();
            base.InsertItem(index, item);
        }

        protected override void SetItem (int index, T item)
        {
            Modified();
            base.SetItem(index, item);
        }

        protected override void RemoveItem (int index)
        {
            Modified();
            base.RemoveItem(index);
        }

        protected override void ClearItems ()
        {
            Modified();
            base.ClearItems();
        }

        private class ProxyList<K> : IList<K>
        {
            public IList<K> InnerList { get; set; }

            public ProxyList ()
            {
                InnerList = new List<K>();
            }

            public ProxyList (IList<K> list)
            {
                InnerList = list;
            }

            public int IndexOf (K item)
            {
                return InnerList.IndexOf(item);
            }

            public void Insert (int index, K item)
            {
                InnerList.Insert(index, item);
            }

            public void RemoveAt (int index)
            {
                InnerList.RemoveAt(index);
            }

            public K this[int index]
            {
                get { return InnerList[index]; }
                set { InnerList[index] = value; }
            }

            public void Add (K item)
            {
                InnerList.Add(item);
            }

            public void Clear ()
            {
                InnerList.Clear();
            }

            public bool Contains (K item)
            {
                return InnerList.Contains(item);
            }

            public void CopyTo (K[] array, int arrayIndex)
            {
                InnerList.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return InnerList.Count; }
            }

            public bool IsReadOnly
            {
                get { return InnerList.IsReadOnly; }
            }

            public bool Remove (K item)
            {
                return InnerList.Remove(item);
            }

            public IEnumerator<K> GetEnumerator ()
            {
                return InnerList.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator ()
            {
                return InnerList.GetEnumerator();
            }
        }
    }
}
