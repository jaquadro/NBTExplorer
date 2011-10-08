using System;

namespace Substrate.Nbt
{
    /// <summary>
    /// An NBT node representing a null tag type.
    /// </summary>
    public sealed class TagNodeNull : TagNode
    {
        /// <summary>
        /// Converts the node to itself.
        /// </summary>
        /// <returns>A reference to itself.</returns>
        public override TagNodeNull ToTagNull ()
        {
            return this;
        }

        /// <summary>
        /// Gets the tag type of the node.
        /// </summary>
        /// <returns>The TAG_END tag type.</returns>
        public override TagType GetTagType ()
        {
            return TagType.TAG_END;
        }

        /// <summary>
        /// Makes a deep copy of the node.
        /// </summary>
        /// <returns>A new null node.</returns>
        public override TagNode Copy ()
        {
            return new TagNodeNull();
        }
    }
}