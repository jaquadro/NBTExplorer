using System;
using System.Collections.Generic;

namespace Substrate.Nbt
{
    /// <summary>
    /// A concrete <see cref="SchemaNode"/> representing a <see cref="TagNodeCompound"/>.
    /// </summary>
    public sealed class SchemaNodeCompound : SchemaNode, ICollection<SchemaNode>
    {
        private List<SchemaNode> _subnodes;

        #region ICollection<NBTSchemaNode> Members

        /// <summary>
        /// Adds a <see cref="SchemaNode"/> as a child of this node.
        /// </summary>
        /// <param name="item">The <see cref="SchemaNode"/> to add.</param>
        public void Add (SchemaNode item)
        {
            _subnodes.Add(item);
        }

        /// <summary>
        /// Removes all <see cref="SchemaNode"/> objects from the node.
        /// </summary>
        public void Clear ()
        {
            _subnodes.Clear();
        }

        /// <summary>
        /// Checks if a <see cref="SchemaNode"/> is a child of this node.
        /// </summary>
        /// <param name="item">The <see cref="SchemaNode"/> to check for existance.</param>
        /// <returns>Status indicating if the <see cref="SchemaNode"/> exists as a child of this node.</returns>
        public bool Contains (SchemaNode item)
        {
            return _subnodes.Contains(item);
        }

        /// <summary>
        /// Copies all child <see cref="SchemaNode"/> objects of this node to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the subnodes copied. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo (SchemaNode[] array, int arrayIndex)
        {
            _subnodes.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of child <see cref="SchemaNode"/> objects in this node.
        /// </summary>
        public int Count
        {
            get { return _subnodes.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the node is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurance of a <see cref="SchemaNode"/> from this node.
        /// </summary>
        /// <param name="item">The <see cref="SchemaNode"/> to remove.</param>
        /// <returns>Status indicating whether a <see cref="SchemaNode"/> was removed.</returns>
        public bool Remove (SchemaNode item)
        {
            return _subnodes.Remove(item);
        }

        #endregion

        #region IEnumerable<SchemaNode> Members

        /// <summary>
        /// Iterates through all of the <see cref="SchemaNode"/> objects in this <see cref="SchemaNodeCompound"/>.
        /// </summary>
        /// <returns>An enumerator for this node.</returns>
        public IEnumerator<SchemaNode> GetEnumerator ()
        {
            return _subnodes.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Iterates through all of the <see cref="SchemaNode"/> objects in this <see cref="SchemaNodeCompound"/>.
        /// </summary>
        /// <returns>An enumerator for this node.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return _subnodes.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Constructs a new <see cref="SchemaNodeCompound"/> representing a root <see cref="TagNodeCompound"/>.
        /// </summary>
        public SchemaNodeCompound ()
            : base("")
        {
            _subnodes = new List<SchemaNode>();
        }

        /// <summary>
        /// Constructs a new <see cref="SchemaNodeCompound"/> with additional options.
        /// </summary>
        /// <param name="options">One or more option flags modifying the processing of this node.</param>
        public SchemaNodeCompound (SchemaOptions options)
            : base("", options)
        {
            _subnodes = new List<SchemaNode>();
        }

        /// <summary>
        /// Constructs a new <see cref="SchemaNodeCompound"/> representing a <see cref="TagNodeCompound"/> named <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the corresponding <see cref="TagNodeCompound"/>.</param>
        public SchemaNodeCompound (string name)
            : base(name)
        {
            _subnodes = new List<SchemaNode>();
        }

        /// <summary>
        /// Constructs a new <see cref="SchemaNodeCompound"/> with additional options.
        /// </summary>
        /// <param name="name">The name of the corresponding <see cref="TagNodeCompound"/>.</param>
        /// <param name="options">One or more option flags modifying the processing of this node.</param>
        public SchemaNodeCompound (string name, SchemaOptions options)
            : base(name, options)
        {
            _subnodes = new List<SchemaNode>();
        }

        /// <summary>
        /// Constructs a new <see cref="SchemaNodeCompound"/> representing a <see cref="TagNodeCompound"/> named <paramref name="name"/> matching the given schema.
        /// </summary>
        /// <param name="name">The name of the corresponding <see cref="TagNodeCompound"/>.</param>
        /// <param name="subschema">A <see cref="SchemaNodeCompound"/> representing a schema to verify against the corresponding <see cref="TagNodeCompound"/>.</param>
        public SchemaNodeCompound (string name, SchemaNode subschema)
            : base(name)
        {
            _subnodes = new List<SchemaNode>();

            SchemaNodeCompound schema = subschema as SchemaNodeCompound;
            if (schema == null) {
                return;
            }

            foreach (SchemaNode node in schema._subnodes) {
                _subnodes.Add(node);
            }
        }

        /// <summary>
        /// Constructs a new <see cref="SchemaNodeCompound"/> with additional options.
        /// </summary>
        /// <param name="name">The name of the corresponding <see cref="TagNodeCompound"/>.</param>
        /// <param name="subschema">A <see cref="SchemaNodeCompound"/> representing a schema to verify against the corresponding <see cref="TagNodeCompound"/>.</param>
        /// <param name="options">One or more option flags modifying the processing of this node.</param>
        public SchemaNodeCompound (string name, SchemaNode subschema, SchemaOptions options)
            : base(name, options)
        {
            _subnodes = new List<SchemaNode>();

            SchemaNodeCompound schema = subschema as SchemaNodeCompound;
            if (schema == null) {
                return;
            }

            foreach (SchemaNode node in schema._subnodes) {
                _subnodes.Add(node);
            }
        }

        /// <summary>
        /// Copies all the elements of this <see cref="SchemaNodeCompound"/> into <paramref name="tree"/>.
        /// </summary>
        /// <param name="tree">The destination <see cref="SchemaNodeCompound"/> to copy <see cref="SchemaNode"/> elements into.</param>
        /// <returns>A reference to <paramref name="tree"/>.</returns>
        public SchemaNodeCompound MergeInto (SchemaNodeCompound tree)
        {
            foreach (SchemaNode node in _subnodes) {
                SchemaNode f = tree._subnodes.Find(n => n.Name == node.Name);
                if (f != null) {
                    continue;
                }
                tree.Add(node);
            }

            return tree;
        }

        /// <summary>
        /// Constructs a default <see cref="TagNodeCompound"/> satisfying the constraints of this node.
        /// </summary>
        /// <returns>A <see cref="TagNodeCompound"/> with a sensible default value.  A default child <see cref="TagNode"/> is created for every <see cref="SchemaNode"/> contained in this <see cref="SchemaNodeCompound"/>.</returns>
        public override TagNode BuildDefaultTree ()
        {
            TagNodeCompound list = new TagNodeCompound();
            foreach (SchemaNode node in _subnodes) {
                list[node.Name] = node.BuildDefaultTree();
            }

            return list;
        }
    }
}
