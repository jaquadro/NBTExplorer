using System;

namespace Substrate.Nbt
{
    /// <summary>
    /// An NBT node representing a signed long tag type.
    /// </summary>
    public sealed class TagNodeLong : TagNode
    {
        private long _data = 0;

        /// <summary>
        /// Converts the node to itself.
        /// </summary>
        /// <returns>A reference to itself.</returns>
        public override TagNodeLong ToTagLong () 
        {
            return this;
        }

        /// <summary>
        /// Gets the tag type of the node.
        /// </summary>
        /// <returns>The TAG_LONG tag type.</returns>
        public override TagType GetTagType () 
        { 
            return TagType.TAG_LONG;
        }

        /// <summary>
        /// Gets or sets a long of tag data.
        /// </summary>
        public long Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Constructs a new long node with a data value of 0.
        /// </summary>
        public TagNodeLong () { }

        /// <summary>
        /// Constructs a new long node.
        /// </summary>
        /// <param name="d">The value to set the node's tag data value.</param>
        public TagNodeLong (long d)
        {
            _data = d;
        }

        /// <summary>
        /// Makes a deep copy of the node.
        /// </summary>
        /// <returns>A new long node representing the same data.</returns>
        public override TagNode Copy ()
        {
            return new TagNodeLong(_data);
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
        /// Converts a system byte to a long node representing the same value.
        /// </summary>
        /// <param name="b">A byte value.</param>
        /// <returns>A new long node containing the given value.</returns>
        public static implicit operator TagNodeLong (byte b)
        {
            return new TagNodeLong(b);
        }

        /// <summary>
        /// Converts a system shprt to a long node representing the same value.
        /// </summary>
        /// <param name="s">A short value.</param>
        /// <returns>A new long node containing the given value.</returns>
        public static implicit operator TagNodeLong (short s)
        {
            return new TagNodeLong(s);
        }

        /// <summary>
        /// Converts a system int to a long node representing the same value.
        /// </summary>
        /// <param name="i">An int value.</param>
        /// <returns>A new long node containing the given value.</returns>
        public static implicit operator TagNodeLong (int i)
        {
            return new TagNodeLong(i);
        }

        /// <summary>
        /// Converts a system long to a long node representing the same value.
        /// </summary>
        /// <param name="l">A long value.</param>
        /// <returns>A new long node containing the given value.</returns>
        public static implicit operator TagNodeLong (long l)
        {
            return new TagNodeLong(l);
        }

        /// <summary>
        /// Converts a long node to a system long representing the same value.
        /// </summary>
        /// <param name="l">A long node.</param>
        /// <returns>A system long set to the node's data value.</returns>
        public static implicit operator long (TagNodeLong l)
        {
            return l._data;
        }
    }
}