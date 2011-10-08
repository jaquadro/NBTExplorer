using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Substrate.Core;

namespace Substrate
{
    /// <summary>
    /// Manages the regions of a Beta-compatible world.
    /// </summary>
    public class RegionManager : IRegionManager
    {
        private string _regionPath;

        private Dictionary<RegionKey, Region> _cache;

        private ChunkCache _chunkCache;

        /// <summary>
        /// Creates a new instance of a <see cref="RegionManager"/> for the given region directory and chunk cache.
        /// </summary>
        /// <param name="regionDir">The path to a directory containing region files.</param>
        /// <param name="cache">The shared chunk cache to hold chunk data in.</param>
        public RegionManager (string regionDir, ChunkCache cache)
        {
            _regionPath = regionDir;
            _chunkCache = cache;
            _cache = new Dictionary<RegionKey, Region>();
        }

        /// <summary>
        /// Gets a <see cref="Region"/> at the given coordinates.
        /// </summary>
        /// <param name="rx">The global X-coordinate of a region.</param>
        /// <param name="rz">The global Z-coordinate of a region.</param>
        /// <returns>A <see cref="Region"/> representing a region at the given coordinates, or null if the region does not exist.</returns>
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

        /// <summary>
        /// Determines if a region exists at the given coordinates.
        /// </summary>
        /// <param name="rx">The global X-coordinate of a region.</param>
        /// <param name="rz">The global Z-coordinate of a region.</param>
        /// <returns>True if a region exists at the given global region coordinates; false otherwise.</returns>
        public bool RegionExists (int rx, int rz)
        {
            Region r = GetRegion(rx, rz);
            return r != null;
        }

        /// <summary>
        /// Creates a new empty region at the given coordinates, if no region exists.
        /// </summary>
        /// <param name="rx">The global X-coordinate of a region.</param>
        /// <param name="rz">The global Z-coordinate of a region.</param>
        /// <returns>A new empty <see cref="Region"/> object for the given coordinates, or an existing <see cref="Region"/> if one exists.</returns>
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

        /// <summary>
        /// Gets a <see cref="Region"/> for the given region filename.
        /// </summary>
        /// <param name="filename">The filename of the region to get.</param>
        /// <returns>A <see cref="Region"/> corresponding to the coordinates encoded in the filename.</returns>
        public Region GetRegion (string filename)
        {
            int rx, rz;
            if (!Region.ParseFileName(filename, out rx, out rz)) {
                throw new ArgumentException("Malformed region file name: " + filename, "filename");
            }

            return GetRegion(rx, rz);
        }

        /// <summary>
        /// Get the current region directory path.
        /// </summary>
        /// <returns>The path to the region directory.</returns>
        public string GetRegionPath ()
        {
            return _regionPath;
        }

        // XXX: Exceptions
        /// <summary>
        /// Deletes a region at the given coordinates.
        /// </summary>
        /// <param name="rx">The global X-coordinate of a region.</param>
        /// <param name="rz">The global Z-coordinate of a region.</param>
        /// <returns>True if a region was deleted; false otherwise.</returns>
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

        #region IEnumerable<Region> Members

        /// <summary>
        /// Returns an enumerator that iterates over all of the regions in the underlying dimension.
        /// </summary>
        /// <returns>An enumerator instance.</returns>
        public IEnumerator<Region> GetEnumerator ()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates over all of the regions in the underlying dimension.
        /// </summary>
        /// <returns>An enumerator instance.</returns>
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return new Enumerator(this);
        }

        #endregion


        private struct Enumerator : IEnumerator<Region>
        {
            private List<Region> _regions;
            private int _pos;

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
