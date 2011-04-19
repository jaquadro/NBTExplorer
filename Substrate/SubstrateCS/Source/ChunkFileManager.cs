using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Substrate
{
    using NBT;
    using Utility;

    public class ChunkFileManager : IChunkManager, IChunkCache
    {
        protected string _mapPath;

        protected Dictionary<ChunkKey, WeakReference> _cache;
        protected Dictionary<ChunkKey, ChunkRef> _dirty;

        public string ChunkPath
        {
            get { return _mapPath; }
        }

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

            c = ChunkRef.Create(this, this, cx, cz);
            if (c != null) {
                _cache[k].Target = c;
            }

            return c;
        }

        public ChunkRef CreateChunk (int cx, int cz)
        {
            DeleteChunk(cx, cz);
            Chunk c = new Chunk(cx, cz);
            c.Save(GetChunkOutStream(cx, cz));

            ChunkRef cr = ChunkRef.Create(this, this, cx, cz);
            ChunkKey k = new ChunkKey(cx, cz);
            _cache[k] = new WeakReference(cr);

            return cr;
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
            protected ChunkFileManager _cm;
            protected Queue<string> _tld;
            protected Queue<string> _sld;
            protected Queue<ChunkRef> _chunks;

            private string _curtld;
            private string _cursld;
            private ChunkRef _curchunk;

            public ChunkEnumerator (ChunkFileManager cfm)
            {
                _cm = cfm;

                if (!Directory.Exists(_cm.ChunkPath)) {
                    throw new DirectoryNotFoundException();
                }

                Reset();
            }

            private bool MoveNextTLD ()
            {
                if (_tld.Count == 0) {
                    return false;
                }

                _curtld = _tld.Dequeue();

                //string path = Path.Combine(_cm.ChunkPath, _curtld);

                string[] files = Directory.GetDirectories(_curtld);
                foreach (string file in files) {
                    _sld.Enqueue(file);
                }

                return true;
            }

            public bool MoveNextSLD ()
            {
                while (_sld.Count == 0) {
                    if (MoveNextTLD() == false) {
                        return false;
                    }
                }

                _cursld = _sld.Dequeue();

                //string path = Path.Combine(_cm.ChunkPath, _curtld);
                //path = Path.Combine(path, _cursld);

                string[] files = Directory.GetFiles(_cursld);
                foreach (string file in files) {
                    int x;
                    int z;

                    string basename = Path.GetFileName(file);

                    if (!ParseFileName(basename, out x, out z)) {
                        continue;
                    }

                    ChunkRef cref = _cm.GetChunkRef(x, z);
                    if (cref != null) {
                        _chunks.Enqueue(cref);
                    }
                }

                return true;
            }

            public bool MoveNext ()
            {
                while (_chunks.Count == 0) {
                    if (MoveNextSLD() == false) {
                        return false;
                    }
                }

                _curchunk = _chunks.Dequeue();
                return true;
            }

            public void Reset ()
            {
                _curchunk = null;

                _tld = new Queue<string>();
                _sld = new Queue<string>();
                _chunks = new Queue<ChunkRef>();

                string[] files = Directory.GetDirectories(_cm.ChunkPath);
                foreach (string file in files) {
                    _tld.Enqueue(file);
                }
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
                    if (_curchunk == null) {
                        throw new InvalidOperationException();
                    }
                    return _curchunk;
                }
            }

            private bool ParseFileName (string filename, out int x, out int z)
            {
                x = 0;
                z = 0;

                Match match = _namePattern.Match(filename);
                if (!match.Success) {
                    return false;
                }

                x = (int)Base36.Decode(match.Groups[1].Value);
                z = (int)Base36.Decode(match.Groups[2].Value);
                return true;
            }

            protected static Regex _namePattern = new Regex("c\\.(-?[0-9a-zA-Z]+)\\.(-?[0-9a-zA-Z]+)\\.dat$");
        }
    }
}
