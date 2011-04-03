using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    public interface IChunkManager
    {
        Chunk GetChunk (int cx, int cz);
        ChunkRef GetChunkRef (int cx, int cz);

        bool ChunkExists (int cx, int cz);

        bool MarkChunkDirty (int cx, int cz);
        bool MarkChunkClean (int cx, int cz);

        int Save ();
        bool Save (Chunk chunk);

        ChunkRef CreateChunk (int cx, int cz);
        bool DeleteChunk (int cx, int cz);

        bool CopyChunk (int cx1, int cz1, int cx2, int cz2);
        bool MoveChunk (int cx1, int cz1, int cx2, int cz2);
    }

    public class ChunkManager
    {
        public const int REGION_XLEN = 32;
        public const int REGION_ZLEN = 32;

        public const int REGION_XLOG = 5;
        public const int REGION_ZLOG = 5;

        public const int REGION_XMASK = 0x1F;
        public const int REGION_ZMASK = 0x1F;

        protected RegionManager _regionMan;

        protected Dictionary<ChunkKey, WeakReference> _cache;
        protected Dictionary<ChunkKey, ChunkRef> _dirty;

        public ChunkManager (RegionManager rm)
        {
            _regionMan = rm;
            _cache = new Dictionary<ChunkKey, WeakReference>();
            _dirty = new Dictionary<ChunkKey, ChunkRef>();
        }

        public Chunk GetChunk (int cx, int cz)
        {
            int lcx = cx & REGION_XMASK;
            int lcz = cz & REGION_ZMASK;

            Region r = GetRegion(cx, cz);
            if (r == null || !r.ChunkExists(lcx, lcz)) {
                return null;
            }

            return new Chunk(r.GetChunkTree(lcx, lcz));
        }

        public ChunkRef GetChunkRef (int cx, int cz)
        {
            ChunkKey k = new ChunkKey(cx, cz);

            ChunkRef c = null;

            WeakReference chunkref = null;
            if (_cache.TryGetValue(k, out chunkref)) {
                c = chunkref.Target as ChunkRef;
            }
            else {
                _cache.Add(k, new WeakReference(null));
            }

            if (c != null) {
                return c;
            }

            try {
                c = new ChunkRef(this, cx, cz);
                _cache[k].Target = c;
                return c;
            }
            catch (MissingChunkException) {
                return null;
            }
        }

        public bool ChunkExists (int cx, int cz)
        {
            Region r = GetRegion(cx, cz);
            if (r == null) {
                return false;
            }

            return r.ChunkExists(cx & REGION_XMASK, cz & REGION_ZMASK);
        }

        public bool MarkChunkDirty (int cx, int cz)
        {
            ChunkKey k = new ChunkKey(cx, cz);
            if (!_dirty.ContainsKey(k)) {
                _dirty.Add(k, GetChunkRef(cx, cz));
                return true;
            }
            return false;
        }

        public bool MarkChunkClean (int cx, int cz)
        {
            ChunkKey k = new ChunkKey(cx, cz);
            if (_dirty.ContainsKey(k)) {
                _dirty.Remove(k);
                return true;
            }
            return false;
        }

        public int Save ()
        {
            int saved = 0;
            foreach (ChunkRef c in _dirty.Values) {
                int lcx = ChunkLocalX(c.X);
                int lcz = ChunkLocalZ(c.Z);

                Region r = GetRegion(c.X, c.Z);
                if (r == null || !r.ChunkExists(lcx, lcz)) {
                    throw new MissingChunkException();
                }

                if (c.Save(r.GetChunkOutStream(lcx, lcz))) {
                    saved++;
                }
            }

            _dirty.Clear();
            return saved;
        }

        public bool Save (Chunk chunk)
        {
            Region r = GetRegion(chunk.X, chunk.Z);
            if (r == null) {
                return false;
            }

            return chunk.Save(r.GetChunkOutStream(chunk.X & REGION_XLEN, chunk.Z & REGION_ZLEN));
        }

        public bool DeleteChunk (int cx, int cz)
        {
            Region r = GetRegion(cx, cz);
            if (r == null) {
                return false;
            }

            if (!r.DeleteChunk(cx & REGION_XMASK, cz & REGION_ZMASK)) {
                return false;
            }

            ChunkKey k = new ChunkKey(cx, cz);
            _cache.Remove(k);
            _dirty.Remove(k);

            if (r.ChunkCount() == 0) {
                _regionMan.DeleteRegion(r.X, r.Z);
            }

            return true;
        }

        public RegionManager GetRegionManager ()
        {
            return _regionMan;
        }

        public ChunkRef GetChunkRefInRegion (Region r, int lcx, int lcz)
        {
            int cx = r.X * REGION_XLEN + lcx;
            int cz = r.Z * REGION_ZLEN + lcz;
            return GetChunkRef(cx, cz);
        }

        protected int ChunkLocalX (int cx)
        {
            return cx & REGION_XMASK;
        }

        protected int ChunkLocalZ (int cz)
        {
            return cz & REGION_ZMASK;
        }

        protected Region GetRegion (int cx, int cz)
        {
            cx >>= REGION_XLOG;
            cz >>= REGION_ZLOG;
            return _regionMan.GetRegion(cx, cz);
        }
    }

    public class MissingChunkException : Exception
    {

    }
}
