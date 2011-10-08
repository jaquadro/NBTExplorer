using System;

namespace Substrate.Nbt
{
    /// <summary>
    /// Defines methods for loading or extracting an NBT tree.
    /// </summary>
    /// <typeparam name="T">Object type that supports this interface.</typeparam>
    public interface INbtObject<T>
    {
        /// <summary>
        /// Attempt to load an NBT tree into the object without validation.
        /// </summary>
        /// <param name="tree">The root node of an NBT tree.</param>
        /// <returns>The object returns itself on success, or null if the tree was unparsable.</returns>
        T LoadTree (TagNode tree);

        /// <summary>
        /// Attempt to load an NBT tree into the object with validation.
        /// </summary>
        /// <param name="tree">The root node of an NBT tree.</param>
        /// <returns>The object returns itself on success, or null if the tree failed validation.</returns>
        T LoadTreeSafe (TagNode tree);

        /// <summary>
        /// Builds an NBT tree from the object's data.
        /// </summary>
        /// <returns>The root node of an NBT tree representing the object's data.</returns>
        TagNode BuildTree ();

        /// <summary>
        /// Validate an NBT tree, usually against an object-supplied schema.
        /// </summary>
        /// <param name="tree">The root node of an NBT tree.</param>
        /// <returns>Status indicating whether the tree was valid for this object.</returns>
        bool ValidateTree (TagNode tree);
    }
}
