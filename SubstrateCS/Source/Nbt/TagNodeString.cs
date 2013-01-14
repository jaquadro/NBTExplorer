using System;

namespace Substrate.Nbt
{
    /// <summary>
    /// An NBT node representing a string tag type.
    /// </summary>
    public sealed class TagNodeString : TagNode
    {
        private string _data = "";

        /// <summary>
        /// Converts the node to itself.
        /// </summary>
        /// <returns>A reference to itself.</returns>
        public override TagNodeString ToTagString () 
        {
            return this; 
        }

        /// <summary>
        /// Gets the tag type of the node.
        /// </summary>
        /// <returns>The TAG_STRING tag type.</returns>
        public override TagType GetTagType () 
        { 
            return TagType.TAG_STRING;
        }

        /// <summary>
        /// Gets or sets a string of tag data.
        /// </summary>
        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Gets the length of the stored string.
        /// </summary>
        public int Length
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Constructs a new byte array node with an empty string.
        /// </summary>
        public TagNodeString () { }

        /// <summary>
        /// Constructs a new string node.
        /// </summary>
        /// <param name="d">The value to set the node's tag data value.</param>
        public TagNodeString (string d)
        {
            _data = d;
            if (_data == null)
                _data = "";
        }

        /// <summary>
        /// Makes a deep copy of the node.
        /// </summary>
        /// <returns>A new string node representing the same data.</returns>
        public override TagNode Copy ()
        {
            return new TagNodeString(_data);
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
        /// Converts a system string to a string node representing the same data.
        /// </summary>
        /// <param name="s">A string.</param>
        /// <returns>A new string node containing the given value.</returns>
        public static implicit operator TagNodeString (string s)
        {
            return new TagNodeString(s);
        }

        /// <summary>
        /// Converts a string node to a system string representing the same data.
        /// </summary>
        /// <param name="s">A string node.</param>
        /// <returns>A system string set to the node's data.</returns>
        public static implicit operator string (TagNodeString s)
        {
            return s._data;
        }
    }
}