using System;

namespace Substrate.Nbt
{
    /// <summary>
    /// A concrete <see cref="SchemaNode"/> representing a <see cref="TagNodeByteArray"/>.
    /// </summary>
    public sealed class SchemaNodeArray : SchemaNode
    {
        private int _length;

        /// <summary>
        /// Gets the expected length of the corresponding byte array.
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Indicates whether there is an expected length of the corresponding byte array.
        /// </summary>
        public bool HasExpectedLength
        {
            get { return _length > 0; }
        }

        /// <summary>
        /// Constructs a new <see cref="SchemaNodeArray"/> representing a <see cref="TagNodeByteArray"/> named <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the corresponding <see cref="TagNodeByteArray"/>.</param>
        public SchemaNodeArray (string name)
            : base(name)
        {
            _length = 0;
        }

        /// <summary>
        /// Constructs a new <see cref="SchemaNodeArray"/> with additional options.
        /// </summary>
        /// <param name="name">The name of the corresponding <see cref="TagNodeByteArray"/>.</param>
        /// <param name="options">One or more option flags modifying the processing of this node.</param>
        public SchemaNodeArray (string name, SchemaOptions options)
            : base(name, options)
        {
            _length = 0;
        }

        /// <summary>
        /// Constructs a new <see cref="SchemaNodeArray"/> representing a <see cref="TagNodeByteArray"/> named <paramref name="name"/> with expected length <paramref name="length"/>.
        /// </summary>
        /// <param name="name">The name of the corresponding <see cref="TagNodeByteArray"/>.</param>
        /// <param name="length">The expected length of corresponding byte array.</param>
        public SchemaNodeArray (string name, int length)
            : base(name)
        {
            _length = length;
        }

        /// <summary>
        /// Constructs a new <see cref="SchemaNodeArray"/> with additional options.
        /// </summary>
        /// <param name="name">The name of the corresponding <see cref="TagNodeByteArray"/>.</param>
        /// <param name="length">The expected length of corresponding byte array.</param>
        /// <param name="options">One or more option flags modifying the processing of this node.</param>
        public SchemaNodeArray (string name, int length, SchemaOptions options)
            : base(name, options)
        {
            _length = length;
        }

        /// <summary>
        /// Constructs a default <see cref="TagNodeByteArray"/> satisfying the constraints of this node.
        /// </summary>
        /// <returns>A <see cref="TagNodeString"/> with a sensible default value.</returns>
        public override TagNode BuildDefaultTree ()
        {
            return new TagNodeByteArray(new byte[_length]);
        }
    }
}
