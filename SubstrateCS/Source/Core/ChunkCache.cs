using System;
using System.Collections.Generic;

namespace Substrate.Core
{
    public class ChunkCache
    {
        private LRUCache<ChunkKey, ChunkRef> _cache;
        private Dictionary<ChunkKey, ChunkRef> _dirty;

        public ChunkCache ()
        {
            _cache = new LRUCache<ChunkKey, ChunkRef>(256);
            _dirty = new Dictionary<ChunkKey, ChunkRef>();

            _cache.RemoveCacheValue += EvictionHandler;
        }

        #region IChunkCache Members

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

        public bool Remove (ChunkKey key)
        {
            _dirty.Remove(key);
            return _cache.Remove(key);
        }

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

        public IEnumerator<ChunkRef> GetDirtyEnumerator ()
        {
            return _dirty.Values.GetEnumerator();
        }

        public void ClearDirty ()
        {
            _dirty.Clear();
        }

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
