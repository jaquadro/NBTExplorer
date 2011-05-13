using System;
using System.Collections.Generic;
using System.Collections;

using Substrate.Utility;

namespace Substrate
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
            if (!_cache.TryGetValue(key, out c)) {
                return null;
            }

            return c;
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

        /*public bool MarkChunkDirty (ChunkRef chunk)
        {
            int cx = chunk.X;
            int cz = chunk.Z;

            ChunkKey k = new ChunkKey(cx, cz);
            if (!_dirty.ContainsKey(k)) {
                _dirty.Add(k, chunk);
                return true;
            }
            return false;
        }

        public bool MarkChunkClean (ChunkRef chunk)
        {
            int cx = chunk.X;
            int cz = chunk.Z;

            ChunkKey k = new ChunkKey(cx, cz);
            if (_dirty.ContainsKey(k)) {
                _dirty.Remove(k);
                return true;
            }
            return false;
        }*/
        
        #endregion
    }
}
