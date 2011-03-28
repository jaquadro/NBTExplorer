using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace NBT
{
    public enum NBT_Type
    {
        TAG_END = 0,
        TAG_BYTE = 1,   // 8 bits signed
        TAG_SHORT = 2,  // 16 bits signed
        TAG_INT = 3,    // 32 buts signed
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
        virtual public NBT_Byte toByte () { throw new InvalidCastException(); }
        virtual public NBT_Short toShort () { throw new InvalidCastException(); }
        virtual public NBT_Int toInt () { throw new InvalidCastException(); }
        virtual public NBT_Long toLong () { throw new InvalidCastException(); }
        virtual public NBT_Float toFloat () { throw new InvalidCastException(); }
        virtual public NBT_Double toDouble () { throw new InvalidCastException(); }
        virtual public NBT_ByteArray toByteArray () { throw new InvalidCastException(); }
        virtual public NBT_String toString () { throw new InvalidCastException(); }
        virtual public NBT_List toList () { throw new InvalidCastException(); }
        virtual public NBT_Compound toCompound () { throw new InvalidCastException(); }

        virtual public NBT_Type getType () { return NBT_Type.TAG_END; }
    }

    public abstract class NBT_NumericValue : NBT_Value
    {

    }

    public class NBT_Byte : NBT_Value
    {
        protected byte _data = 0;

        override public NBT_Byte toByte () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_BYTE; }

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
    }

    public class NBT_Short : NBT_Value
    {
        protected short _data = 0;

        override public NBT_Short toShort () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_SHORT; }

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
    }

    public class NBT_Int : NBT_Value
    {
        protected int _data = 0;

        override public NBT_Int toInt () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_INT; }

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
    }

    public class NBT_Long : NBT_Value
    {
        protected long _data = 0;

        override public NBT_Long toLong () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_LONG; }

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
    }

    public class NBT_Float : NBT_Value
    {
        protected float _data = 0;

        override public NBT_Float toFloat () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_FLOAT; }

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
    }

    public class NBT_Double : NBT_Value
    {
        protected double _data = 0;

        override public NBT_Double toDouble () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_DOUBLE; }

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
    }

    public class NBT_ByteArray : NBT_Value
    {
        protected byte[] _data = null;

        override public NBT_ByteArray toByteArray () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_BYTE_ARRAY; }

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
    }

    public class NBT_String : NBT_Value
    {
        protected string _data = null;

        override public NBT_String toString () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_STRING; }

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
    }

    public class NBT_List : NBT_Value
    {
        protected NBT_Type _type = NBT_Type.TAG_END;

        protected List<NBT_Value> _items = null;

        override public NBT_List toList () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_LIST; }

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

        public NBT_List (NBT_Type type) {
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
            if (_type != val.getType()) {
                throw new InvalidValueException();
            }

            _items.Add(val);
        }

        public bool RemoveItem (NBT_Value val)
        {
            return _items.Remove(val);
        }
    }

    public class NBT_Compound : NBT_Value
    {
        protected List<NBT_Tag> _tags = null;

        override public NBT_Compound toCompound () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_COMPOUND; }

        public int Count
        {
            get { return _tags.Count; }
        }

        public List<NBT_Tag> Tags
        {
            get { return _tags; }
        }

        public NBT_Compound () {
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

        public bool RemoveTag (NBT_Tag sub)
        {
            return _tags.Remove(sub);
        }

        public NBT_Tag FindTagByName (string name)
        {
            foreach (NBT_Tag tag in _tags) {
                if (tag.name.Data == name) {
                    return tag;
                }
            }

            return null;
        }
    }

    public class NBT_Tag
    {
        public NBT_Type type;
        public NBT_String name;

        public NBT_Value value;

        public NBT_Tag FindTagByName (string name)
        {
            if (type != NBT_Type.TAG_COMPOUND) {
                throw new InvalidTagException();
            }

            return value.toCompound().FindTagByName(name);
        }
    }

    public class NBT_Tree
    {
        private Stream stream = null;

        protected NBT_Tag _root = null;

        public NBT_Tree (Stream s)
        {
            ReadFrom(s);
        }

        public void ReadFrom (Stream s)
        {
            if (s != null) {
                stream = s;
                _root = ReadTag();
                stream = null;
            }
        }

        public void WriteTo (Stream s)
        {
            if (s != null) {
                stream = s;

                if (_root != null) {
                    WriteTag(_root);
                }

                stream = null;
            }
        }

        public NBT_Tag Root
        {
            get { return _root; }
        }

        private NBT_Value ReadValue (NBT_Type type)
        {
            switch (type) {
                case NBT_Type.TAG_END:
                    return null;

                case NBT_Type.TAG_BYTE:
                    return ReadByte();

                case NBT_Type.TAG_SHORT:
                    return ReadShort();

                case NBT_Type.TAG_INT:
                    return ReadInt();

                case NBT_Type.TAG_LONG:
                    return ReadLong();

                case NBT_Type.TAG_FLOAT:
                    return ReadFloat();

                case NBT_Type.TAG_DOUBLE:
                    return ReadDouble();

                case NBT_Type.TAG_BYTE_ARRAY:
                    return ReadByteArray();

                case NBT_Type.TAG_STRING:
                    return ReadString();

                case NBT_Type.TAG_LIST:
                    return ReadList();

                case NBT_Type.TAG_COMPOUND:
                    return ReadCompound();
            }

            throw new Exception();
        }

        private NBT_Value ReadByte ()
        {
            int gzByte = stream.ReadByte();
            if (gzByte == -1) {
                throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
            }

            NBT_Byte val = new NBT_Byte((byte)gzByte);

            return val;
        }

        private NBT_Value ReadShort ()
        {
            byte[] gzBytes = new byte[2];
            stream.Read(gzBytes, 0, 2);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Short val = new NBT_Short(BitConverter.ToInt16(gzBytes, 0));

            return val;
        }

        private NBT_Value ReadInt ()
        {
            byte[] gzBytes = new byte[4];
            stream.Read(gzBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Int val = new NBT_Int(BitConverter.ToInt32(gzBytes, 0));

            return val;
        }

        private NBT_Value ReadLong ()
        {
            byte[] gzBytes = new byte[8];
            stream.Read(gzBytes, 0, 8);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Long val = new NBT_Long(BitConverter.ToInt64(gzBytes, 0));

            return val;
        }

        private NBT_Value ReadFloat ()
        {
            byte[] gzBytes = new byte[4];
            stream.Read(gzBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Float val = new NBT_Float(BitConverter.ToSingle(gzBytes, 0));

            return val;
        }

        private NBT_Value ReadDouble ()
        {
            byte[] gzBytes = new byte[8];
            stream.Read(gzBytes, 0, 8);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Double val = new NBT_Double(BitConverter.ToDouble(gzBytes, 0));

            return val;
        }

        private NBT_Value ReadByteArray ()
        {
            byte[] lenBytes = new byte[4];
            stream.Read(lenBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            int length = BitConverter.ToInt32(lenBytes, 0);
            if (length < 0) {
                throw new NBTException(NBTException.MSG_READ_NEG);
            }

            byte[] data = new byte[length];
            stream.Read(data, 0, length);

            NBT_ByteArray val = new NBT_ByteArray(data);

            return val;
        }

        private NBT_Value ReadString ()
        {
            byte[] lenBytes = new byte[2];
            stream.Read(lenBytes, 0, 2);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            short len = BitConverter.ToInt16(lenBytes, 0);
            if (len < 0) {
                throw new NBTException(NBTException.MSG_READ_NEG);
            }

            byte[] strBytes = new byte[len];
            stream.Read(strBytes, 0, len);

            System.Text.Encoding str = Encoding.GetEncoding(28591);

            NBT_String val = new NBT_String(str.GetString(strBytes));

            return val;
        }

        private NBT_Value ReadList ()
        {
            int gzByte = stream.ReadByte();
            if (gzByte == -1) {
                throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
            }

            NBT_List val = new NBT_List((NBT_Type)gzByte);
            if (val.ValueType > (NBT_Type)Enum.GetValues(typeof(NBT_Type)).GetUpperBound(0)) {
                throw new NBTException(NBTException.MSG_READ_TYPE);
            }

            byte[] lenBytes = new byte[4];
            stream.Read(lenBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            int length = BitConverter.ToInt32(lenBytes, 0);
            if (length < 0) {
                throw new NBTException(NBTException.MSG_READ_NEG);
            }

            for (int i = 0; i < length; i++) {
                val.AddItem(ReadValue(val.ValueType));
            }

            return val;
        }

        private NBT_Value ReadCompound ()
        {
            NBT_Compound val = new NBT_Compound();

            while (true) {
                NBT_Tag tag = ReadTag();
                if (tag.type == NBT_Type.TAG_END) {
                    break;
                }

                val.AddTag(tag);
            }

            return val;
        }

        private NBT_Tag ReadTag ()
        {
            NBT_Tag tag = new NBT_Tag();

            tag.type = (NBT_Type)stream.ReadByte();
            tag.name = null;
            tag.value = null;

            if (tag.type != NBT_Type.TAG_END) {
                tag.name = ReadString().toString();
            }

            tag.value = ReadValue(tag.type);

            return tag;
        }

        private void WriteValue (NBT_Value val)
        {
            switch (val.getType()) {
                case NBT_Type.TAG_END:
                    break;

                case NBT_Type.TAG_BYTE:
                    WriteByte(val.toByte());
                    break;

                case NBT_Type.TAG_SHORT:
                    WriteShort(val.toShort());
                    break;

                case NBT_Type.TAG_INT:
                    WriteInt(val.toInt());
                    break;

                case NBT_Type.TAG_LONG:
                    WriteLong(val.toLong());
                    break;

                case NBT_Type.TAG_FLOAT:
                    WriteFloat(val.toFloat());
                    break;

                case NBT_Type.TAG_DOUBLE:
                    WriteDouble(val.toDouble());
                    break;

                case NBT_Type.TAG_BYTE_ARRAY:
                    WriteByteArray(val.toByteArray());
                    break;

                case NBT_Type.TAG_STRING:
                    WriteString(val.toString());
                    break;

                case NBT_Type.TAG_LIST:
                    WriteList(val.toList());
                    break;

                case NBT_Type.TAG_COMPOUND:
                    WriteCompound(val.toCompound());
                    break;
            }
        }

        private void WriteByte (NBT_Byte val)
        {
            stream.WriteByte(val.Data);
        }

        private void WriteShort (NBT_Short val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            stream.Write(gzBytes, 0, 2);
        }

        private void WriteInt (NBT_Int val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            stream.Write(gzBytes, 0, 4);
        }

        private void WriteLong (NBT_Long val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            stream.Write(gzBytes, 0, 8);
        }

        private void WriteFloat (NBT_Float val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            stream.Write(gzBytes, 0, 4);
        }

        private void WriteDouble (NBT_Double val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            stream.Write(gzBytes, 0, 8);
        }

        private void WriteByteArray (NBT_ByteArray val)
        {
            byte[] lenBytes = BitConverter.GetBytes(val.Length);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            stream.Write(lenBytes, 0, 4);
            stream.Write(val.Data, 0, val.Length);
        }

        private void WriteString (NBT_String val)
        {
            byte[] lenBytes = BitConverter.GetBytes((short)val.Length);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            stream.Write(lenBytes, 0, 2);

            System.Text.Encoding str = Encoding.GetEncoding(28591);
            byte[] gzBytes = str.GetBytes(val.Data);

            stream.Write(gzBytes, 0, gzBytes.Length);
        }

        private void WriteList (NBT_List val)
        {
            byte[] lenBytes = BitConverter.GetBytes(val.Count);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            stream.WriteByte((byte)val.ValueType);
            stream.Write(lenBytes, 0, 4);

            foreach (NBT_Value v in val.Items) {
                WriteValue(v);
            }
        }

        private void WriteCompound (NBT_Compound val)
        {
            foreach (NBT_Tag t in val.Tags) {
                WriteTag(t);
            }

            NBT_Tag e = new NBT_Tag();
            e.type = NBT_Type.TAG_END;

            WriteTag(e);
        }

        private void WriteTag (NBT_Tag tag)
        {
            stream.WriteByte((byte)tag.type);

            if (tag.type != NBT_Type.TAG_END) {
                WriteString(tag.name);
                WriteValue(tag.value);
            }
        }
    }

    public class NBTException : Exception
    {
        public const String MSG_GZIP_ENDOFSTREAM = "Gzip Error: Unexpected end of stream";

        public const String MSG_READ_NEG = "Read Error: Negative length";
        public const String MSG_READ_TYPE = "Read Error: Invalid value type";

        public NBTException () { }

        public NBTException (String msg) : base(msg) { }

        public NBTException (String msg, Exception innerException) : base(msg, innerException) { }
    }

    public class InvalidTagException : Exception { }

    public class InvalidValueException : Exception { }
}
