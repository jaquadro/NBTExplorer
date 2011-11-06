using System;
using System.Collections;
using System.Collections.Generic;

namespace Substrate.Nbt
{
    /// <summary>
    /// An NBT node representing a compound tag type containing other nodes.
    /// </summary>
    public sealed class TagNodeCompound : TagNode, IDictionary<string, TagNode>
    {
        private Dictionary<string, TagNode> _tags;

        /// <summary>
        /// Converts the node to itself.
        /// </summary>
        /// <returns>A reference to itself.</returns>
        public override TagNodeCompound ToTagCompound () 
        { 
            return this; 
        }

        /// <summary>
        /// Gets the tag type of the node.
        /// </summary>
        /// <returns>The TAG_STRING tag type.</returns>
        public override TagType GetTagType ()
        { 
            return TagType.TAG_COMPOUND;
        }

        /// <summary>
        /// Gets the number of subnodes contained in node.
        /// </summary>
        public int Count
        {
            get { return _tags.Count; }
        }

        /// <summary>
        /// Constructs a new empty compound node.
        /// </summary>
        public TagNodeCompound ()
        {
            _tags = new Dictionary<string, TagNode>();
        }

        /// <summary>
        /// Copies all the elements of <paramref name="tree"/> into this <see cref="TagNodeCompound"/> if they do not already exist.
        /// </summary>
        /// <param name="tree">The source <see cref="TagNodeCompound"/> to copy elements from.</param>
        public void MergeFrom (TagNodeCompound tree)
        {
            foreach (KeyValuePair<string, TagNode> node in tree) {
                if (_tags.ContainsKey(node.Key)) {
                    continue;
                }

                _tags.Add(node.Key, node.Value);
            }
        }

        /// <summary>
        /// Makes a deep copy of the node.
        /// </summary>
        /// <returns>A new compound node containing new subnodes representing the same data.</returns>
        public override TagNode Copy ()
        {
            TagNodeCompound list = new TagNodeCompound();
            foreach (KeyValuePair<string, TagNode> item in _tags) {
                list[item.Key] = item.Value.Copy();
            }
            return list;
        }

        /// <summary>
        /// Gets a string representation of the node's data.
        /// </summary>
        /// <returns>String representation of the node's data.</returns>
        public override string ToString ()
        {
            return _tags.ToString();
        }

        #region IDictionary<string,NBT_Value> Members

        /// <summary>
        /// Adds a named subnode to the set.
        /// </summary>
        /// <param name="key">The name of the subnode.</param>
        /// <param name="value">The subnode to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException">A subnode with the same key already exists in the set.</exception>
        public void Add (string key, TagNode value)
        {
            _tags.Add(key, value);
        }

        /// <summary>
        /// Checks if a subnode exists in the set with the specified name.
        /// </summary>
        /// <param name="key">The name of a subnode to check.</param>
        /// <returns>Status indicating whether a subnode with the specified name exists.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey (string key)
        {
            return _tags.ContainsKey(key);
        }

        /// <summary>
        /// Gets a collection containing all the names of subnodes in this set.
        /// </summary>
        public ICollection<string> Keys
        {
            get { return _tags.Keys; }
        }

        /// <summary>
        /// Removes a subnode with the specified name.
        /// </summary>
        /// <param name="key">The name of the subnode to remove.</param>
        /// <returns>Status indicating whether a subnode was removed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool Remove (string key)
        {
            return _tags.Remove(key);
        }

        /// <summary>
        /// Gets the subnode associated with the given name.
        /// </summary>
        /// <param name="key">The name of the subnode to get.</param>
        /// <param name="value">When the function returns, contains the subnode assicated with the specified key.  If no subnode was found, contains a default value.</param>
        /// <returns>Status indicating whether a subnode was found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool TryGetValue (string key, out TagNode value)
        {
            return _tags.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets a collection containing all the subnodes in this set.
        /// </summary>
        public ICollection<TagNode> Values
        {
            get { return _tags.Values; }
        }

        /// <summary>
        /// Gets or sets the subnode with the associated name.
        /// </summary>
        /// <param name="key">The name of the subnode to get or set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
        public TagNode this[string key]
        {
            get
            {
                return _tags[key];
            }
            set
            {
                _tags[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,NBT_Value>> Members

        /// <summary>
        /// Adds a subnode to the to the set with the specified name.
        /// </summary>
        /// <param name="item">The <see cref="KeyValuePair{TKey, TVal}"/> structure representing the key and subnode to add to the set.</param>
        /// <exception cref="ArgumentNullException">The key of <paramref name="item"/> is null.</exception>
        /// <exception cref="ArgumentException">A subnode with the same key already exists in the set.</exception>
        public void Add (KeyValuePair<string, TagNode> item)
        {
            _tags.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all of the subnodes from this node.
        /// </summary>
        public void Clear ()
        {
            _tags.Clear();
        }

        /// <summary>
        /// Checks if a specific subnode with a specific name is contained in the set.
        /// </summary>
        /// <param name="item">The <see cref="KeyValuePair{TKey, TValue}"/> structure representing the key and subnode to look for.</param>
        /// <returns>Status indicating if the subnode and key combination exists in the set.</returns>
        public bool Contains (KeyValuePair<string, TagNode> item)
        {
            TagNode value;
            if (!_tags.TryGetValue(item.Key, out value)) {
                return false;
            }
            return value == item.Value;
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an array of type <see cref="KeyValuePair{TKey, TVal}"/>, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the subnodes copied. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ICollection{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo (KeyValuePair<string, TagNode>[] array, int arrayIndex)
        {
            if (array == null) {
                throw new ArgumentNullException();
            }
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException();
            }
            if (array.Length - arrayIndex < _tags.Count) {
                throw new ArgumentException();
            }

            foreach (KeyValuePair<string, TagNode> item in _tags) {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the node is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the specified key and subnode combination from the set.
        /// </summary>
        /// <param name="item">The <see cref="KeyValuePair{TKey, TVal}"/> structure representing the key and value to remove from the set.</param>
        /// <returns>Status indicating whether a subnode was removed.</returns>
        public bool Remove (KeyValuePair<string, TagNode> item)
        {
            if (Contains(item)) {
                _tags.Remove(item.Key);
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,NBT_Value>> Members

        /// <summary>
        /// Returns an enumerator that iterates through all of the subnodes in the set.
        /// </summary>
        /// <returns>An enumerator for this node.</returns>
        public IEnumerator<KeyValuePair<string, TagNode>> GetEnumerator ()
        {
            return _tags.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through all of the subnodes in the set.
        /// </summary>
        /// <returns>An enumerator for this node.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return _tags.GetEnumerator();
        }

        #endregion
    }
}