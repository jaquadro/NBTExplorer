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

    public class NBT_Value
    {
        virtual public NBT_Byte toByte () { return null; }
        virtual public NBT_Short toShort () { return null; }
        virtual public NBT_Int toInt () { return null; }
        virtual public NBT_Long toLong () { return null; }
        virtual public NBT_Float toFloat () { return null; }
        virtual public NBT_Double toDouble () { return null; }
        virtual public NBT_ByteArray toByteArray () { return null; }
        virtual public NBT_String toString () { return null; }
        virtual public NBT_List toList () { return null; }
        virtual public NBT_Compound toCompound () { return null; }

        virtual public NBT_Type getType () { return NBT_Type.TAG_END; }
    }

    public class NBT_Byte : NBT_Value
    {
        public byte data = 0;
        override public NBT_Byte toByte () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_BYTE; }
    }

    public class NBT_Short : NBT_Value
    {
        public short data = 0;
        override public NBT_Short toShort () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_SHORT; }
    }

    public class NBT_Int : NBT_Value
    {
        public int data = 0;
        override public NBT_Int toInt () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_INT; }
    }

    public class NBT_Long : NBT_Value
    {
        public long data = 0;
        override public NBT_Long toLong () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_LONG; }
    }

    public class NBT_Float : NBT_Value
    {
        public float data = 0;
        override public NBT_Float toFloat () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_FLOAT; }
    }

    public class NBT_Double : NBT_Value
    {
        public double data = 0;
        override public NBT_Double toDouble () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_DOUBLE; }
    }

    public class NBT_ByteArray : NBT_Value
    {
        public int length = 0;
        public byte[] data = null;

        override public NBT_ByteArray toByteArray () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_BYTE_ARRAY; }
    }

    public class NBT_String : NBT_Value
    {
        public String data = null;
        override public NBT_String toString () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_STRING; }
    }

    public class NBT_List : NBT_Value
    {
        public int length = 0;
        public NBT_Type type = NBT_Type.TAG_END;

        public List<NBT_Value> items = null;

        override public NBT_List toList () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_LIST; }
    }

    public class NBT_Compound : NBT_Value
    {
        public int length = 0;

        public List<NBT_Tag> tags = null;

        override public NBT_Compound toCompound () { return this; }
        override public NBT_Type getType () { return NBT_Type.TAG_COMPOUND; }
    }

    public class NBT_Tag
    {
        public NBT_Type type;
        public NBT_String name;

        public NBT_Value value;

        public NBT_Tag findTagByName (String name)
        {
            if (type != NBT_Type.TAG_COMPOUND) {
                return null;
            }

            foreach (NBT_Tag tag in value.toCompound().tags) {
                if (tag.name.data.Equals(name)) {
                    return tag;
                }
            }

            return null;
        }
    }

    public class NBT_Tree
    {
        private Stream stream = null;

        //String path = null;

        NBT_Tag root = null;

        public NBT_Tree (Stream s)
        {
            ReadFrom(s);
        }

        public void ReadFrom (Stream s)
        {
            if (s != null) {
                stream = s;
                root = ReadTag();
                stream = null;
            }
        }

        public void WriteTo (Stream s)
        {
            if (s != null) {
                stream = s;

                if (root != null) {
                    WriteTag(root);
                }

                stream = null;
            }
        }

        /*public void activate ()
        {
            if (root == null) {
                read();
            }
        }

        public void deactivate ()
        {
            if (root != null) {
                root = null;
            }
        }

        public void save ()
        {
            if (root != null) {
                write();
            }
        }*/

        public NBT_Tag getRoot ()
        {
            return root;
        }

        /*public void addListItem (NBT_Tag tag, NBT_Value val)
        {
            if (tag.type != NBT_Type.TAG_LIST) {
                throw new Exception();
            }

            if (tag.value.toList().type != val.getType()) {
                throw new Exception();
            }

            val.toList().length++;
            val.toList().items.Add(val);
        }

        public void addTag (NBT_Tag tag, NBT_Tag sub)
        {
            if (tag.type != NBT_Type.TAG_COMPOUND) {
                throw new Exception();
            }

            tag.value.toCompound().length++;
            tag.value.toCompound().tags.Add(sub);
        }*/

        /*private void read ()
        {
            FileStream fStream = new FileStream(path, System.IO.FileMode.Open);
            gzStream = new GZipStream(fStream, CompressionMode.Decompress);

            root = readTag();

            gzStream.Close();

            gzStream = null;
        }*/

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
            //Console.Write("NBT_File.readByte()");

            int gzByte = stream.ReadByte();
            if (gzByte == -1) {
                throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
            }

            NBT_Byte val = new NBT_Byte();
            val.data = (byte)gzByte;

            //Console.WriteLine(" [" + val.data + "]");

            return val;
        }

        private NBT_Value ReadShort ()
        {
            //Console.Write("NBT_File.readShort");

            byte[] gzBytes = new byte[2];
            stream.Read(gzBytes, 0, 2);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Short val = new NBT_Short();
            val.data = BitConverter.ToInt16(gzBytes, 0);

            //Console.WriteLine(" [" + val.data + "]");

            return val;
        }

        private NBT_Value ReadInt ()
        {
            //Console.Write("NBT_File.readInt");

            byte[] gzBytes = new byte[4];
            stream.Read(gzBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Int val = new NBT_Int();
            val.data = BitConverter.ToInt32(gzBytes, 0);

            //Console.WriteLine(" [" + val.data + "]");

            return val;
        }

        private NBT_Value ReadLong ()
        {
            //Console.Write("NBT_File.readLong");

            byte[] gzBytes = new byte[8];
            stream.Read(gzBytes, 0, 8);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Long val = new NBT_Long();
            val.data = BitConverter.ToInt64(gzBytes, 0);

            //Console.WriteLine(" [" + val.data + "]");

            return val;
        }

        private NBT_Value ReadFloat ()
        {
            //Console.Write("NBT_File.readFloat()");

            byte[] gzBytes = new byte[4];
            stream.Read(gzBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Float val = new NBT_Float();
            val.data = BitConverter.ToSingle(gzBytes, 0);

            //Console.WriteLine(" [" + val.data + "]");

            return val;
        }

        private NBT_Value ReadDouble ()
        {
            //Console.Write("NBT_File.readDouble()");

            byte[] gzBytes = new byte[8];
            stream.Read(gzBytes, 0, 8);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            NBT_Double val = new NBT_Double();
            val.data = BitConverter.ToDouble(gzBytes, 0);

            //Console.WriteLine(" [" + val.data + "]");

            return val;
        }

        private NBT_Value ReadByteArray ()
        {
            //Console.Write("NBT_File.readByteArray()");

            byte[] lenBytes = new byte[4];
            stream.Read(lenBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            NBT_ByteArray val = new NBT_ByteArray();
            val.length = BitConverter.ToInt32(lenBytes, 0);

            if (val.length < 0) {
                throw new NBTException(NBTException.MSG_READ_NEG);
            }

            //Console.WriteLine(" [" + val.length + "]");

            val.data = new byte[val.length];
            stream.Read(val.data, 0, val.length);

            return val;
        }

        private NBT_Value ReadString ()
        {
            //Console.Write("NBT_File.readString()");

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

            NBT_String val = new NBT_String();
            val.data = str.GetString(strBytes);

            //Console.WriteLine(" [" + val.data.ToString() + "]");

            return val;
        }

        private NBT_Value ReadList ()
        {
            //Console.Write("NBT_File.readList()");

            int gzByte = stream.ReadByte();
            if (gzByte == -1) {
                throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
            }

            NBT_List val = new NBT_List();
            val.type = (NBT_Type)gzByte;
            if (val.type > (NBT_Type)Enum.GetValues(typeof(NBT_Type)).GetUpperBound(0)) {
                throw new NBTException(NBTException.MSG_READ_TYPE);
            }

            byte[] lenBytes = new byte[4];
            stream.Read(lenBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            val.length = BitConverter.ToInt32(lenBytes, 0);
            if (val.length < 0) {
                throw new NBTException(NBTException.MSG_READ_NEG);
            }

            //Console.WriteLine(" [" + val.type + ", " + val.length + "]");

            val.items = new List<NBT_Value>();

            for (int i = 0; i < val.length; i++) {
                val.items.Add(ReadValue(val.type));
            }

            return val;
        }

        private NBT_Value ReadCompound ()
        {
            //Console.WriteLine("NBT_File.readCompound()");

            NBT_Compound val = new NBT_Compound();
            val.tags = new List<NBT_Tag>();

            while (true) {
                NBT_Tag tag = ReadTag();
                if (tag.type == NBT_Type.TAG_END) {
                    break;
                }

                val.length++;
                val.tags.Add(tag);
            }

            return val;
        }

        private NBT_Tag ReadTag ()
        {
            //Console.Write("NBT_File.readTag()");

            NBT_Tag tag = new NBT_Tag();

            tag.type = (NBT_Type)stream.ReadByte();
            tag.name = null;
            tag.value = null;

            //Console.WriteLine(" [" + tag.type + "]");

            if (tag.type != NBT_Type.TAG_END) {
                tag.name = ReadString().toString();
            }

            tag.value = ReadValue(tag.type);

            return tag;
        }

        /*private void write ()
        {
            if (root != null) {
                FileStream fStream = new FileStream(path, System.IO.FileMode.Truncate);
                stream = new GZipStream(fStream, CompressionMode.Compress);

                writeTag(root);

                stream.Close();
            }

            stream = null;
        }*/

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
            stream.WriteByte(val.data);
        }

        private void WriteShort (NBT_Short val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.data);

            if (BitConverter.IsLittleEndian) {
                //gzBytes.Reverse();
                Array.Reverse(gzBytes);
            }

            stream.Write(gzBytes, 0, 2);
        }

        private void WriteInt (NBT_Int val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.data);

            if (BitConverter.IsLittleEndian) {
                //gzBytes.Reverse();
                Array.Reverse(gzBytes);
            }

            stream.Write(gzBytes, 0, 4);
        }

        private void WriteLong (NBT_Long val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.data);

            if (BitConverter.IsLittleEndian) {
                //gzBytes.Reverse();
                Array.Reverse(gzBytes);
            }

            stream.Write(gzBytes, 0, 8);
        }

        private void WriteFloat (NBT_Float val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.data);

            if (BitConverter.IsLittleEndian) {
                //gzBytes.Reverse();
                Array.Reverse(gzBytes);
            }

            stream.Write(gzBytes, 0, 4);
        }

        private void WriteDouble (NBT_Double val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.data);

            if (BitConverter.IsLittleEndian) {
                //gzBytes.Reverse();
                Array.Reverse(gzBytes);
            }

            stream.Write(gzBytes, 0, 8);
        }

        private void WriteByteArray (NBT_ByteArray val)
        {
            byte[] lenBytes = BitConverter.GetBytes(val.length);

            if (BitConverter.IsLittleEndian) {
                //lenBytes.Reverse();
                Array.Reverse(lenBytes);
            }

            stream.Write(lenBytes, 0, 4);
            stream.Write(val.data, 0, val.length);
        }

        private void WriteString (NBT_String val)
        {
            byte[] lenBytes = BitConverter.GetBytes((short)val.data.Length);

            if (BitConverter.IsLittleEndian) {
                //lenBytes.Reverse();
                Array.Reverse(lenBytes);
            }

            stream.Write(lenBytes, 0, 2);

            System.Text.Encoding str = Encoding.GetEncoding(28591);
            byte[] gzBytes = str.GetBytes(val.data);

            stream.Write(gzBytes, 0, gzBytes.Length);
        }

        private void WriteList (NBT_List val)
        {
            byte[] lenBytes = BitConverter.GetBytes(val.length);

            if (BitConverter.IsLittleEndian) {
                //lenBytes.Reverse();
                Array.Reverse(lenBytes);
            }

            stream.WriteByte((byte)val.type);
            stream.Write(lenBytes, 0, 4);

            foreach (NBT_Value v in val.items) {
                WriteValue(v);
            }
        }

        private void WriteCompound (NBT_Compound val)
        {
            foreach (NBT_Tag t in val.tags) {
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

    class NBTException : Exception
    {
        public const String MSG_GZIP_ENDOFSTREAM = "Gzip Error: Unexpected end of stream";

        public const String MSG_READ_NEG = "Read Error: Negative length";
        public const String MSG_READ_TYPE = "Read Error: Invalid value type";

        public NBTException () { }

        public NBTException (String msg) : base(msg) { }

        public NBTException (String msg, Exception innerException) : base(msg, innerException) { }
    }
}
