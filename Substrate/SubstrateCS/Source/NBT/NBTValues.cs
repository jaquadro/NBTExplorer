using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.NBT {

    using Substrate.Utility;

    /// <summary>
    /// Describes the type of value held by an NBT_Tag
    /// </summary>
    public enum NBT_Type
    {
        TAG_END = 0,
        TAG_BYTE = 1,   // 8 bits signed
        TAG_SHORT = 2,  // 16 bits signed
        TAG_INT = 3,    // 32 bits signed
        TAG_LONG = 4,   // 64 bits signed
        TAG_FLOAT = 5,
        TAG_DOUBLE = 6,
        TAG_BYTE_ARRAY = 7,
        TAG_STRING = 8,
        TAG_LIST = 9,
        TAG_COMPOUND = 10
    }

    public abstract class NBT_Value : ICopyable<NBT_Value>
    {
        virtual public NBT_Null ToNBTNull () { throw new InvalidCastException(); }
        virtual public NBT_Byte ToNBTByte () { throw new InvalidCastException(); }
        virtual public NBT_Short ToNBTShort () { throw new InvalidCastException(); }
        virtual public NBT_Int ToNBTInt () { throw new InvalidCastException(); }
        virtual public NBT_Long ToNBTLong () { throw new InvalidCastException(); }
        virtual public NBT_Float ToNBTFloat () { throw new InvalidCastException(); }
        virtual public NBT_Double ToNBTDouble () { throw new InvalidCastException(); }
        virtual public NBT_ByteArray ToNBTByteArray () { throw new InvalidCastException(); }
        virtual public NBT_String ToNBTString () { throw new InvalidCastException(); }
        virtual public NBT_List ToNBTList () { throw new InvalidCastException(); }
        virtual public NBT_Compound ToNBTCompound () { throw new InvalidCastException(); }

        virtual public NBT_Type GetNBTType () { return NBT_Type.TAG_END; }

        public virtual NBT_Value Copy ()
        {
            return null;
        }
    }

    public class NBT_Null : NBT_Value
    {
        override public NBT_Null ToNBTNull () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_END; }

        public override NBT_Value Copy ()
        {
            return new NBT_Null();
        }
    }

    public class NBT_Byte : NBT_Value
    {
        private byte _data = 0;

        override public NBT_Byte ToNBTByte () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_BYTE; }

        public byte Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public NBT_Byte () { }

        public NBT_Byte (byte d)
        {
            _data = d;
        }

        public override NBT_Value Copy ()
        {
            return new NBT_Byte(_data);
        }

        public static implicit operator NBT_Byte (byte b)
        {
            return new NBT_Byte(b);
        }

        public static implicit operator byte (NBT_Byte b)
        {
            return b._data;
        }
    }

    public class NBT_Short : NBT_Value
    {
        private short _data = 0;

        override public NBT_Short ToNBTShort () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_SHORT; }

        public short Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public NBT_Short () { }

        public NBT_Short (short d)
        {
            _data = d;
        }

        public override NBT_Value Copy ()
        {
            return new NBT_Short(_data);
        }

        public static implicit operator NBT_Short (short s)
        {
            return new NBT_Short(s);
        }

        public static implicit operator short (NBT_Short s)
        {
            return s._data;
        }
    }

    public class NBT_Int : NBT_Value
    {
        private int _data = 0;

        override public NBT_Int ToNBTInt () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_INT; }

        public int Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public NBT_Int () { }

        public NBT_Int (int d)
        {
            _data = d;
        }

        public override NBT_Value Copy ()
        {
            return new NBT_Int(_data);
        }

        public static implicit operator NBT_Int (int i)
        {
            return new NBT_Int(i);
        }

        public static implicit operator int (NBT_Int i)
        {
            return i._data;
        }
    }

    public class NBT_Long : NBT_Value
    {
        private long _data = 0;

        override public NBT_Long ToNBTLong () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_LONG; }

        public long Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public NBT_Long () { }

        public NBT_Long (long d)
        {
            _data = d;
        }

        public override NBT_Value Copy ()
        {
            return new NBT_Long(_data);
        }

        public static implicit operator NBT_Long (long l)
        {
            return new NBT_Long(l);
        }

        public static implicit operator long (NBT_Long l)
        {
            return l._data;
        }
    }

    public class NBT_Float : NBT_Value
    {
        private float _data = 0;

        override public NBT_Float ToNBTFloat () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_FLOAT; }

        public float Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public NBT_Float () { }

        public NBT_Float (float d)
        {
            _data = d;
        }

        public override NBT_Value Copy ()
        {
            return new NBT_Float(_data);
        }

        public static implicit operator NBT_Float (float f)
        {
            return new NBT_Float(f);
        }

        public static implicit operator float (NBT_Float f)
        {
            return f._data;
        }
    }

    public class NBT_Double : NBT_Value
    {
        private double _data = 0;

        override public NBT_Double ToNBTDouble () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_DOUBLE; }

        public double Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public NBT_Double () { }

        public NBT_Double (double d)
        {
            _data = d;
        }

        public override NBT_Value Copy ()
        {
            return new NBT_Double(_data);
        }

        public static implicit operator NBT_Double (double d)
        {
            return new NBT_Double(d);
        }

        public static implicit operator double (NBT_Double d)
        {
            return d._data;
        }
    }

    public class NBT_ByteArray : NBT_Value
    {
        private byte[] _data = null;

        override public NBT_ByteArray ToNBTByteArray () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_BYTE_ARRAY; }

        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public int Length
        {
            get { return _data.Length; }
        }

        public NBT_ByteArray () { }

        public NBT_ByteArray (byte[] d)
        {
            _data = d;
        }

        public override NBT_Value Copy ()
        {
            byte[] arr = new byte[_data.Length];
            _data.CopyTo(arr, 0);

            return new NBT_ByteArray(arr);
        }

        public byte this [int index] {
            get { return _data[index]; }
            set { _data[index] = value; }
        }

        public static implicit operator NBT_ByteArray (byte[] b)
        {
            return new NBT_ByteArray(b);
        }
    }

    public class NBT_String : NBT_Value
    {
        private string _data = "";

        override public NBT_String ToNBTString () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_STRING; }

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public int Length
        {
            get { return _data.Length; }
        }

        public NBT_String () { }

        public NBT_String (string d)
        {
            _data = d;
        }

        public override NBT_Value Copy ()
        {
            return new NBT_String(_data);
        }

        public static implicit operator NBT_String (string s)
        {
            return new NBT_String(s);
        }

        public static implicit operator string (NBT_String s)
        {
            return s._data;
        }
    }

    public class NBT_List : NBT_Value, IList<NBT_Value>
    {
        private NBT_Type _type = NBT_Type.TAG_END;

        private List<NBT_Value> _items = null;

        override public NBT_List ToNBTList () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_LIST; }

        public int Count
        {
            get { return _items.Count; }
        }

        public NBT_Type ValueType
        {
            get { return _type; }
        }

        public NBT_List (NBT_Type type)
        {
            _type = type;
            _items = new List<NBT_Value>();
        }

        public NBT_List (NBT_Type type, List<NBT_Value> items)
        {
            _type = type;
            _items = items;
        }

        public override NBT_Value Copy ()
        {
            NBT_List list = new NBT_List(_type);
            foreach (NBT_Value item in _items) {
                list.Add(item.Copy());
            }
            return list;
        }

        public List<NBT_Value> FindAll(Predicate<NBT_Value> match)
        {
            return _items.FindAll(match);
        }

        public int RemoveAll (Predicate<NBT_Value> match)
        {
            return _items.RemoveAll(match);
        }

        #region IList<NBT_Value> Members

        public int IndexOf (NBT_Value item)
        {
            return _items.IndexOf(item);
        }

        public void Insert (int index, NBT_Value item)
        {
            _items.Insert(index, item);
        }

        public void RemoveAt (int index)
        {
            _items.RemoveAt(index);
        }

        public NBT_Value this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
            }
        }

        #endregion

        #region ICollection<NBT_Value> Members

        public void Add (NBT_Value item)
        {
            _items.Add(item);
        }

        public void Clear ()
        {
            _items.Clear();
        }

        public bool Contains (NBT_Value item)
        {
            return _items.Contains(item);
        }

        public void CopyTo (NBT_Value[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove (NBT_Value item)
        {
            return _items.Remove(item);
        }

        #endregion

        #region IEnumerable<NBT_Value> Members

        public IEnumerator<NBT_Value> GetEnumerator ()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return _items.GetEnumerator();
        }

        #endregion
    }

    public class NBT_Compound : NBT_Value, IDictionary<string, NBT_Value>
    {
        private Dictionary<string, NBT_Value> _tags;

        override public NBT_Compound ToNBTCompound () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_COMPOUND; }

        public int Count
        {
            get { return _tags.Count; }
        }

        public NBT_Compound ()
        {
            _tags = new Dictionary<string, NBT_Value>();
        }

        public override NBT_Value Copy ()
        {
            NBT_Compound list = new NBT_Compound();
            foreach (KeyValuePair<string, NBT_Value> item in _tags) {
                list[item.Key] = item.Value.Copy();
            }
            return list;
        }

        #region IDictionary<string,NBT_Value> Members

        public void Add (string key, NBT_Value value)
        {
            _tags.Add(key, value);
        }

        public bool ContainsKey (string key)
        {
            return _tags.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _tags.Keys; }
        }

        public bool Remove (string key)
        {
            return _tags.Remove(key);
        }

        public bool TryGetValue (string key, out NBT_Value value)
        {
            return _tags.TryGetValue(key, out value);
        }

        public ICollection<NBT_Value> Values
        {
            get { return _tags.Values; }
        }

        public NBT_Value this[string key]
        {
            get
            {
                return _tags[key];
            }
            set
            {
                _tags[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,NBT_Value>> Members

        public void Add (KeyValuePair<string, NBT_Value> item)
        {
            _tags.Add(item.Key, item.Value);
        }

        public void Clear ()
        {
            _tags.Clear();
        }

        public bool Contains (KeyValuePair<string, NBT_Value> item)
        {
            NBT_Value value;
            if (!_tags.TryGetValue(item.Key, out value)) {
                return false;
            }
            return value == item.Value;
        }

        public void CopyTo (KeyValuePair<string, NBT_Value>[] array, int arrayIndex)
        {
            if (array == null) {
                throw new ArgumentNullException();
            }
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException();
            }
            if (array.Length - arrayIndex < _tags.Count) {
                throw new ArgumentException();
            }

            foreach (KeyValuePair<string, NBT_Value> item in _tags) {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove (KeyValuePair<string, NBT_Value> item)
        {
            if (Contains(item)) {
                _tags.Remove(item.Key);
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,NBT_Value>> Members

        public IEnumerator<KeyValuePair<string, NBT_Value>> GetEnumerator ()
        {
            return _tags.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return _tags.GetEnumerator();
        }

        #endregion
    }
}