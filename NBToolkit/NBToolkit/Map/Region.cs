using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace NBToolkit
{
    using NBT;

    public class Region : IDisposable
    {
        protected int _rx;
        protected int _rz;
        protected bool _disposed = false;

        protected RegionManager _regionMan;

        protected static Regex _namePattern = new Regex("r\\.(-?[0-9]+)\\.(-?[0-9]+)\\.mcr$");

        protected WeakReference _regionFile;

        public Region (RegionManager rm, int rx, int rz)
        {
            _regionMan = rm;
            _regionFile = new WeakReference(null);
            _rx = rx;
            _rz = rz;

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

        public int X
        {
            get
            {
                return _rx;
            }
        }

        public int Z
        {
            get
            {
                return _rz;
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
            RegionFile rf = GetRegionFile();
            Stream nbtstr = rf.GetChunkDataInputStream(lcx, lcz);
            if (nbtstr == null) {
                return null;
            }

            return new NBT_Tree(nbtstr);
        }

        public bool SaveChunkTree (int lcx, int lcz, NBT_Tree tree)
        {
            RegionFile rf = GetRegionFile();
            Stream zipstr = rf.GetChunkDataOutputStream(lcx, lcz);
            if (zipstr == null) {
                return false;
            }

            tree.WriteTo(zipstr);
            zipstr.Close();

            return true;
        }

        public bool ChunkExists (int lcx, int lcz)
        {
            RegionFile rf = GetRegionFile();
            return rf.HasChunk(lcx, lcz);
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

        public bool DeleteChunk (int lcx, int lcz)
        {
            RegionFile rf = GetRegionFile();
            if (!rf.HasChunk(lcx, lcz)) {
                return false;
            }

            rf.DeleteChunk(lcx, lcz);
            return true;
        }
    }
}
