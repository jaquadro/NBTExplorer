using System;

namespace Substrate.Nbt
{
    /// <summary>
    /// An NBT node representing a signed byte tag type.
    /// </summary>
    public sealed class TagNodeByte : TagNode
    {
        private byte _data = 0;

        /// <summary>
        /// Converts the node to itself.
        /// </summary>
        /// <returns>A reference to itself.</returns>
        public override TagNodeByte ToTagByte ()
        {
            return this;
        }

        /// <summary>
        /// Converts the node to a new short node.
        /// </summary>
        /// <returns>A short node representing the same data.</returns>
        public override TagNodeShort ToTagShort ()
        {
            return new TagNodeShort(_data);
        }

        /// <summary>
        /// Converts the node to a new int node.
        /// </summary>
        /// <returns>An int node representing the same data.</returns>
        public override TagNodeInt ToTagInt ()
        {
            return new TagNodeInt(_data);
        }

        /// <summary>
        /// Converts the node to a new long node.
        /// </summary>
        /// <returns>A long node representing the same data.</returns>
        public override TagNodeLong ToTagLong ()
        {
            return new TagNodeLong(_data);
        }

        /// <summary>
        /// Gets the tag type of the node.
        /// </summary>
        /// <returns>The TAG_BYTE tag type.</returns>
        public override TagType GetTagType ()
        {
            return TagType.TAG_BYTE;
        }

        /// <summary>
        /// Checks if the node is castable to another node of a given tag type.
        /// </summary>
        /// <param name="type">An NBT tag type.</param>
        /// <returns>Status indicating whether this object could be cast to a node type represented by the given tag type.</returns>
        public override bool IsCastableTo (TagType type)
        {
            return (type == TagType.TAG_BYTE ||
                type == TagType.TAG_SHORT ||
                type == TagType.TAG_INT ||
                type == TagType.TAG_LONG);
        }

        /// <summary>
        /// Gets or sets a byte of tag data.
        /// </summary>
        public byte Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Constructs a new byte node with a data value of 0.
        /// </summary>
        public TagNodeByte () { }

        /// <summary>
        /// Constructs a new byte node.
        /// </summary>
        /// <param name="d">The value to set the node's tag data value.</param>
        public TagNodeByte (byte d)
        {
            _data = d;
        }

        /// <summary>
        /// Makes a deep copy of the node.
        /// </summary>
        /// <returns>A new byte node representing the same data.</returns>
        public override TagNode Copy ()
        {
            return new TagNodeByte(_data);
        }

        /// <summary>
        /// Gets a string representation of the node's data.
        /// </summary>
        /// <returns>String representation of the node's data.</returns>
        public override string ToString ()
        {
            return _data.ToString();
        }

        /// <summary>
        /// Converts a system byte to a byte node representing the same value.
        /// </summary>
        /// <param name="b">A byte value.</param>
        /// <returns>A new byte node containing the given value.</returns>
        public static implicit operator TagNodeByte (byte b)
        {
            return new TagNodeByte(b);
        }

        /// <summary>
        /// Converts a byte node to a system byte representing the same value.
        /// </summary>
        /// <param name="b">A byte node.</param>
        /// <returns>A system byte set to the node's data value.</returns>
        public static implicit operator byte (TagNodeByte b)
        {
            return b._data;
        }

        /// <summary>
        /// Converts a byte node to a system short representing the same value.
        /// </summary>
        /// <param name="b">A byte node.</param>
        /// <returns>A system short set to the node's data value.</returns>
        public static implicit operator short (TagNodeByte b)
        {
            return b._data;
        }

        /// <summary>
        /// Converts a byte node to a system int representing the same value.
        /// </summary>
        /// <param name="b">A byte node.</param>
        /// <returns>A system int set to the node's data value.</returns>
        public static implicit operator int (TagNodeByte b)
        {
            return b._data;
        }

        /// <summary>
        /// Converts a byte node to a system long representing the same value.
        /// </summary>
        /// <param name="b">A byte node.</param>
        /// <returns>A system long set to the node's data value.</returns>
        public static implicit operator long (TagNodeByte b)
        {
            return b._data;
        }
    }
}