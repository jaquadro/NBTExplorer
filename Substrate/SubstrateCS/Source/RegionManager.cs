using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Substrate.Core;

namespace Substrate
{
    public interface IRegionContainer
    {
        bool RegionExists (int rx, int rz);

        Region GetRegion (int rx, int rz);
        Region CreateRegion (int rx, int rz);

        bool DeleteRegion (int rx, int rz);
    }

    public interface IRegionManager : IRegionContainer, IEnumerable<Region>
    {
    }

    public class RegionManager : IRegionManager
    {
        protected string _regionPath;

        protected Dictionary<RegionKey, Region> _cache;

        protected ChunkCache _chunkCache;

        public RegionManager (string regionDir, ChunkCache cache)
        {
            _regionPath = regionDir;
            _chunkCache = cache;
            _cache = new Dictionary<RegionKey, Region>();
        }

        public Region GetRegion (int rx, int rz)
        {
            RegionKey k = new RegionKey(rx, rz);
            Region r;

            try {
                if (_cache.TryGetValue(k, out r) == false) {
                    r = new Region(this, _chunkCache, rx, rz);
                    _cache.Add(k, r);
                }
                return r;
            }
            catch (FileNotFoundException) {
                _cache.Add(k, null);
                return null;
            }
        }

        public bool RegionExists (int rx, int rz)
        {
            Region r = GetRegion(rx, rz);
            return r != null;
        }

        public Region CreateRegion (int rx, int rz)
        {
            Region r = GetRegion(rx, rz);
            if (r == null) {
                string fp = "r." + rx + "." + rz + ".mcr";
                using (RegionFile rf = new RegionFile(Path.Combine(_regionPath, fp))) {
                    
                }

                r = new Region(this, _chunkCache, rx, rz);

                RegionKey k = new RegionKey(rx, rz);
                _cache[k] = r;
            }

            return r;
        }


        public Region GetRegion (string filename)
        {
            int rx, rz;
            if (!Region.ParseFileName(filename, out rx, out rz)) {
                throw new ArgumentException("Malformed region file name: " + filename, "filename");
            }

            return GetRegion(rx, rz);
        }


        public string GetRegionPath ()
        {
            return _regionPath;
        }

        public bool DeleteRegion (int rx, int rz)
        {
            Region r = GetRegion(rx, rz);
            if (r == null) {
                return false;
            }

            RegionKey k = new RegionKey(rx, rz);
            _cache.Remove(k);

            r.Dispose();

            try {
                File.Delete(r.GetFilePath());
            }
            catch (Exception e) {
                Console.WriteLine("NOTICE: " + e.Message);
                return false;
            }

            return true;
        }

        /*public int Save ()
        {
            int saved = 0;
            foreach (Region r in _cache.Values) {
                saved += r.Save();
            }

            return saved;
        }*/


        #region IEnumerable<Region> Members

        public IEnumerator<Region> GetEnumerator ()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return new Enumerator(this);
        }

        #endregion


        private struct Enumerator : IEnumerator<Region>
        {
            private List<Region> _regions;
            private int _pos;

            public Enumerator (List<Region> regs)
            {
                _regions = regs;
                _pos = -1;
            }

            public Enumerator (RegionManager rm)
            {
                _regions = new List<Region>();
                _pos = -1;

                if (!Directory.Exists(rm.GetRegionPath())) {
                    throw new DirectoryNotFoundException();
                }

                string[] files = Directory.GetFiles(rm.GetRegionPath());
                _regions.Capacity = files.Length;

                foreach (string file in files) {
                    try {
                        Region r = rm.GetRegion(file);
                        _regions.Add(r);
                    }
                    catch (ArgumentException) {
                        continue;
                    }
                }
            }

            public bool MoveNext ()
            {
                _pos++;
                return (_pos < _regions.Count);
            }

            public void Reset ()
            {
                _pos = -1;
            }

            void IDisposable.Dispose () { }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            Region IEnumerator<Region>.Current
            {
                get
                {
                    return Current;
                }
            }

            public Region Current
            {
                get
                {
                    try {
                        return _regions[_pos];
                    }
                    catch (IndexOutOfRangeException) {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

    }
}
