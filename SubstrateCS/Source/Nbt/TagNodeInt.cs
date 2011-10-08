using System;

namespace Substrate.Nbt
{
    /// <summary>
    /// An NBT node representing a signed int tag type.
    /// </summary>
    public sealed class TagNodeInt : TagNode
    {
        private int _data = 0;

        /// <summary>
        /// Converts the node to itself.
        /// </summary>
        /// <returns>A reference to itself.</returns>
        public override TagNodeInt ToTagInt () 
        { 
            return this;
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
        /// <returns>The TAG_INT tag type.</returns>
        public override TagType GetTagType ()
        {
            return TagType.TAG_INT;
        }

        /// <summary>
        /// Checks if the node is castable to another node of a given tag type.
        /// </summary>
        /// <param name="type">An NBT tag type.</param>
        /// <returns>Status indicating whether this object could be cast to a node type represented by the given tag type.</returns>
        public override bool IsCastableTo (TagType type)
        {
            return (type == TagType.TAG_INT ||
                type == TagType.TAG_LONG);
        }

        /// <summary>
        /// Gets or sets an int of tag data.
        /// </summary>
        public int Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Constructs a new int node with a data value of 0.
        /// </summary>
        public TagNodeInt () { }

        /// <summary>
        /// Constructs a new int node.
        /// </summary>
        /// <param name="d">The value to set the node's tag data value.</param>
        public TagNodeInt (int d)
        {
            _data = d;
        }

        /// <summary>
        /// Makes a deep copy of the node.
        /// </summary>
        /// <returns>A new int node representing the same data.</returns>
        public override TagNode Copy ()
        {
            return new TagNodeInt(_data);
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
        /// Converts a system byte to an int node representing the same value.
        /// </summary>
        /// <param name="b">A byte value.</param>
        /// <returns>A new int node containing the given value.</returns>
        public static implicit operator TagNodeInt (byte b)
        {
            return new TagNodeInt(b);
        }

        /// <summary>
        /// Converts a system short to an int node representing the same value.
        /// </summary>
        /// <param name="s">A short value.</param>
        /// <returns>A new int node containing the given value.</returns>
        public static implicit operator TagNodeInt (short s)
        {
            return new TagNodeInt(s);
        }

        /// <summary>
        /// Converts a system int to an int node representing the same value.
        /// </summary>
        /// <param name="i">An int value.</param>
        /// <returns>A new int node containing the given value.</returns>
        public static implicit operator TagNodeInt (int i)
        {
            return new TagNodeInt(i);
        }

        /// <summary>
        /// Converts an int node to a system int representing the same value.
        /// </summary>
        /// <param name="i">An int node.</param>
        /// <returns>A system int set to the node's data value.</returns>
        public static implicit operator int (TagNodeInt i)
        {
            return i._data;
        }

        /// <summary>
        /// Converts an int node to a system long representing the same value.
        /// </summary>
        /// <param name="i">An int node.</param>
        /// <returns>A system long set to the node's data value.</returns>
        public static implicit operator long (TagNodeInt i)
        {
            return i._data;
        }
    }
}