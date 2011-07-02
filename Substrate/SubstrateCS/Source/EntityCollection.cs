using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// Functions to query and manage a collection of entities.
    /// </summary>
    public class EntityCollection : IEnumerable<EntityTyped>
    {
        private TagNodeList _entities;

        private bool _dirty;

        /// <summary>
        /// Gets or sets a value indicating whether this collection contains unsaved changes.
        /// </summary>
        public bool IsDirty
        {
            get { return _dirty; }
            set { _dirty = value; }
        }

        /// <summary>
        /// Creates a new <see cref="EntityCollection"/> around a <see cref="TagNodeList"/> containing Entity nodes.
        /// </summary>
        /// <param name="entities">A <see cref="TagNodeList"/> containing Entity nodes.</param>
        public EntityCollection (TagNodeList entities)
        {
            _entities = entities;
        }

        /// <summary>
        /// Gets a list of all entities in the collection that match a given id (type).
        /// </summary>
        /// <param name="id">The id (type) of entities that should be returned.</param>
        /// <returns>A list of <see cref="EntityTyped"/> objects matching the given id (type).</returns>
        public List<EntityTyped> FindAll (string id)
        {
            List<EntityTyped> set = new List<EntityTyped>();

            foreach (TagNodeCompound ent in _entities) {
                TagNode eid;
                if (!ent.TryGetValue("id", out eid)) {
                    continue;
                }

                if (eid.ToTagString().Data != id) {
                    continue;
                }

                EntityTyped obj = EntityFactory.Create(ent);
                if (obj != null) {
                    set.Add(obj);
                }
            }

            return set;
        }

        /// <summary>
        /// Gets a list of all entities in the collection that match a given condition.
        /// </summary>
        /// <param name="match">A <see cref="Predicate{T}"/> defining the matching condition.</param>
        /// <returns>A list of <see cref="EntityTyped"/> objects matching the given condition.</returns>
        public List<EntityTyped> FindAll (Predicate<EntityTyped> match)
        {
            List<EntityTyped> set = new List<EntityTyped>();

            foreach (TagNodeCompound ent in _entities) {
                EntityTyped obj = EntityFactory.Create(ent);
                if (obj == null) {
                    continue;
                }

                if (match(obj)) {
                    set.Add(obj);
                }
            }

            return set;
        }

        /// <summary>
        /// Adds a <see cref="EntityTyped"/> to the collection.
        /// </summary>
        /// <param name="ent">The <see cref="EntityTyped"/> object to add.</param>
        /// <remarks>It is up to the developer to ensure that the <see cref="EntityTyped"/> being added to the collection has a position that
        /// is within acceptable range of the collection.  <see cref="EntityCollection"/> transparently back other objects such as 
        /// <see cref="Chunk"/> objects, which have a well-defined position in global space.  The <see cref="EntityCollection"/> itself has
        /// no concept of position and will not enforce constraints on the positions of <see cref="EntityTyped"/> objects being added.</remarks>
        public void Add (EntityTyped ent)
        {
            _entities.Add(ent.BuildTree());
            _dirty = true;
        }

        /// <summary>
        /// Removes all entities matching the given id (type) from the collection.
        /// </summary>
        /// <param name="id">The id (type) of entities that should be removed.</param>
        /// <returns>A count of the number of entities that were removed.</returns>
        public int RemoveAll (string id)
        {
            int rem = _entities.RemoveAll(val =>
            {
                TagNodeCompound cval = val as TagNodeCompound;
                if (cval == null) {
                    return false;
                }

                TagNode sval;
                if (!cval.TryGetValue("id", out sval)) {
                    return false;
                }

                return (sval.ToTagString().Data == id);
            });

            if (rem > 0) {
                _dirty = true;
            }
            
            return rem;
        }

        /// <summary>
        /// Removes all entities matching the given condition from the collection.
        /// </summary>
        /// <param name="match">A <see cref="Predicate{T}"/> defining the matching condition.</param>
        /// <returns>A count of the number of entities that were removed.</returns>
        public int RemoveAll (Predicate<EntityTyped> match)
        {
            int rem = _entities.RemoveAll(val =>
            {
                TagNodeCompound cval = val as TagNodeCompound;
                if (cval == null) {
                    return false;
                }

                EntityTyped obj = EntityFactory.Create(cval);
                if (obj == null) {
                    return false;
                }

                return match(obj);
            });

            if (rem > 0) {
                _dirty = true;
            }

            return rem;
        }

        #region IEnumerable<Entity> Members

        /// <summary>
        /// Returns an enumerator that iterates through all entities.
        /// </summary>
        /// <returns>An <see cref="Enumerator"/> for this object.</returns>
        public IEnumerator<EntityTyped> GetEnumerator ()
        {
            return new Enumerator(_entities);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through all entities.
        /// </summary>
        /// <returns>An <see cref="Enumerator"/> for this object.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return new Enumerator(_entities);
        }

        #endregion

        /// <summary>
        /// Enumerates the entities within an <see cref="EntityCollection"/>.
        /// </summary>
        private struct Enumerator : IEnumerator<EntityTyped>
        {
            private IEnumerator<TagNode> _enum;

            private EntityTyped _cur;

            internal Enumerator (TagNodeList entities)
            {
                _enum = entities.GetEnumerator();
                _cur = null;
            }

            #region IEnumerator<Entity> Members

            /// <summary>
            /// Gets the <see cref="EntityTyped"/> at the current position of the enumerator.
            /// </summary>
            public EntityTyped Current
            {
                get 
                {
                    if (_cur == null) {
                        throw new InvalidOperationException();
                    } 
                    return _cur;
                }
            }

            #endregion

            #region IDisposable Members

            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator"/>.
            /// </summary>
            public void Dispose () { }

            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Gets the <see cref="EntityTyped"/> at the current position of the enumerator.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            /// <summary>
            /// Advances the enumerator to the next <see cref="EntityTyped"/> in the <see cref="EntityCollection"/>.
            /// </summary>
            /// <returns>True if the enumerator was successfully advanced to the next position; false if the enumerator advanced past the end of the collection.</returns>
            public bool MoveNext ()
            {
                if (!_enum.MoveNext()) {
                    return false;
                }

                _cur = EntityFactory.Create(_enum.Current.ToTagCompound());
                return true;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first <see cref="EntityTyped"/> in the collection.
            /// </summary>
            void System.Collections.IEnumerator.Reset ()
            {
                _cur = null;
                _enum.Reset();
            }

            #endregion
        }
    }
}
