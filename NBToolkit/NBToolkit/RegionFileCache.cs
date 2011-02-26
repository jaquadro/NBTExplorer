using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NBToolkit
{
    public class RegionFileCache
    {
        private const int MAX_CACHE_SIZE = 256;

        private static Dictionary<string, WeakReference> cache = new Dictionary<string, WeakReference>();
        
        private RegionFileCache() {
        }

        public static RegionFile GetRegionFile(string basePath, string fileName) {
            string regionDir = Path.Combine(basePath, "region");
            string file = Path.Combine(regionDir, fileName);

            RegionFile rf = null;
            if (cache.ContainsKey(file)) {
                rf = cache[file].Target as RegionFile;
            }

            if (rf != null) {
                return rf;
            }

            if (!Directory.Exists(regionDir)) {
                Directory.CreateDirectory(regionDir);
            }

            if (cache.Count >= MAX_CACHE_SIZE) {
                RegionFileCache.Clear();
            }

            RegionFile reg = new RegionFile(file);
            cache.Add(file, new WeakReference(reg));
            return reg;
        }

        public static RegionFile GetRegionFile (string basePath, int chunkX, int chunkZ)
        {
            string regionDir = Path.Combine(basePath, "region");
            string fileName = Path.Combine(regionDir, "r." + (chunkX >> 5) + "." + (chunkZ >> 5) + ".mcr");

            return GetRegionFile(basePath, fileName);
        }

        public static string[] GetRegionFileList (string basePath)
        {
            string regionDir = Path.Combine(basePath, "region");

            if (!Directory.Exists(regionDir)) {
                Directory.CreateDirectory(regionDir);
            }

            string[] files = Directory.GetFiles(regionDir);
            List<string> valid = new List<string>();

            foreach (string file in files) {
                if (System.Text.RegularExpressions.Regex.IsMatch(file, "r\\.-?[0-9]+\\.-?[0-9]+\\.mcr$")) {
                    valid.Add(Path.GetFileName(file));
                }
            }

            return valid.ToArray();
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
    }
}
