using System;

namespace Substrate.Nbt
{
    /// <summary>
    /// An NBT node representing an unsigned byte array tag type.
    /// </summary>
    public sealed class TagNodeByteArray : TagNode
    {
        private byte[] _data = null;

        /// <summary>
        /// Converts the node to itself.
        /// </summary>
        /// <returns>A reference to itself.</returns>
        public override TagNodeByteArray ToTagByteArray () 
        {
            return this;
        }

        /// <summary>
        /// Gets the tag type of the node.
        /// </summary>
        /// <returns>The TAG_BYTE_ARRAY tag type.</returns>
        public override TagType GetTagType ()
        {
            return TagType.TAG_BYTE_ARRAY; 
        }

        /// <summary>
        /// Gets or sets a byte array of tag data.
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Gets the length of the stored byte array.
        /// </summary>
        public int Length
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Constructs a new byte array node with a null data value.
        /// </summary>
        public TagNodeByteArray () { }

        /// <summary>
        /// Constructs a new byte array node.
        /// </summary>
        /// <param name="d">The value to set the node's tag data value.</param>
        public TagNodeByteArray (byte[] d)
        {
            _data = d;
        }

        /// <summary>
        /// Makes a deep copy of the node.
        /// </summary>
        /// <returns>A new byte array node representing the same data.</returns>
        public override TagNode Copy ()
        {
            byte[] arr = new byte[_data.Length];
            _data.CopyTo(arr, 0);

            return new TagNodeByteArray(arr);
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
        /// Gets or sets a single byte at the specified index.
        /// </summary>
        /// <param name="index">Valid index within stored byte array.</param>
        /// <returns>The byte value at the given index of the stored byte array.</returns>
        public byte this[int index]
        {
            get { return _data[index]; }
            set { _data[index] = value; }
        }

        /// <summary>
        /// Converts a system byte array to a byte array node representing the same data.
        /// </summary>
        /// <param name="b">A byte array.</param>
        /// <returns>A new byte array node containing the given value.</returns>
        public static implicit operator TagNodeByteArray (byte[] b)
        {
            return new TagNodeByteArray(b);
        }

        /// <summary>
        /// Converts a byte array node to a system byte array representing the same data.
        /// </summary>
        /// <param name="b">A byte array node.</param>
        /// <returns>A system byte array set to the node's data.</returns>
        public static implicit operator byte[] (TagNodeByteArray b)
        {
            return b._data;
        }
    }
}