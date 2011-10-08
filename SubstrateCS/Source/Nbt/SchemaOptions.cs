using System;

namespace Substrate.Nbt
{
    /// <summary>
    /// Additional options that modify the processing of a <see cref="SchemaNode"/>.
    /// </summary>
    [Flags]
    public enum SchemaOptions
    {
        /// <summary>
        /// Any <see cref="SchemaNode"/> with this option will not throw an error if the corresponding <see cref="TagNode"/> is missing.
        /// </summary>
        OPTIONAL = 0x1,

        /// <summary>
        /// If a <see cref="TagNode"/> cannot be found for a <see cref="SchemaNode"/> marked with this option, a sensible default <see cref="TagNode"/> will be created and inserted into the tree.
        /// </summary>
        CREATE_ON_MISSING = 0x2,
    }
}
