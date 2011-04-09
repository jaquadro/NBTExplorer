using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.NBT {

    using Substrate.Utility;

    public enum TagType
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

    public abstract class TagValue : ICopyable<TagValue>
    {
        public virtual TagNull ToTagNull () { throw new InvalidCastException(); }
        public virtual TagByte ToTagByte () { throw new InvalidCastException(); }
        public virtual TagShort ToTagShort () { throw new InvalidCastException(); }
        public virtual TagInt ToTagInt () { throw new InvalidCastException(); }
        public virtual TagLong ToTagLong () { throw new InvalidCastException(); }
        public virtual TagFloat ToTagFloat () { throw new InvalidCastException(); }
        public virtual TagDouble ToTagDouble () { throw new InvalidCastException(); }
        public virtual TagByteArray ToTagByteArray () { throw new InvalidCastException(); }
        public virtual TagString ToTagString () { throw new InvalidCastException(); }
        public virtual TagList ToTagList () { throw new InvalidCastException(); }
        public virtual TagCompound ToTagCompound () { throw new InvalidCastException(); }

        public virtual TagType GetTagType () { return TagType.TAG_END; }

        public virtual bool IsCastableTo (TagType type)
        {
            return type == GetTagType();
        }

        public virtual TagValue Copy ()
        {
            return null;
        }
    }

    public class TagNull : TagValue
    {
        public override TagNull ToTagNull () { return this; }
        public override TagType GetTagType () { return TagType.TAG_END; }

        public override TagValue Copy ()
        {
            return new TagNull();
        }
    }

    public class TagByte : TagValue
    {
        private byte _data = 0;

        public override TagByte ToTagByte () { return this; }
        public override TagShort ToTagShort () { return new TagShort(_data); }
        public override TagInt ToTagInt () { return new TagInt(_data); }
        public override TagLong ToTagLong () { return new TagLong(_data); }
 
        public override TagType GetTagType () { return TagType.TAG_BYTE; }

        public override bool IsCastableTo (TagType type)
        {
            return (type == TagType.TAG_BYTE ||
                type == TagType.TAG_SHORT ||
                type == TagType.TAG_INT ||
                type == TagType.TAG_LONG);
        }

        public byte Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public TagByte () { }

        public TagByte (byte d)
        {
            _data = d;
        }

        public override TagValue Copy ()
        {
            return new TagByte(_data);
        }

        public override string ToString ()
        {
            return _data.ToString();
        }

        public static implicit operator TagByte (byte b)
        {
            return new TagByte(b);
        }

        public static implicit operator byte (TagByte b)
        {
            return b._data;
        }

        public static implicit operator short (TagByte b)
        {
            return b._data;
        }

        public static implicit operator int (TagByte b)
        {
            return b._data;
        }

        public static implicit operator long (TagByte b)
        {
            return b._data;
        }
    }

    public class TagShort : TagValue
    {
        private short _data = 0;

        public override TagShort ToTagShort () { return this; }
        public override TagInt ToTagInt () { return new TagInt(_data); }
        public override TagLong ToTagLong () { return new TagLong(_data); }

        public override TagType GetTagType () { return TagType.TAG_SHORT; }

        public override bool IsCastableTo (TagType type)
        {
            return (type == TagType.TAG_SHORT ||
                type == TagType.TAG_INT ||
                type == TagType.TAG_LONG);
        }

        public short Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public TagShort () { }

        public TagShort (short d)
        {
            _data = d;
        }

        public override TagValue Copy ()
        {
            return new TagShort(_data);
        }

        public override string ToString ()
        {
            return _data.ToString();
        }

        public static implicit operator TagShort (byte b)
        {
            return new TagShort(b);
        }

        public static implicit operator TagShort (short s)
        {
            return new TagShort(s);
        }

        public static implicit operator short (TagShort s)
        {
            return s._data;
        }

        public static implicit operator int (TagShort s)
        {
            return s._data;
        }

        public static implicit operator long (TagShort s)
        {
            return s._data;
        }
    }

    public class TagInt : TagValue
    {
        private int _data = 0;

        public override TagInt ToTagInt () { return this; }
        public override TagLong ToTagLong () { return new TagLong(_data); }

        public override TagType GetTagType () { return TagType.TAG_INT; }

        public override bool IsCastableTo (TagType type)
        {
            return (type == TagType.TAG_INT ||
                type == TagType.TAG_LONG);
        }

        public int Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public TagInt () { }

        public TagInt (int d)
        {
            _data = d;
        }

        public override TagValue Copy ()
        {
            return new TagInt(_data);
        }

        public override string ToString ()
        {
            return _data.ToString();
        }

        public static implicit operator TagInt (byte b)
        {
            return new TagInt(b);
        }

        public static implicit operator TagInt (short s)
        {
            return new TagInt(s);
        }

        public static implicit operator TagInt (int i)
        {
            return new TagInt(i);
        }

        public static implicit operator int (TagInt i)
        {
            return i._data;
        }

        public static implicit operator long (TagInt i)
        {
            return i._data;
        }
    }

    public class TagLong : TagValue
    {
        private long _data = 0;

        public override TagLong ToTagLong () { return this; }

        public override TagType GetTagType () { return TagType.TAG_LONG; }

        public long Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public TagLong () { }

        public TagLong (long d)
        {
            _data = d;
        }

        public override TagValue Copy ()
        {
            return new TagLong(_data);
        }

        public override string ToString ()
        {
            return _data.ToString();
        }

        public static implicit operator TagLong (byte b)
        {
            return new TagLong(b);
        }

        public static implicit operator TagLong (short s)
        {
            return new TagLong(s);
        }

        public static implicit operator TagLong (int i)
        {
            return new TagLong(i);
        }

        public static implicit operator TagLong (long l)
        {
            return new TagLong(l);
        }

        public static implicit operator long (TagLong l)
        {
            return l._data;
        }
    }

    public class TagFloat : TagValue
    {
        private float _data = 0;

        public override TagFloat ToTagFloat () { return this; }
        public override TagDouble ToTagDouble () { return new TagDouble(_data); }

        public override TagType GetTagType () { return TagType.TAG_FLOAT; }

        public override bool IsCastableTo (TagType type)
        {
            return (type == TagType.TAG_FLOAT ||
                type == TagType.TAG_DOUBLE);
        }

        public float Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public TagFloat () { }

        public TagFloat (float d)
        {
            _data = d;
        }

        public override TagValue Copy ()
        {
            return new TagFloat(_data);
        }

        public override string ToString ()
        {
            return _data.ToString();
        }

        public static implicit operator TagFloat (float f)
        {
            return new TagFloat(f);
        }

        public static implicit operator float (TagFloat f)
        {
            return f._data;
        }

        public static implicit operator double (TagFloat f)
        {
            return f._data;
        }
    }

    public class TagDouble : TagValue
    {
        private double _data = 0;

        public override TagDouble ToTagDouble () { return this; }

        public override TagType GetTagType () { return TagType.TAG_DOUBLE; }

        public double Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public TagDouble () { }

        public TagDouble (double d)
        {
            _data = d;
        }

        public override TagValue Copy ()
        {
            return new TagDouble(_data);
        }

        public override string ToString ()
        {
            return _data.ToString();
        }

        public static implicit operator TagDouble (float f)
        {
            return new TagDouble(f);
        }

        public static implicit operator TagDouble (double d)
        {
            return new TagDouble(d);
        }

        public static implicit operator double (TagDouble d)
        {
            return d._data;
        }
    }

    public class TagByteArray : TagValue
    {
        private byte[] _data = null;

        public override TagByteArray ToTagByteArray () { return this; }

        public override TagType GetTagType () { return TagType.TAG_BYTE_ARRAY; }

        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public int Length
        {
            get { return _data.Length; }
        }

        public TagByteArray () { }

        public TagByteArray (byte[] d)
        {
            _data = d;
        }

        public override TagValue Copy ()
        {
            byte[] arr = new byte[_data.Length];
            _data.CopyTo(arr, 0);

            return new TagByteArray(arr);
        }

        public override string ToString ()
        {
            return _data.ToString();
        }

        public byte this [int index] {
            get { return _data[index]; }
            set { _data[index] = value; }
        }

        public static implicit operator TagByteArray (byte[] b)
        {
            return new TagByteArray(b);
        }

        public static implicit operator byte[] (TagByteArray b)
        {
            return b._data;
        }
    }

    public class TagString : TagValue
    {
        private string _data = "";

        public override TagString ToTagString () { return this; }

        public override TagType GetTagType () { return TagType.TAG_STRING; }

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public int Length
        {
            get { return _data.Length; }
        }

        public TagString () { }

        public TagString (string d)
        {
            _data = d;
        }

        public override TagValue Copy ()
        {
            return new TagString(_data);
        }

        public override string ToString ()
        {
            return _data.ToString();
        }

        public static implicit operator TagString (string s)
        {
            return new TagString(s);
        }

        public static implicit operator string (TagString s)
        {
            return s._data;
        }
    }

    public class TagList : TagValue, IList<TagValue>
    {
        private TagType _type = TagType.TAG_END;

        private List<TagValue> _items = null;

        public override TagList ToTagList () { return this; }

        public override TagType GetTagType () { return TagType.TAG_LIST; }

        public int Count
        {
            get { return _items.Count; }
        }

        public TagType ValueType
        {
            get { return _type; }
        }

        public TagList (TagType type)
        {
            _type = type;
            _items = new List<TagValue>();
        }

        public TagList (TagType type, List<TagValue> items)
        {
            _type = type;
            _items = items;
        }

        public override TagValue Copy ()
        {
            TagList list = new TagList(_type);
            foreach (TagValue item in _items) {
                list.Add(item.Copy());
            }
            return list;
        }

        public List<TagValue> FindAll(Predicate<TagValue> match)
        {
            return _items.FindAll(match);
        }

        public int RemoveAll (Predicate<TagValue> match)
        {
            return _items.RemoveAll(match);
        }

        public override string ToString ()
        {
            return _items.ToString();
        }

        #region IList<NBT_Value> Members

        public int IndexOf (TagValue item)
        {
            return _items.IndexOf(item);
        }

        public void Insert (int index, TagValue item)
        {
            _items.Insert(index, item);
        }

        public void RemoveAt (int index)
        {
            _items.RemoveAt(index);
        }

        public TagValue this[int index]
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

        public void Add (TagValue item)
        {
            _items.Add(item);
        }

        public void Clear ()
        {
            _items.Clear();
        }

        public bool Contains (TagValue item)
        {
            return _items.Contains(item);
        }

        public void CopyTo (TagValue[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove (TagValue item)
        {
            return _items.Remove(item);
        }

        #endregion

        #region IEnumerable<NBT_Value> Members

        public IEnumerator<TagValue> GetEnumerator ()
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

    public class TagCompound : TagValue, IDictionary<string, TagValue>
    {
        private Dictionary<string, TagValue> _tags;

        public override TagCompound ToTagCompound () { return this; }

        public override TagType GetTagType () { return TagType.TAG_COMPOUND; }

        public int Count
        {
            get { return _tags.Count; }
        }

        public TagCompound ()
        {
            _tags = new Dictionary<string, TagValue>();
        }

        public override TagValue Copy ()
        {
            TagCompound list = new TagCompound();
            foreach (KeyValuePair<string, TagValue> item in _tags) {
                list[item.Key] = item.Value.Copy();
            }
            return list;
        }

        public override string ToString ()
        {
            return _tags.ToString();
        }

        #region IDictionary<string,NBT_Value> Members

        public void Add (string key, TagValue value)
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

        public bool TryGetValue (string key, out TagValue value)
        {
            return _tags.TryGetValue(key, out value);
        }

        public ICollection<TagValue> Values
        {
            get { return _tags.Values; }
        }

        public TagValue this[string key]
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

        public void Add (KeyValuePair<string, TagValue> item)
        {
            _tags.Add(item.Key, item.Value);
        }

        public void Clear ()
        {
            _tags.Clear();
        }

        public bool Contains (KeyValuePair<string, TagValue> item)
        {
            TagValue value;
            if (!_tags.TryGetValue(item.Key, out value)) {
                return false;
            }
            return value == item.Value;
        }

        public void CopyTo (KeyValuePair<string, TagValue>[] array, int arrayIndex)
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

            foreach (KeyValuePair<string, TagValue> item in _tags) {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove (KeyValuePair<string, TagValue> item)
        {
            if (Contains(item)) {
                _tags.Remove(item.Key);
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,NBT_Value>> Members

        public IEnumerator<KeyValuePair<string, TagValue>> GetEnumerator ()
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