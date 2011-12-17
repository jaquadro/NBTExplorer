using System;
using System.Collections.Generic;

namespace Substrate.Core
{
    /// <summary>
    /// An LRU-based caching data structure for holding references to <see cref="ChunkRef"/> objects.
    /// </summary>
    /// <remarks>The <see cref="ChunkCache"/> class maintains a separate dictionary on the side for tracking
    /// dirty chunks.  References to dirty chunks will still be held even if the chunk has been evicted from
    /// the normal LRU cache.  The dirty list is reset when the world is saved, or manually cleared.</remarks>
    public class ChunkCache
    {
        private LRUCache<ChunkKey, ChunkRef> _cache;
        private Dictionary<ChunkKey, ChunkRef> _dirty;

        /// <summary>
        /// Creates a new <see cref="ChunkCache"/> with the default capacity of 256 chunks.
        /// </summary>
        public ChunkCache ()
            : this(256)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ChunkCache"/> with the given chunk capacity.
        /// </summary>
        /// <param name="cacheSize">The capacity of the LRU-portion of the cache.</param>
        public ChunkCache (int cacheSize)
        {
            _cache = new LRUCache<ChunkKey, ChunkRef>(cacheSize);
            _dirty = new Dictionary<ChunkKey, ChunkRef>();

            _cache.RemoveCacheValue += EvictionHandler;
        }

        #region IChunkCache Members

        /// <summary>
        /// Inserts a new chunk into the cache.
        /// </summary>
        /// <param name="chunk">The <see cref="ChunkRef"/> to add to the cache.</param>
        /// <returns><c>True</c> if the chunk did not already exist in the cache, <c>false</c> otherwise.</returns>
        /// <remarks>If the chunk does not already exist and the list is full, then the least-recently used chunk in the cache will be evicted 
        /// to make room for the new chunk.  If the chunk is present in the dirty list, it will be removed.</remarks>
        public bool Insert (ChunkRef chunk)
        {
            ChunkKey key = new ChunkKey(chunk.X, chunk.Z);

            _dirty.Remove(key);

            ChunkRef c;
            if (!_cache.TryGetValue(key, out c)) {
                _cache[key] = chunk;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a chunk from the cache.
        /// </summary>
        /// <param name="key">The key identifying which <see cref="ChunkRef"/> to try and remove.</param>
        /// <returns><c>True</c> if the chunk was in the LRU-portion of the cache and removed, <c>False</c> otherwise.</returns>
        /// <remarks>The chunk will also be removed from the dirty list, if it is currently in it.</remarks>
        public bool Remove (ChunkKey key)
        {
            _dirty.Remove(key);
            return _cache.Remove(key);
        }

        /// <summary>
        /// Attempts to get a chunk from the LRU- or dirty-portions of the cache.
        /// </summary>
        /// <param name="key">The key identifying which <see cref="ChunkRef"/> to find.</param>
        /// <returns>The cached <see cref="ChunkRef"/> if it was found anywhere in the cache, or <c>null</c> if it was not found.</returns>
        /// <remarks>If the <see cref="ChunkRef"/> is found in the LRU-portion of the cache, it will be moved to the front of the
        /// LRU list, making future eviction less likely.</remarks>
        public ChunkRef Fetch (ChunkKey key)
        {
            ChunkRef c;
            if (_dirty.TryGetValue(key, out c)) {
                return c;
            }

            if (_cache.TryGetValue(key, out c)) {
                return c;
            }

            return null;
        }

        /// <summary>
        /// Gets an enumerator to iterate over all of the <see cref="ChunkRef"/> objects currently in the dirty list.
        /// </summary>
        /// <returns>An enumerator over all of the dirty <see cref="ChunkRef"/> objects.</returns>
        public IEnumerator<ChunkRef> GetDirtyEnumerator ()
        {
            return _dirty.Values.GetEnumerator();
        }

        /// <summary>
        /// Clears all chunks from the LRU list.
        /// </summary>
        /// <remarks>This method will clear all chunks from the LRU-portion of the cache, including any chunks that are
        /// dirty but have not yet been discovered and added to the dirty list.  Chunks already in the dirty list will
        /// not be affected.  To clear dirty chunks as well, see <see cref="ClearDirty"/>.</remarks>
        public void Clear ()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Clears all chunks from the dirty list.
        /// </summary>
        public void ClearDirty ()
        {
            _dirty.Clear();
        }

        /// <summary>
        /// Scans the LRU list for any dirty chunks, and adds them to the dirty list.
        /// </summary>
        public void SyncDirty ()
        {
            foreach (KeyValuePair<ChunkKey, ChunkRef> e in _cache) {
                if (e.Value.IsDirty) {
                    _dirty[e.Key] = e.Value;
                }
            }
        }


        private void EvictionHandler (object sender, LRUCache<ChunkKey, ChunkRef>.CacheValueArgs e)
        {
            if (e.Value.IsDirty) {
                _dirty[e.Key] = e.Value;
            }
        }

        #endregion
    }
}
