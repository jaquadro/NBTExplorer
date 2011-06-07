using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Substrate.NBT
{
    using Substrate.Utility;

    public interface INBTObject<T>
    {
        T LoadTree (TagValue tree);
        T LoadTreeSafe (TagValue tree);

        TagValue BuildTree ();

        bool ValidateTree (TagValue tree);
    }

    public class NBT_Tree : ICopyable<NBT_Tree>
    {
        private Stream _stream = null;
        private TagCompound _root = null;

        private static TagNull _nulltag = new TagNull();

        public TagCompound Root
        {
            get { return _root; }
        }

        public NBT_Tree ()
        {
            _root = new TagCompound();
        }

        public NBT_Tree (TagCompound tree)
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

        private TagValue ReadValue (TagType type)
        {
            switch (type) {
                case TagType.TAG_END:
                    return null;

                case TagType.TAG_BYTE:
                    return ReadByte();

                case TagType.TAG_SHORT:
                    return ReadShort();

                case TagType.TAG_INT:
                    return ReadInt();

                case TagType.TAG_LONG:
                    return ReadLong();

                case TagType.TAG_FLOAT:
                    return ReadFloat();

                case TagType.TAG_DOUBLE:
                    return ReadDouble();

                case TagType.TAG_BYTE_ARRAY:
                    return ReadByteArray();

                case TagType.TAG_STRING:
                    return ReadString();

                case TagType.TAG_LIST:
                    return ReadList();

                case TagType.TAG_COMPOUND:
                    return ReadCompound();
            }

            throw new Exception();
        }

        private TagValue ReadByte ()
        {
            int gzByte = _stream.ReadByte();
            if (gzByte == -1) {
                throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
            }

            TagByte val = new TagByte((byte)gzByte);

            return val;
        }

        private TagValue ReadShort ()
        {
            byte[] gzBytes = new byte[2];
            _stream.Read(gzBytes, 0, 2);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            TagShort val = new TagShort(BitConverter.ToInt16(gzBytes, 0));

            return val;
        }

        private TagValue ReadInt ()
        {
            byte[] gzBytes = new byte[4];
            _stream.Read(gzBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            TagInt val = new TagInt(BitConverter.ToInt32(gzBytes, 0));

            return val;
        }

        private TagValue ReadLong ()
        {
            byte[] gzBytes = new byte[8];
            _stream.Read(gzBytes, 0, 8);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            TagLong val = new TagLong(BitConverter.ToInt64(gzBytes, 0));

            return val;
        }

        private TagValue ReadFloat ()
        {
            byte[] gzBytes = new byte[4];
            _stream.Read(gzBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            TagFloat val = new TagFloat(BitConverter.ToSingle(gzBytes, 0));

            return val;
        }

        private TagValue ReadDouble ()
        {
            byte[] gzBytes = new byte[8];
            _stream.Read(gzBytes, 0, 8);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            TagDouble val = new TagDouble(BitConverter.ToDouble(gzBytes, 0));

            return val;
        }

        private TagValue ReadByteArray ()
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

            TagByteArray val = new TagByteArray(data);

            return val;
        }

        private TagValue ReadString ()
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

            TagString val = new TagString(str.GetString(strBytes));

            return val;
        }

        private TagValue ReadList ()
        {
            int gzByte = _stream.ReadByte();
            if (gzByte == -1) {
                throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
            }

            TagList val = new TagList((TagType)gzByte);
            if (val.ValueType > (TagType)Enum.GetValues(typeof(TagType)).GetUpperBound(0)) {
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

        private TagValue ReadCompound ()
        {
            TagCompound val = new TagCompound();

            while (ReadTag(val)) ;

            return val;
        }

        private TagCompound ReadRoot ()
        {
            TagType type = (TagType)_stream.ReadByte();
            if (type == TagType.TAG_COMPOUND) {
                ReadString(); // name
                return ReadValue(type) as TagCompound;
            }

            return null;
        }

        private bool ReadTag (TagCompound parent)
        {
            //NBT_Tag tag = new NBT_Tag();

            TagType type = (TagType)_stream.ReadByte();
            if (type != TagType.TAG_END) {
                string name = ReadString().ToTagString().Data;
                parent[name] = ReadValue(type);
                return true;
            }

            return false;

            //tag.Value = ReadValue(type);

            //return tag;
        }

        private void WriteValue (TagValue val)
        {
            switch (val.GetTagType()) {
                case TagType.TAG_END:
                    break;

                case TagType.TAG_BYTE:
                    WriteByte(val.ToTagByte());
                    break;

                case TagType.TAG_SHORT:
                    WriteShort(val.ToTagShort());
                    break;

                case TagType.TAG_INT:
                    WriteInt(val.ToTagInt());
                    break;

                case TagType.TAG_LONG:
                    WriteLong(val.ToTagLong());
                    break;

                case TagType.TAG_FLOAT:
                    WriteFloat(val.ToTagFloat());
                    break;

                case TagType.TAG_DOUBLE:
                    WriteDouble(val.ToTagDouble());
                    break;

                case TagType.TAG_BYTE_ARRAY:
                    WriteByteArray(val.ToTagByteArray());
                    break;

                case TagType.TAG_STRING:
                    WriteString(val.ToTagString());
                    break;

                case TagType.TAG_LIST:
                    WriteList(val.ToTagList());
                    break;

                case TagType.TAG_COMPOUND:
                    WriteCompound(val.ToTagCompound());
                    break;
            }
        }

        private void WriteByte (TagByte val)
        {
            _stream.WriteByte(val.Data);
        }

        private void WriteShort (TagShort val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 2);
        }

        private void WriteInt (TagInt val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 4);
        }

        private void WriteLong (TagLong val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 8);
        }

        private void WriteFloat (TagFloat val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 4);
        }

        private void WriteDouble (TagDouble val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 8);
        }

        private void WriteByteArray (TagByteArray val)
        {
            byte[] lenBytes = BitConverter.GetBytes(val.Length);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            _stream.Write(lenBytes, 0, 4);
            _stream.Write(val.Data, 0, val.Length);
        }

        private void WriteString (TagString val)
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

        private void WriteList (TagList val)
        {
            byte[] lenBytes = BitConverter.GetBytes(val.Count);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            _stream.WriteByte((byte)val.ValueType);
            _stream.Write(lenBytes, 0, 4);

            foreach (TagValue v in val) {
                WriteValue(v);
            }
        }

        private void WriteCompound (TagCompound val)
        {
            foreach (KeyValuePair<string, TagValue> item in val) {
                WriteTag(item.Key, item.Value);
            }

            WriteTag(null, _nulltag);
        }

        private void WriteTag (string name, TagValue val)
        {
            _stream.WriteByte((byte)val.GetTagType());

            if (val.GetTagType() != TagType.TAG_END) {
                WriteString(name);
                WriteValue(val);
            }
        }

        #region ICopyable<NBT_Tree> Members

        public NBT_Tree Copy ()
        {
            NBT_Tree tree = new NBT_Tree();
            tree._root = _root.Copy() as TagCompound;

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
