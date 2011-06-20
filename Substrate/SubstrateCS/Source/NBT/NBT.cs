using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Substrate.NBT
{
    using Substrate.Utility;

    /// <summary>
    /// Defines methods for loading or extracting an NBT tree.
    /// </summary>
    /// <typeparam name="T">Object type that supports this interface.</typeparam>
    public interface INBTObject<T>
    {
        /// <summary>
        /// Attempt to load an NBT tree into the object without validation.
        /// </summary>
        /// <param name="tree">The root node of an NBT tree.</param>
        /// <returns>The object returns itself on success, or null if the tree was unparsable.</returns>
        T LoadTree (TagNode tree);

        /// <summary>
        /// Attempt to load an NBT tree into the object with validation.
        /// </summary>
        /// <param name="tree">The root node of an NBT tree.</param>
        /// <returns>The object returns itself on success, or null if the tree failed validation.</returns>
        T LoadTreeSafe (TagNode tree);

        /// <summary>
        /// Builds an NBT tree from the object's data.
        /// </summary>
        /// <returns>The root node of an NBT tree representing the object's data.</returns>
        TagNode BuildTree ();

        /// <summary>
        /// Validate an NBT tree, usually against an object-supplied schema.
        /// </summary>
        /// <param name="tree">The root node of an NBT tree.</param>
        /// <returns>Status indicating whether the tree was valid for this object.</returns>
        bool ValidateTree (TagNode tree);
    }

    /// <summary>
    /// Contains the root node of an NBT tree and handles IO of tree nodes.
    /// </summary>
    /// <remarks>
    /// NBT, or Named Byte Tag, is a tree-based data structure for storing most Minecraft data.
    /// NBT_Tree is more of a helper class for NBT trees that handles reading and writing nodes to data streams.
    /// Most of the API takes a TagValue or derived node as the root of the tree, rather than an NBT_Tree object itself.
    /// </remarks>
    public class NBT_Tree : ICopyable<NBT_Tree>
    {
        private Stream _stream = null;
        private TagNodeCompound _root = null;

        private static TagNodeNull _nulltag = new TagNodeNull();

        /// <summary>
        /// Gets the root node of this tree.
        /// </summary>
        public TagNodeCompound Root
        {
            get { return _root; }
        }

        /// <summary>
        /// Constructs a wrapper around a new NBT tree with an empty root node.
        /// </summary>
        public NBT_Tree ()
        {
            _root = new TagNodeCompound();
        }

        /// <summary>
        /// Constructs a wrapper around another NBT tree.
        /// </summary>
        /// <param name="tree">The root node of an NBT tree.</param>
        public NBT_Tree (TagNodeCompound tree)
        {
            _root = tree;
        }

        /// <summary>
        /// Constructs and wrapper around a new NBT tree parsed from a source data stream.
        /// </summary>
        /// <param name="s">An open, readable data stream containing NBT data.</param>
        public NBT_Tree (Stream s)
        {
            ReadFrom(s);
        }

        /// <summary>
        /// Rebuild the internal NBT tree from a source data stream.
        /// </summary>
        /// <param name="s">An open, readable data stream containing NBT data.</param>
        public void ReadFrom (Stream s)
        {
            if (s != null) {
                _stream = s;
                _root = ReadRoot();
                _stream = null;
            }
        }

        /// <summary>
        /// Writes out the internal NBT tree to a destination data stream.
        /// </summary>
        /// <param name="s">An open, writable data stream.</param>
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

        private TagNode ReadValue (TagType type)
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

        private TagNode ReadByte ()
        {
            int gzByte = _stream.ReadByte();
            if (gzByte == -1) {
                throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
            }

            TagNodeByte val = new TagNodeByte((byte)gzByte);

            return val;
        }

        private TagNode ReadShort ()
        {
            byte[] gzBytes = new byte[2];
            _stream.Read(gzBytes, 0, 2);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            TagNodeShort val = new TagNodeShort(BitConverter.ToInt16(gzBytes, 0));

            return val;
        }

        private TagNode ReadInt ()
        {
            byte[] gzBytes = new byte[4];
            _stream.Read(gzBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            TagNodeInt val = new TagNodeInt(BitConverter.ToInt32(gzBytes, 0));

            return val;
        }

        private TagNode ReadLong ()
        {
            byte[] gzBytes = new byte[8];
            _stream.Read(gzBytes, 0, 8);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            TagNodeLong val = new TagNodeLong(BitConverter.ToInt64(gzBytes, 0));

            return val;
        }

        private TagNode ReadFloat ()
        {
            byte[] gzBytes = new byte[4];
            _stream.Read(gzBytes, 0, 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            TagNodeFloat val = new TagNodeFloat(BitConverter.ToSingle(gzBytes, 0));

            return val;
        }

        private TagNode ReadDouble ()
        {
            byte[] gzBytes = new byte[8];
            _stream.Read(gzBytes, 0, 8);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            TagNodeDouble val = new TagNodeDouble(BitConverter.ToDouble(gzBytes, 0));

            return val;
        }

        private TagNode ReadByteArray ()
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

            TagNodeByteArray val = new TagNodeByteArray(data);

            return val;
        }

        private TagNode ReadString ()
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

            TagNodeString val = new TagNodeString(str.GetString(strBytes));

            return val;
        }

        private TagNode ReadList ()
        {
            int gzByte = _stream.ReadByte();
            if (gzByte == -1) {
                throw new NBTException(NBTException.MSG_GZIP_ENDOFSTREAM);
            }

            TagNodeList val = new TagNodeList((TagType)gzByte);
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

        private TagNode ReadCompound ()
        {
            TagNodeCompound val = new TagNodeCompound();

            while (ReadTag(val)) ;

            return val;
        }

        private TagNodeCompound ReadRoot ()
        {
            TagType type = (TagType)_stream.ReadByte();
            if (type == TagType.TAG_COMPOUND) {
                ReadString(); // name
                return ReadValue(type) as TagNodeCompound;
            }

            return null;
        }

        private bool ReadTag (TagNodeCompound parent)
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

        private void WriteValue (TagNode val)
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

        private void WriteByte (TagNodeByte val)
        {
            _stream.WriteByte(val.Data);
        }

        private void WriteShort (TagNodeShort val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 2);
        }

        private void WriteInt (TagNodeInt val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 4);
        }

        private void WriteLong (TagNodeLong val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 8);
        }

        private void WriteFloat (TagNodeFloat val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 4);
        }

        private void WriteDouble (TagNodeDouble val)
        {
            byte[] gzBytes = BitConverter.GetBytes(val.Data);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(gzBytes);
            }

            _stream.Write(gzBytes, 0, 8);
        }

        private void WriteByteArray (TagNodeByteArray val)
        {
            byte[] lenBytes = BitConverter.GetBytes(val.Length);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            _stream.Write(lenBytes, 0, 4);
            _stream.Write(val.Data, 0, val.Length);
        }

        private void WriteString (TagNodeString val)
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

        private void WriteList (TagNodeList val)
        {
            byte[] lenBytes = BitConverter.GetBytes(val.Count);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(lenBytes);
            }

            _stream.WriteByte((byte)val.ValueType);
            _stream.Write(lenBytes, 0, 4);

            foreach (TagNode v in val) {
                WriteValue(v);
            }
        }

        private void WriteCompound (TagNodeCompound val)
        {
            foreach (KeyValuePair<string, TagNode> item in val) {
                WriteTag(item.Key, item.Value);
            }

            WriteTag(null, _nulltag);
        }

        private void WriteTag (string name, TagNode val)
        {
            _stream.WriteByte((byte)val.GetTagType());

            if (val.GetTagType() != TagType.TAG_END) {
                WriteString(name);
                WriteValue(val);
            }
        }

        #region ICopyable<NBT_Tree> Members

        /// <summary>
        /// Creates a deep copy of the NBT_Tree and underlying nodes.
        /// </summary>
        /// <returns>A new NBT_tree.</returns>
        public NBT_Tree Copy ()
        {
            NBT_Tree tree = new NBT_Tree();
            tree._root = _root.Copy() as TagNodeCompound;

            return tree;
        }

        #endregion
    }

    // TODO: Revise exceptions?
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
