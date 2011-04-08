using System;
using System.Collections.Generic;
using System.Collections;

namespace Substrate
{

    public class ChunkManager : IChunkManager, IChunkCache, IEnumerable<ChunkRef>
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

        public ChunkManager (ChunkManager cm)
        {
            _regionMan = cm._regionMan;
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

        public bool MarkChunkClean (ChunkRef chunk)
        {
            Region r = GetRegion(chunk.X, chunk.Z);
            if (r == null) {
                return false;
            }

            RegionKey k = new RegionKey(r.X, r.Z);
            _dirty.Remove(k);

            r.MarkChunkClean(chunk);

            return true;
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


        #region IEnumerable<ChunkRef> Members

        public IEnumerator<ChunkRef> GetEnumerator ()
        {
            return new ChunkEnumerator(this);
        }

        #endregion


        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return new ChunkEnumerator(this);
        }

        #endregion


        public class ChunkEnumerator : IEnumerator<ChunkRef>
        {
            private ChunkManager _cm;

            private IEnumerator<Region> _enum;
            private Region _region;
            private ChunkRef _chunk;

            private int _x = 0;
            private int _z = -1;

            public ChunkEnumerator (ChunkManager cm)
            {
                _cm = cm;
                _enum = _cm.GetRegionManager().GetEnumerator();
                _enum.MoveNext();
                _region = _enum.Current;
            }

            public virtual bool MoveNext ()
            {
                if (_enum == null) {
                    return MoveNextInRegion();
                }
                else {
                    while (true) {
                        if (_x >= ChunkManager.REGION_XLEN) {
                            if (!_enum.MoveNext()) {
                                return false;
                            }
                            _x = 0;
                            _z = -1;
                            _region = _enum.Current;
                        }
                        if (MoveNextInRegion()) {
                            _chunk = _cm.GetChunkRefInRegion(_region, _x, _z);
                            return true;
                        }
                    }
                }
            }

            protected bool MoveNextInRegion ()
            {
                for (; _x < ChunkManager.REGION_XLEN; _x++) {
                    for (_z++; _z < ChunkManager.REGION_ZLEN; _z++) {
                        if (_region.ChunkExists(_x, _z)) {
                            goto FoundNext;
                        }
                    }
                    _z = -1;
                }

            FoundNext:

                return (_x < ChunkManager.REGION_XLEN);
            }

            public void Reset ()
            {
                if (_enum != null) {
                    _enum.Reset();
                    _enum.MoveNext();
                    _region = _enum.Current;
                }
                _x = 0;
                _z = -1;
            }

            void IDisposable.Dispose () { }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            ChunkRef IEnumerator<ChunkRef>.Current
            {
                get
                {
                    return Current;
                }
            }

            public ChunkRef Current
            {
                get
                {
                    if (_x >= ChunkManager.REGION_XLEN) {
                        throw new InvalidOperationException();
                    }
                    return _chunk;
                }
            }
        }
    }

    public class MissingChunkException : Exception
    {

    }
}
