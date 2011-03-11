using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
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

        public ChunkManager (RegionManager rm)
        {
            _regionMan = rm;
            _cache = new Dictionary<ChunkKey, WeakReference>();
        }

        public Chunk GetChunk (int cx, int cz)
        {
            ChunkKey k = new ChunkKey(cx, cz);

            Chunk c = null;
            if (_cache.ContainsKey(k)) {
                c = _cache[k].Target as Chunk;
            }
            else {
                _cache.Add(k, new WeakReference(null));
            }

            if (c != null) {
                return c;
            }

            c = new Chunk(this, cx, cz);
            _cache[k].Target = c;

            return c;
        }

        public Chunk GetChunkInRegion (Region r, int lcx, int lcz)
        {
            int cx = r.X * REGION_XLEN + lcx;
            int cz = r.Z * REGION_ZLEN + lcz;
            return GetChunk(cx, cz);
        }

        public Region GetRegion (int cx, int cz)
        {
            cx >>= REGION_XLOG;
            cz >>= REGION_ZLOG;
            return _regionMan.GetRegion(cx, cz);
        }

        public RegionManager GetRegionManager ()
        {
            return _regionMan;
        }
    }
}
