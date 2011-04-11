using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Substrate
{
    using NBT;

    public class Region : IDisposable, IChunkContainer, IChunkCache
    {
        private const int XDIM = 32;
        private const int ZDIM = 32;
        private const int XMASK = XDIM - 1;
        private const int ZMASK = ZDIM - 1;
        private const int XLOG = 5;
        private const int ZLOG = 5;

        protected int _rx;
        protected int _rz;
        protected bool _disposed = false;

        protected RegionManager _regionMan;

        protected static Regex _namePattern = new Regex("r\\.(-?[0-9]+)\\.(-?[0-9]+)\\.mcr$");

        protected WeakReference _regionFile;

        protected Dictionary<ChunkKey, WeakReference> _cache;
        protected Dictionary<ChunkKey, ChunkRef> _dirty;

        public int X
        {
            get { return _rx; }
        }

        public int Z
        {
            get { return _rz; }
        }

        public int XDim
        {
            get { return XDIM; }
        }

        public int ZDim
        {
            get { return ZDIM; }
        }

        public Region (RegionManager rm, int rx, int rz)
        {
            _regionMan = rm;
            _regionFile = new WeakReference(null);
            _rx = rx;
            _rz = rz;

            _cache = new Dictionary<ChunkKey, WeakReference>();
            _dirty = new Dictionary<ChunkKey, ChunkRef>();

            if (!File.Exists(GetFilePath())) {
                throw new FileNotFoundException();
            }
        }

        public Region (RegionManager rm, string filename)
        {
            _regionMan = rm;
            _regionFile = new WeakReference(null);

            ParseFileName(filename, out _rx, out _rz);

            if (!File.Exists(Path.Combine(_regionMan.GetRegionPath(), filename))) {
                throw new FileNotFoundException();
            }
        }

        ~Region ()
        {
            Dispose(false);
        }

        public void Dispose ()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (!_disposed) {
                if (disposing) {
                    // Cleanup managed resources
                    RegionFile rf = _regionFile.Target as RegionFile;
                    if (rf != null) {
                        rf.Dispose();
                        rf = null;
                    }
                }

                // Cleanup unmanaged resources
            }
            _disposed = true;
        }

        public string GetFileName ()
        {
            return "r." + _rx + "." + _rz + ".mcr";
            
        }

        public static bool TestFileName (string filename)
        {
            Match match = _namePattern.Match(filename);
            if (!match.Success) {
                return false;
            }

            return true;
        }

        public static bool ParseFileName (string filename, out int x, out int z)
        {
            x = 0;
            z = 0;

            Match match = _namePattern.Match(filename);
            if (!match.Success) {
                return false;
            }

            x = Convert.ToInt32(match.Groups[1].Value);
            z = Convert.ToInt32(match.Groups[2].Value);
            return true;
        }

        public string GetFilePath ()
        {
            return System.IO.Path.Combine(_regionMan.GetRegionPath(), GetFileName());
        }

        protected RegionFile GetRegionFile ()
        {
            RegionFile rf = _regionFile.Target as RegionFile;
            if (rf == null) {
                rf = new RegionFile(GetFilePath());
                _regionFile.Target = rf;
            }

            return rf;
        }

        public NBT_Tree GetChunkTree (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunkTree(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            Stream nbtstr = rf.GetChunkDataInputStream(lcx, lcz);
            if (nbtstr == null) {
                return null;
            }

            NBT_Tree tree = new NBT_Tree(nbtstr);

            nbtstr.Close();
            return tree;
        }

        public bool SaveChunkTree (int lcx, int lcz, NBT_Tree tree)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? false : alt.SaveChunkTree(ForeignX(lcx), ForeignZ(lcz), tree);
            }

            RegionFile rf = GetRegionFile();
            Stream zipstr = rf.GetChunkDataOutputStream(lcx, lcz);
            if (zipstr == null) {
                return false;
            }

            tree.WriteTo(zipstr);
            zipstr.Close();

            return true;
        }

        public Stream GetChunkOutStream (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunkOutStream(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            return rf.GetChunkDataOutputStream(lcx, lcz);
        }

        public int ChunkCount ()
        {
            RegionFile rf = GetRegionFile();

            int count = 0;
            for (int x = 0; x < ChunkManager.REGION_XLEN; x++) {
                for (int z = 0; z < ChunkManager.REGION_ZLEN; z++) {
                    if (rf.HasChunk(x, z)) {
                        count++;
                    }
                }
            }

            return count;
        }

        public ChunkRef GetChunkRef (int lcx, int lcz, IChunkCache cache)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunkRef(ForeignX(lcx), ForeignZ(lcz), cache);
            }

            ChunkKey k = new ChunkKey(lcx, lcz);

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
                c = new ChunkRef(this, cache, lcx, lcz);
                _cache[k].Target = c;
                return c;
            }
            catch (MissingChunkException) {
                return null;
            }
        }

        public ChunkRef CreateChunk (int lcx, int lcz)
        {
            return CreateChunk(lcx, lcz, this);
        }

        public ChunkRef CreateChunk (int lcx, int lcz, IChunkCache cache)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.CreateChunk(ForeignX(lcx), ForeignZ(lcz), cache);
            }

            DeleteChunk(lcx, lcz);

            Chunk c = new Chunk(ChunkGlobalX(lcx), ChunkGlobalZ(lcz));
            c.Save(GetChunkOutStream(lcx, lcz));

            ChunkRef cr = new ChunkRef(this, cache, lcx, lcz);
            ChunkKey k = new ChunkKey(lcx, lcz);
            _cache[k] = new WeakReference(cr);

            return cr;
        }


        #region IChunkCollection Members

        public int ChunkGlobalX (int cx)
        {
            return _rx * ChunkManager.REGION_XLEN + cx;
        }

        public int ChunkGlobalZ (int cz)
        {
            return _rz * ChunkManager.REGION_ZLEN + cz;
        }

        public int ChunkLocalX (int cx)
        {
            return cx;
        }

        public int ChunkLocalZ (int cz)
        {
            return cz;
        }

        public Chunk GetChunk (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            if (!ChunkExists(lcx, lcz)) {
                return null;
            }

            return new Chunk(GetChunkTree(lcx, lcz));
        }

        public ChunkRef GetChunkRef (int lcx, int lcz)
        {
            return GetChunkRef(lcx, lcz, this);
        }

        public bool ChunkExists (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? false : alt.ChunkExists(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            return rf.HasChunk(lcx, lcz);
        }

        public bool DeleteChunk (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? false : alt.DeleteChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            if (!rf.HasChunk(lcx, lcz)) {
                return false;
            }

            rf.DeleteChunk(lcx, lcz);

            ChunkKey k = new ChunkKey(lcx, lcz);
            _cache.Remove(k);
            _dirty.Remove(k);

            if (ChunkCount() == 0) {
                _regionMan.DeleteRegion(X, Z);
            }

            return true;
        }

        public int Save ()
        {
            int saved = 0;
            foreach (ChunkRef c in _dirty.Values) {
                int lcx = c.LocalX;
                int lcz = c.LocalZ;

                if (!ChunkExists(lcx, lcz)) {
                    throw new MissingChunkException();
                }

                if (c.Save(GetChunkOutStream(lcx, lcz))) {
                    saved++;
                }
            }

            _dirty.Clear();
            return saved;
        }

        public bool SaveChunk (Chunk chunk)
        {
            return chunk.Save(GetChunkOutStream(ChunkLocalX(chunk.X), ChunkLocalZ(chunk.Z)));
        }

        #endregion


        #region IChunkCache Members

        public bool MarkChunkDirty (ChunkRef chunk)
        {
            int lcx = chunk.LocalX;
            int lcz = chunk.LocalZ;

            ChunkKey k = new ChunkKey(lcx, lcz);
            if (!_dirty.ContainsKey(k)) {
                _dirty.Add(k, GetChunkRef(lcx, lcz));
                return true;
            }
            return false;
        }

        public bool MarkChunkClean (ChunkRef chunk)
        {
            int lcx = chunk.LocalX;
            int lcz = chunk.LocalZ;

            ChunkKey k = new ChunkKey(lcx, lcz);
            if (_dirty.ContainsKey(k)) {
                _dirty.Remove(k);
                return true;
            }
            return false;
        }

        #endregion

        private bool LocalBoundsCheck (int lcx, int lcz)
        {
            return (lcx >= 0 && lcx < XDIM && lcz >= 0 && lcz < ZDIM);
        }

        private Region GetForeignRegion (int lcx, int lcz)
        {
            return _regionMan.GetRegion(_rx + (lcx >> XLOG), _rz + (lcz >> ZLOG));
        }

        private int ForeignX (int lcx)
        {
            return (lcx + XDIM * 10000) & XMASK;
        }

        private int ForeignZ (int lcz)
        {
            return (lcz + ZDIM * 10000) & ZMASK;
        }
    }
}
