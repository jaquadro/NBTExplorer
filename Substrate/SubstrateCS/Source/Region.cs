using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Substrate.NBT;
using Substrate.Core;

namespace Substrate
{
    public class Region : IDisposable, IChunkContainer
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

        //protected Dictionary<ChunkKey, WeakReference> _cache;
        //protected Dictionary<ChunkKey, ChunkRef> _dirty;

        protected ChunkCache _cache;

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

        public Region (RegionManager rm, ChunkCache cache, int rx, int rz)
        {
            _regionMan = rm;
            _cache = cache;
            _regionFile = new WeakReference(null);
            _rx = rx;
            _rz = rz;

            //_cache = new Dictionary<ChunkKey, WeakReference>();
            //_dirty = new Dictionary<ChunkKey, ChunkRef>();

            if (!File.Exists(GetFilePath())) {
                throw new FileNotFoundException();
            }
        }

        public Region (RegionManager rm, ChunkCache cache, string filename)
        {
            _regionMan = rm;
            _cache = cache;
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

        public ChunkRef GetChunkRef (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunkRef(ForeignX(lcx), ForeignZ(lcz));
            }

            int cx = lcx + _rx * ChunkManager.REGION_XLEN;
            int cz = lcz + _rz * ChunkManager.REGION_ZLEN;

            ChunkKey k = new ChunkKey(cx, cz);
            ChunkRef c = _cache.Fetch(k);
            if (c != null) {
                return c;
            }

            c = ChunkRef.Create(this, lcx, lcz);
            if (c != null) {
                _cache.Insert(c);
            }

            return c;
        }

        public ChunkRef CreateChunk (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.CreateChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            DeleteChunk(lcx, lcz);

            int cx = lcx + _rx * ChunkManager.REGION_XLEN;
            int cz = lcz + _rz * ChunkManager.REGION_ZLEN;

            Chunk c = Chunk.Create(cx, cz);
            c.Save(GetChunkOutStream(lcx, lcz));

            ChunkRef cr = ChunkRef.Create(this, lcx, lcz);
            _cache.Insert(cr);

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

            return Chunk.CreateVerified(GetChunkTree(lcx, lcz));
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

            if (ChunkCount() == 0) {
                _regionMan.DeleteRegion(X, Z);
            }

            return true;
        }

        public ChunkRef SetChunk (int lcx, int lcz, Chunk chunk)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.CreateChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            DeleteChunk(lcx, lcz);

            int cx = lcx + _rx * ChunkManager.REGION_XLEN;
            int cz = lcz + _rz * ChunkManager.REGION_ZLEN;

            chunk.SetLocation(cx, cz);
            chunk.Save(GetChunkOutStream(lcx, lcz));

            ChunkRef cr = ChunkRef.Create(this, lcx, lcz);
            _cache.Insert(cr);

            return cr;
        }

        public int Save ()
        {
            _cache.SyncDirty();

            int saved = 0;
            IEnumerator<ChunkRef> en = _cache.GetDirtyEnumerator();
            while (en.MoveNext()) {
                ChunkRef chunk = en.Current;

                if (!ChunkExists(chunk.LocalX, chunk.LocalZ)) {
                    throw new MissingChunkException();
                }

                if (chunk.Save(GetChunkOutStream(chunk.LocalX, chunk.LocalZ))) {
                    saved++;
                }
            }

            _cache.ClearDirty();
            return saved;
        }

        public bool SaveChunk (Chunk chunk)
        {
            //Console.WriteLine("Region[{0}, {1}].Save({2}, {3})", _rx, _rz, ForeignX(chunk.X),ForeignZ(chunk.Z));
            return chunk.Save(GetChunkOutStream(ForeignX(chunk.X), ForeignZ(chunk.Z)));
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
