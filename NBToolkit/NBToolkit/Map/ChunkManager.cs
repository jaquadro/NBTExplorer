using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{

    public class ChunkManager : IChunkContainer, IChunkCache
    {
        public const int REGION_XLEN = 32;
        public const int REGION_ZLEN = 32;

        public const int REGION_XLOG = 5;
        public const int REGION_ZLOG = 5;

        public const int REGION_XMASK = 0x1F;
        public const int REGION_ZMASK = 0x1F;

        protected RegionManager _regionMan;

        protected Dictionary<RegionKey, Region> _cache;
        protected Dictionary<RegionKey, Region> _dirty;

        public ChunkManager (RegionManager rm)
        {
            _regionMan = rm;
            _cache = new Dictionary<RegionKey, Region>();
            _dirty = new Dictionary<RegionKey, Region>();
        }

        public int ChunkGlobalX (int cx)
        {
            return cx;
        }

        public int ChunkGlobalZ (int cz)
        {
            return cz;
        }

        public int ChunkLocalX (int cx)
        {
            return cx & REGION_XMASK;
        }

        public int ChunkLocalZ (int cz)
        {
            return cz & REGION_ZMASK;
        }

        public Chunk GetChunk (int cx, int cz)
        {
            Region r = GetRegion(cx, cz);
            if (r == null) {
                return null;
            }

            return r.GetChunk(cx & REGION_XMASK, cz & REGION_ZMASK);
        }

        public ChunkRef GetChunkRef (int cx, int cz)
        {
            Region r = GetRegion(cx, cz);
            if (r == null) {
                return null;
            }

            return r.GetChunkRef(cx & REGION_XMASK, cz & REGION_ZMASK, this);
        }

        public bool ChunkExists (int cx, int cz)
        {
            Region r = GetRegion(cx, cz);
            if (r == null) {
                return false;
            }

            return r.ChunkExists(cx & REGION_XMASK, cz & REGION_ZMASK);
        }

        public bool MarkChunkDirty (ChunkRef chunk)
        {
            Region r = GetRegion(chunk.X, chunk.Z);
            if (r == null) {
                return false;
            }

            RegionKey k = new RegionKey(r.X, r.Z);
            _dirty[k] = r;

            r.MarkChunkDirty(chunk);

            return true;
        }

        public bool MarkChunkClean (int cx, int cz)
        {
            Region r = GetRegion(cx, cz);
            if (r == null) {
                return false;
            }

            RegionKey k = new RegionKey(r.X, r.Z);
            return _dirty.Remove(k);
        }

        public int Save ()
        {
            int saved = 0;
            foreach (Region r in _dirty.Values) {
                saved += r.Save();
            }

            _dirty.Clear();
            return saved;
        }

        public bool SaveChunk (Chunk chunk)
        {
            Region r = GetRegion(chunk.X, chunk.Z);
            if (r == null) {
                return false;
            }

            return r.SaveChunk(chunk);
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

            if (r.ChunkCount() == 0) {
                RegionKey k = new RegionKey(r.X, r.Z);
                _cache.Remove(k);
                _dirty.Remove(k);

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
