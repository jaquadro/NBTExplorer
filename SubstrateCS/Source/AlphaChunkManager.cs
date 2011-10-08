using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// Represents an Alpha-compatible interface for globally managing chunks.
    /// </summary>
    public class AlphaChunkManager : IChunkManager, IEnumerable<ChunkRef>
    {
        private string _mapPath;

        //protected Dictionary<ChunkKey, WeakReference> _cache;
        private LRUCache<ChunkKey, ChunkRef> _cache;
        private Dictionary<ChunkKey, ChunkRef> _dirty;

        /// <summary>
        /// Gets the path to the base directory containing the chunk directory structure.
        /// </summary>
        public string ChunkPath
        {
            get { return _mapPath; }
        }

        /// <summary>
        /// Creates a new <see cref="AlphaChunkManager"/> instance for the give chunk base directory.
        /// </summary>
        /// <param name="mapDir">The path to the chunk base directory.</param>
        public AlphaChunkManager (string mapDir)
        {
            _mapPath = mapDir;
            _cache = new LRUCache<ChunkKey, ChunkRef>(256);
            _dirty = new Dictionary<ChunkKey, ChunkRef>();
        }

        private ChunkFile GetChunkFile (int cx, int cz)
        {
            return new ChunkFile(_mapPath, cx, cz);
        }

        private NbtTree GetChunkTree (int cx, int cz)
        {
            ChunkFile cf = GetChunkFile(cx, cz);
            Stream nbtstr = cf.GetDataInputStream();
            if (nbtstr == null) {
                return null;
            }

            return new NbtTree(nbtstr);
        }

        private bool SaveChunkTree (int cx, int cz, NbtTree tree)
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

        private Stream GetChunkOutStream (int cx, int cz)
        {
            return new ChunkFile(_mapPath, cx, cz).GetDataOutputStream();
        }

        #region IChunkContainer Members

        /// <inheritdoc/>
        public int ChunkGlobalX (int cx)
        {
            return cx;
        }

        /// <inheritdoc/>
        public int ChunkGlobalZ (int cz)
        {
            return cz;
        }

        /// <inheritdoc/>
        public int ChunkLocalX (int cx)
        {
            return cx;
        }

        /// <inheritdoc/>
        public int ChunkLocalZ (int cz)
        {
            return cz;
        }

        /// <inheritdoc/>
        public Chunk GetChunk (int cx, int cz)
        {
            if (!ChunkExists(cx, cz)) {
                return null;
            }

            return Chunk.CreateVerified(GetChunkTree(cx, cz));
        }

        /// <inheritdoc/>
        public ChunkRef GetChunkRef (int cx, int cz)
        {
            ChunkKey k = new ChunkKey(cx, cz);

            ChunkRef c = null;

            //WeakReference chunkref = null;
            if (_cache.TryGetValue(k, out c)) {
                return c;
            }

            c = ChunkRef.Create(this, cx, cz);
            if (c != null) {
                _cache[k] = c;
            }

            return c;
        }

        /// <inheritdoc/>
        public ChunkRef CreateChunk (int cx, int cz)
        {
            DeleteChunk(cx, cz);
            Chunk c = Chunk.Create(cx, cz);
            c.Save(GetChunkOutStream(cx, cz));

            ChunkRef cr = ChunkRef.Create(this, cx, cz);
            ChunkKey k = new ChunkKey(cx, cz);
            _cache[k] = cr;

            return cr;
        }

        /// <inheritdoc/>
        public bool ChunkExists (int cx, int cz)
        {
            return new ChunkFile(_mapPath, cx, cz).Exists();
        }

        /// <inheritdoc/>
        public bool DeleteChunk (int cx, int cz)
        {
            new ChunkFile(_mapPath, cx, cz).Delete();

            ChunkKey k = new ChunkKey(cx, cz);
            _cache.Remove(k);
            _dirty.Remove(k);

            return true;
        }

        /// <inheritdoc/>
        public ChunkRef SetChunk (int cx, int cz, Chunk chunk)
        {
            DeleteChunk(cx, cz);
            chunk.SetLocation(cx, cz);
            chunk.Save(GetChunkOutStream(cx, cz));

            ChunkRef cr = ChunkRef.Create(this, cx, cz);
            ChunkKey k = new ChunkKey(cx, cz);
            _cache[k] = cr;

            return cr;
        }

        /// <inheritdoc/>
        public int Save ()
        {
            foreach (KeyValuePair<ChunkKey, ChunkRef> e in _cache) {
                if (e.Value.IsDirty) {
                    _dirty[e.Key] = e.Value;
                }
            }

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

        /// <inheritdoc/>
        public bool SaveChunk (Chunk chunk)
        {
            if (chunk.Save(GetChunkOutStream(ChunkGlobalX(chunk.X), ChunkGlobalZ(chunk.Z)))) {
                _dirty.Remove(new ChunkKey(chunk.X, chunk.Z));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool CanDelegateCoordinates
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Gets the (last modified) timestamp of the underlying chunk file.
        /// </summary>
        /// <param name="cx">The global X-coordinate of a chunk.</param>
        /// <param name="cz">The global Z-coordinate of a chunk.</param>
        /// <returns>The last modified timestamp of the underlying chunk file.</returns>
        public int GetChunkTimestamp (int cx, int cz)
        {
            ChunkFile cf = GetChunkFile(cx, cz);
            if (cf == null) {
                return 0;
            }

            return cf.GetModifiedTime();
        }

        #region IEnumerable<ChunkRef> Members

        /// <summary>
        /// Gets an enumerator that iterates through all the chunks in the world.
        /// </summary>
        /// <returns>An enumerator for this manager.</returns>
        public IEnumerator<ChunkRef> GetEnumerator ()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return new Enumerator(this);
        }

        #endregion


        private class Enumerator : IEnumerator<ChunkRef>
        {
            protected AlphaChunkManager _cm;
            protected Queue<string> _tld;
            protected Queue<string> _sld;
            protected Queue<ChunkRef> _chunks;

            private string _curtld;
            private string _cursld;
            private ChunkRef _curchunk;

            public Enumerator (AlphaChunkManager cfm)
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
