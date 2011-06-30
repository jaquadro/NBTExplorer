using System;

namespace Substrate.Nbt
{
    /// <summary>
    /// Defines the type of an NBT tag.
    /// </summary>
    public enum TagType
    {
        /// <summary>
        /// A null tag, used to terminate lists.
        /// </summary>
        TAG_END = 0,

        /// <summary>
        /// A tag containing an 8-bit signed integer.
        /// </summary>
        TAG_BYTE = 1,

        /// <summary>
        /// A tag containing a 16-bit signed integer.
        /// </summary>
        TAG_SHORT = 2,

        /// <summary>
        /// A tag containing a 32-bit signed integer.
        /// </summary>
        TAG_INT = 3,

        /// <summary>
        /// A tag containing a 64-bit signed integer.
        /// </summary>
        TAG_LONG = 4,

        /// <summary>
        /// A tag containing a 32-bit (single precision) floating-point value.
        /// </summary>
        TAG_FLOAT = 5,

        /// <summary>
        /// A tag containing a 64-bit (double precision) floating-point value.
        /// </summary>
        TAG_DOUBLE = 6,

        /// <summary>
        /// A tag containing an array of unsigned 8-bit byte values.
        /// </summary>
        TAG_BYTE_ARRAY = 7,

        /// <summary>
        /// A tag containing a string of text.
        /// </summary>
        TAG_STRING = 8,

        /// <summary>
        /// A tag containing a sequential list of tags, where all tags of of the same type.
        /// </summary>
        TAG_LIST = 9,

        /// <summary>
        /// A tag containing a key-value store of tags, where each tag can be of any type.
        /// </summary>
        TAG_COMPOUND = 10
    }
}