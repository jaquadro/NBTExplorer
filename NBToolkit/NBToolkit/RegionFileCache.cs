using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NBToolkit
{
    /*public class RegionFileCache
    {
        private const int MAX_CACHE_SIZE = 256;

        private string _regionPath;

        private Dictionary<string, WeakReference> cache = new Dictionary<string, WeakReference>();
        
        public RegionFileCache (string path) {
            _regionPath = path;

            if (!Directory.Exists(_regionPath)) {
                throw new DirectoryNotFoundException();
            }
        }

        public RegionFile GetRegionFile (string fileName) {
            //string regionDir = basePath; // Path.Combine(basePath, "region");
            string file = Path.Combine(_regionPath, fileName);

            RegionFile rf = null;
            if (cache.ContainsKey(file)) {
                rf = cache[file].Target as RegionFile;
            }

            if (rf != null) {
                return rf;
            }

            if (cache.Count >= MAX_CACHE_SIZE) {
                RegionFileCache.Clear();
            }

            RegionFile reg = new RegionFile(file);
            cache.Add(file, new WeakReference(reg));
            return reg;
        }

        public RegionFile GetRegionFile (int chunkX, int chunkZ)
        {
            string fileName = "r." + (chunkX >> 5) + "." + (chunkZ >> 5) + ".mcr";

            return GetRegionFile(fileName);
        }

        public static void Clear() {
            foreach (WeakReference wr in cache.Values) {
                RegionFile rf = wr.Target as RegionFile;
                if (rf != null) {
                    rf.Close();
                }
            }
            cache.Clear();
        }

        public static int getSizeDelta(string basePath, int chunkX, int chunkZ) {
            RegionFile r = GetRegionFile(basePath, chunkX, chunkZ);
            return r.GetSizeDelta();
        }

        public static Stream getChunkDataInputStream(string basePath, int chunkX, int chunkZ) {
            RegionFile r = GetRegionFile(basePath, chunkX, chunkZ);
            return r.GetChunkDataInputStream(chunkX & 31, chunkZ & 31);
        }

        public static Stream getChunkDataOutputStream(string basePath, int chunkX, int chunkZ) {
            RegionFile r = GetRegionFile(basePath, chunkX, chunkZ);
            return r.GetChunkDataOutputStream(chunkX & 31, chunkZ & 31);
        }
    }*/
}
