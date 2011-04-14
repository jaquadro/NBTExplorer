using System;
using System.Collections.Generic;
using System.Collections;

namespace Substrate
{
    public class ChunkCache : IChunkCache
    {
        private Dictionary<ChunkKey, WeakReference> _cache;
        private Dictionary<ChunkKey, ChunkRef> _dirty;

        public ChunkCache ()
        {
            _cache = new Dictionary<ChunkKey, WeakReference>();
            _dirty = new Dictionary<ChunkKey, ChunkRef>();
        }

        #region IChunkCache Members

        public bool Insert (ChunkRef chunk)
        {
            ChunkKey key = new ChunkKey(chunk.X, chunk.Z);

            WeakReference wref;
            if (!_cache.TryGetValue(key, out wref)) {
                _cache[key] = new WeakReference(chunk);
                return true;
            }

            if (!wref.IsAlive) {
                wref.Target = chunk;
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
            WeakReference wref;
            if (!_cache.TryGetValue(key, out wref)) {
                return null;
            }

            return wref.Target as ChunkRef;
        }

        public IEnumerator<ChunkRef> GetDirtyEnumerator ()
        {
            return _dirty.Values.GetEnumerator();
        }

        public void ClearDirty ()
        {
            _dirty.Clear();
        }

        public bool MarkChunkDirty (ChunkRef chunk)
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
        }

        #endregion
    }
}
