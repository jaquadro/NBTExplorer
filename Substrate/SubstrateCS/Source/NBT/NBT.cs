using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace NBToolkit.Map.NBT
{
    using Map.Utility;

    public interface INBTObject<T>
    {
        T LoadTree (NBT_Value tree);
        T LoadTreeSafe (NBT_Value tree);

        NBT_Value BuildTree ();

        bool ValidateTree (NBT_Value tree);
    }

    public class NBT_Tree : ICopyable<NBT_Tree>
    {
        private Stream _stream = null;
        private NBT_Compound _root = null;

        private static NBT_Null _nulltag = new NBT_Null();

        public NBT_Compound Root
        {
            get { return _root; }
        }

        public NBT_Tree ()
        {
            _root = new NBT_Compound();
        }

        public NBT_Tree (NBT_Compound tree)
        {
            _root = tree;
        }

        public NBT_Tree (Stream s)
        {
            ReadFrom(s);
        }

        public void ReadFrom (Stream s)
        {
            if (s != null) {
                _stream = s;
                _root = ReadRoot();
                _stream = null;
            }
        }

        public void WriteTo (Stream s)
        {
            if (s != null) {
                _stream = s;

                if (_root != null) {
                    WriteTag("", _root);
                }

                _stream = null;
            }
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
            int gzByte = _stream.ReadByte();
            if (gzByte == -1) {
                throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
            }

            NBT_Byte val = new NBT_Byte((byte)gzByte);

            return val;
        }

        private NBT_Value ReadShort ()
        {
            byte[] gzBytes = new byte[2];
            _stream.Read(gzBytes, 0, 2);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Short val = new NBT_Short(BitConverter.ToInt16(gzBytes, 0));

            return val;
        }

        private NBT_Value ReadInt ()
        {
            byte[] gzBytes = new byte[4];
            _stream.Read(gzBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Int val = new NBT_Int(BitConverter.ToInt32(gzBytes, 0));

            return val;
        }

        private NBT_Value ReadLong ()
        {
            byte[] gzBytes = new byte[8];
            _stream.Read(gzBytes, 0, 8);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Long val = new NBT_Long(BitConverter.ToInt64(gzBytes, 0));

            return val;
        }

        private NBT_Value ReadFloat ()
        {
            byte[] gzBytes = new byte[4];
            _stream.Read(gzBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Float val = new NBT_Float(BitConverter.ToSingle(gzBytes, 0));

            return val;
        }

        private NBT_Value ReadDouble ()
        {
            byte[] gzBytes = new byte[8];
            _stream.Read(gzBytes, 0, 8);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Double val = new NBT_Double(BitConverter.ToDouble(gzBytes, 0));

            return val;
        }

        private NBT_Value ReadByteArray ()
        {
            byte[] lenBytes = new byte[4];
            _stream.Read(lenBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            int length = BitConverter.ToInt32(lenBytes, 0);
            if (length < 0) {
                throw new NBTException(NBTException.MSG_READ_NEG);
            }

            byte[] data = new byte[length];
            _stream.Read(data, 0, length);

            NBT_ByteArray val = new NBT_ByteArray(data);

            return val;
        }

        private NBT_Value ReadString ()
        {
            byte[] lenBytes = new byte[2];
            _stream.Read(lenBytes, 0, 2);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            short len = BitConverter.ToInt16(lenBytes, 0);
            if (len < 0) {
                throw new NBTException(NBTException.MSG_READ_NEG);
            }

            byte[] strBytes = new byte[len];
            _stream.Read(strBytes, 0, len);

            System.Text.Encoding str = Encoding.GetEncoding(28591);

            NBT_String val = new NBT_String(str.GetString(strBytes));

            return val;
        }

        private NBT_Value ReadList ()
        {
            int gzByte = _stream.ReadByte();
            if (gzByte == -1) {
                throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
            }

            NBT_List val = new NBT_List((NBT_Type)gzByte);
            if (val.ValueType > (NBT_Type)Enum.GetValues(typeof(NBT_Type)).GetUpperBound(0)) {
                throw new NBTException(NBTException.MSG_READ_TYPE);
            }

            byte[] lenBytes = new byte[4];
            _stream.Read(lenBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            int length = BitConverter.ToInt32(lenBytes, 0);
            if (length < 0) {
                throw new NBTException(NBTException.MSG_READ_NEG);
            }

            for (int i = 0; i < length; i++) {
                val.Add(ReadValue(val.ValueType));
            }

            return val;
        }

        private NBT_Value ReadCompound ()
        {
            NBT_Compound val = new NBT_Compound();

            while (ReadTag(val)) ;

            return val;
        }

        private NBT_Compound ReadRoot ()
        {
            NBT_Type type = (NBT_Type)_stream.ReadByte();
            if (type == NBT_Type.TAG_COMPOUND) {
                string name = ReadString().ToNBTString().Data;
                return ReadValue(type) as NBT_Compound;
            }

            return null;
        }

        private bool ReadTag (NBT_Compound parent)
        {
            //NBT_Tag tag = new NBT_Tag();

            NBT_Type type = (NBT_Type)_stream.ReadByte();
            if (type != NBT_Type.TAG_END) {
                string name = ReadString().ToNBTString().Data;
                parent[name] = ReadValue(type);
                return true;
            }

            return false;

            //tag.Value = ReadValue(type);

            //return tag;
        }

        private void WriteValue (NBT_Value val)
        {
            switch (val.GetNBTType()) {
                case NBT_Type.TAG_END:
                    break;

                case NBT_Type.TAG_BYTE:
                    WriteByte(val.ToNBTByte());
                    break;

                case NBT_Type.TAG_SHORT:
                    WriteShort(val.ToNBTShort());
                    break;

                case NBT_Type.TAG_INT:
                    WriteInt(val.ToNBTInt());
                    break;

                case NBT_Type.TAG_LONG:
                    WriteLong(val.ToNBTLong());
                    break;

                case NBT_Type.TAG_FLOAT:
                    WriteFloat(val.ToNBTFloat());
                    break;

                case NBT_Type.TAG_DOUBLE:
                    WriteDouble(val.ToNBTDouble());
                    break;

                case NBT_Type.TAG_BYTE_ARRAY:
                    WriteByteArray(val.ToNBTByteArray());
                    break;

                case NBT_Type.TAG_STRING:
                    WriteString(val.ToNBTString());
                    break;

                case NBT_Type.TAG_LIST:
                    WriteList(val.ToNBTList());
                    break;

                case NBT_Type.TAG_COMPOUND:
                    WriteCompound(val.ToNBTCompound());
                    break;
            }
        }

        private void WriteByte (NBT_Byte val)
        {
            _stream.WriteByte(val.Data);
        }

        private void WriteShort (NBT_Short val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 2);
        }

        private void WriteInt (NBT_Int val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 4);
        }

        private void WriteLong (NBT_Long val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 8);
        }

        private void WriteFloat (NBT_Float val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 4);
        }

        private void WriteDouble (NBT_Double val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 8);
        }

        private void WriteByteArray (NBT_ByteArray val)
        {
            byte[] lenBytes = BitConverter.GetBytes(val.Length);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            _stream.Write(lenBytes, 0, 4);
            _stream.Write(val.Data, 0, val.Length);
        }

        private void WriteString (NBT_String val)
        {
            byte[] lenBytes = BitConverter.GetBytes((short)val.Length);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            _stream.Write(lenBytes, 0, 2);

            System.Text.Encoding str = Encoding.GetEncoding(28591);
            byte[] gzBytes = str.GetBytes(val.Data);

            _stream.Write(gzBytes, 0, gzBytes.Length);
        }

        private void WriteList (NBT_List val)
        {
            byte[] lenBytes = BitConverter.GetBytes(val.Count);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            _stream.WriteByte((byte)val.ValueType);
            _stream.Write(lenBytes, 0, 4);

            foreach (NBT_Value v in val) {
                WriteValue(v);
            }
        }

        private void WriteCompound (NBT_Compound val)
        {
            foreach (KeyValuePair<string, NBT_Value> item in val) {
                WriteTag(item.Key, item.Value);
            }

            WriteTag(null, _nulltag);
        }

        private void WriteTag (string name, NBT_Value val)
        {
            _stream.WriteByte((byte)val.GetNBTType());

            if (val.GetNBTType() != NBT_Type.TAG_END) {
                WriteString(name);
                WriteValue(val);
            }
        }

        #region ICopyable<NBT_Tree> Members

        public NBT_Tree Copy ()
        {
            NBT_Tree tree = new NBT_Tree();
            tree._root = _root.Copy() as NBT_Compound;

            return tree;
        }

        #endregion
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

    public class InvalidNBTObjectException : Exception { }

    public class InvalidTagException : Exception { }

    public class InvalidValueException : Exception { }
}
