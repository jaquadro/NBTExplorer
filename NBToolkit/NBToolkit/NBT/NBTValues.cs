using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.NBT {

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

    public abstract class NBT_Value
    {
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

        public static implicit operator NBT_Byte (byte b)
        {
            return new NBT_Byte(b);
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

        public static implicit operator NBT_Short (short s)
        {
            return new NBT_Short(s);
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

        public static implicit operator NBT_Int (int i)
        {
            return new NBT_Int(i);
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

        public static implicit operator NBT_Long (long l)
        {
            return new NBT_Long(l);
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

        public static implicit operator NBT_Float (float f)
        {
            return new NBT_Float(f);
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

        public static implicit operator NBT_Double (double d)
        {
            return new NBT_Double(d);
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

        public static implicit operator NBT_ByteArray (byte[] b)
        {
            return new NBT_ByteArray(b);
        }
    }

    public class NBT_String : NBT_Value
    {
        private string _data = null;

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

        public static implicit operator NBT_String (string s)
        {
            return new NBT_String(s);
        }
    }

    public class NBT_List : NBT_Value
    {
        private NBT_Type _type = NBT_Type.TAG_END;

        private List<NBT_Value> _items = null;

        override public NBT_List ToNBTList () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_LIST; }

        public int Count
        {
            get { return _items.Count; }
        }

        public List<NBT_Value> Items
        {
            get { return _items; }
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

        public void AddItem (NBT_Value val)
        {
            if (_type != val.GetNBTType()) {
                throw new InvalidValueException();
            }

            _items.Add(val);
        }

        public void RemoveItem (int index)
        {
            _items.RemoveAt(index);
        }

        public bool RemoveItem (NBT_Value val)
        {
            return _items.Remove(val);
        }
    }

    public class NBT_Compound : NBT_Value
    {
        private List<NBT_Tag> _tags = null;

        override public NBT_Compound ToNBTCompound () { return this; }
        override public NBT_Type GetNBTType () { return NBT_Type.TAG_COMPOUND; }

        public int Count
        {
            get { return _tags.Count; }
        }

        public List<NBT_Tag> Tags
        {
            get { return _tags; }
        }

        public NBT_Compound ()
        {
            _tags = new List<NBT_Tag>();
        }

        public NBT_Compound (List<NBT_Tag> tags)
        {
            _tags = tags;
        }

        public void AddTag (NBT_Tag sub)
        {
            _tags.Add(sub);
        }

        public void RemoveTag (int index)
        {
            _tags.RemoveAt(index);
        }

        public bool RemoveTag (NBT_Tag sub)
        {
            return _tags.Remove(sub);
        }

        public bool RemoveTag (string name)
        {
            return _tags.Remove(_tags.Find(v => v.Name.Data == name));
        }

        public NBT_Tag FindTagByName (string name)
        {
            foreach (NBT_Tag tag in _tags) {
                if (tag.Name.Data == name) {
                    return tag;
                }
            }

            return null;
        }
    }
}