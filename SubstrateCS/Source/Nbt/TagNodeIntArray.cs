using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Nbt
{
    public sealed class TagNodeIntArray : TagNode
    {
        private int[] _data = null;

        /// <summary>
        /// Converts the node to itself.
        /// </summary>
        /// <returns>A reference to itself.</returns>
        public override TagNodeIntArray ToTagIntArray () 
        {
            return this;
        }

        /// <summary>
        /// Gets the tag type of the node.
        /// </summary>
        /// <returns>The TAG_INT_ARRAY tag type.</returns>
        public override TagType GetTagType ()
        {
            return TagType.TAG_INT_ARRAY; 
        }

        /// <summary>
        /// Gets or sets an int array of tag data.
        /// </summary>
        public int[] Data
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
        public TagNodeIntArray () { }

        /// <summary>
        /// Constructs a new byte array node.
        /// </summary>
        /// <param name="d">The value to set the node's tag data value.</param>
        public TagNodeIntArray (int[] d)
        {
            _data = d;
        }

        /// <summary>
        /// Makes a deep copy of the node.
        /// </summary>
        /// <returns>A new int array node representing the same data.</returns>
        public override TagNode Copy ()
        {
            int[] arr = new int[_data.Length];
            _data.CopyTo(arr, 0);

            return new TagNodeIntArray(arr);
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
        /// Gets or sets a single int at the specified index.
        /// </summary>
        /// <param name="index">Valid index within stored int array.</param>
        /// <returns>The int value at the given index of the stored byte array.</returns>
        public int this[int index]
        {
            get { return _data[index]; }
            set { _data[index] = value; }
        }

        /// <summary>
        /// Converts a system int array to a int array node representing the same data.
        /// </summary>
        /// <param name="i">A int array.</param>
        /// <returns>A new int array node containing the given value.</returns>
        public static implicit operator TagNodeIntArray (int[] i)
        {
            return new TagNodeIntArray(i);
        }

        /// <summary>
        /// Converts an int array node to a system int array representing the same data.
        /// </summary>
        /// <param name="i">An int array node.</param>
        /// <returns>A system int array set to the node's data.</returns>
        public static implicit operator int[] (TagNodeIntArray i)
        {
            return i._data;
        }
    }
}
