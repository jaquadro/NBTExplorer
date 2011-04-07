using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Substrate
{
    using NBT;

    public class ChunkFileManager : IChunkManager, IChunkCache
    {
        protected string _mapPath;

        protected Dictionary<ChunkKey, WeakReference> _cache;
        protected Dictionary<ChunkKey, ChunkRef> _dirty;

        public ChunkFileManager (string mapDir)
        {
            _mapPath = mapDir;
            _cache = new Dictionary<ChunkKey, WeakReference>();
            _dirty = new Dictionary<ChunkKey, ChunkRef>();
        }

        protected ChunkFile GetChunkFile (int cx, int cz)
        {
            return new ChunkFile(_mapPath, cx, cz);
        }

        protected NBT_Tree GetChunkTree (int cx, int cz)
        {
            ChunkFile cf = GetChunkFile(cx, cz);
            Stream nbtstr = cf.GetDataInputStream();
            if (nbtstr == null) {
                return null;
            }

            return new NBT_Tree(nbtstr);
        }

        protected bool SaveChunkTree (int cx, int cz, NBT_Tree tree)
        {
            ChunkFile cf = GetChunkFile(cx, cz);
            Stream zipstr = cf.GetDataOutputStream();
            if (zipstr == null) {
                return false;
            }

            tree.WriteTo(zipstr);
            zipstr.Close();

            return true;
        }

        protected Stream GetChunkOutStream (int cx, int cz)
        {
            return new ChunkFile(_mapPath, cx, cz).GetDataOutputStream();
        }

        #region IChunkContainer Members

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
            return cx;
        }

        public int ChunkLocalZ (int cz)
        {
            return cz;
        }

        public Chunk GetChunk (int cx, int cz)
        {
            if (!ChunkExists(cx, cz)) {
                return null;
            }

            return new Chunk(GetChunkTree(cx, cz));
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
                c = new ChunkRef(this, this, cx, cz);
                _cache[k].Target = c;
                return c;
            }
            catch (MissingChunkException) {
                return null;
            }
        }

        public bool ChunkExists (int cx, int cz)
        {
            return new ChunkFile(_mapPath, cx, cz).Exists();
        }

        public bool DeleteChunk (int cx, int cz)
        {
            new ChunkFile(_mapPath, cx, cz).Delete();

            ChunkKey k = new ChunkKey(cx, cz);
            _cache.Remove(k);
            _dirty.Remove(k);

            return true;
        }

        public int Save ()
        {
            int saved = 0;
            foreach (ChunkRef c in _dirty.Values) {
                int cx = ChunkGlobalX(c.X);
                int cz = ChunkGlobalZ(c.Z);

                if (c.Save(GetChunkOutStream(cx, cz))) {
                    saved++;
                }
            }

            _dirty.Clear();
            return saved;
        }

        public bool SaveChunk (Chunk chunk)
        {
            return chunk.Save(GetChunkOutStream(ChunkGlobalX(chunk.X), ChunkGlobalZ(chunk.Z)));
        }

        #endregion

        #region IChunkCache Members

        public bool MarkChunkDirty (ChunkRef chunk)
        {
            int cx = chunk.X;
            int cz = chunk.Z;

            ChunkKey k = new ChunkKey(cx, cz);
            if (!_dirty.ContainsKey(k)) {
                _dirty.Add(k, GetChunkRef(cx, cz));
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
