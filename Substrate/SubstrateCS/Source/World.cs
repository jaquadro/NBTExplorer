using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Substrate
{
    using NBT;

    public static class Dimension 
    {
        public const int NETHER = -1;
        public const int DEFAULT = 0;
    }

    public interface INBTWorld
    {
        string WorldPath { get; }

        Level Level { get; }

        IBlockManager GetBlockManager ();
        IBlockManager GetBlockManager (int dim);

        IChunkManager GetChunkManager ();
        IChunkManager GetChunkManager (int dim);
    }

    public class AlphaWorld : INBTWorld
    {
        protected string _path;
        protected string _levelFile = "level.dat";

        protected Level _level;

        private Dictionary<int, ChunkFileManager> _chunkMgrs;
        private Dictionary<int, BlockManager> _blockMgrs;

        private AlphaWorld ()
        {
            _chunkMgrs = new Dictionary<int, ChunkFileManager>();
            _blockMgrs = new Dictionary<int, BlockManager>();
        }

        public BlockManager GetBlockManager ()
        {
            return GetBlockManager(Dimension.DEFAULT);
        }

        public BlockManager GetBlockManager (int dim)
        {
            BlockManager rm;
            if (_blockMgrs.TryGetValue(dim, out rm)) {
                return rm;
            }

            OpenDimension(dim);
            return _blockMgrs[dim];
        }

        public ChunkFileManager GetChunkManager ()
        {
            return GetChunkManager(Dimension.DEFAULT);
        }

        public ChunkFileManager GetChunkManager (int dim)
        {
            ChunkFileManager rm;
            if (_chunkMgrs.TryGetValue(dim, out rm)) {
                return rm;
            }

            OpenDimension(dim);
            return _chunkMgrs[dim];
        }

        public static AlphaWorld Open (string path)
        {
            return new AlphaWorld().OpenWorld(path) as AlphaWorld;
        }

        public static AlphaWorld Create (string path)
        {
            return new AlphaWorld().CreateWorld(path) as AlphaWorld;
        }

        public void Save ()
        {
            _level.Save();

            foreach (KeyValuePair<int, ChunkFileManager> cm in _chunkMgrs) {
                cm.Value.Save();
            }
        }

        private void OpenDimension (int dim)
        {
            string path = _path;
            if (dim != Dimension.DEFAULT) {
                path = Path.Combine(path, "DIM" + dim);
            }

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            ChunkFileManager cm = new ChunkFileManager(path);
            BlockManager bm = new BlockManager(cm);

            _chunkMgrs[dim] = cm;
            _blockMgrs[dim] = bm;
        }

        private AlphaWorld OpenWorld (string path)
        {
            if (!Directory.Exists(path)) {
                if (File.Exists(path)) {
                    _levelFile = Path.GetFileName(path);
                    path = Path.GetDirectoryName(path);
                }
                else {
                    throw new DirectoryNotFoundException("Directory '" + path + "' not found");
                }
            }

            _path = path;

            string ldat = Path.Combine(path, _levelFile);
            if (!File.Exists(ldat)) {
                throw new FileNotFoundException("Data file '" + _levelFile + "' not found in '" + path + "'", ldat);
            }

            if (!LoadLevel()) {
                throw new Exception("Failed to load '" + _levelFile + "'");
            }

            return this;
        }

        private AlphaWorld CreateWorld (string path)
        {
            if (!Directory.Exists(path)) {
                throw new DirectoryNotFoundException("Directory '" + path + "' not found");
            }

            _path = path;
            _level = new Level(this);

            return this;
        }

        private bool LoadLevel ()
        {
            NBTFile nf = new NBTFile(Path.Combine(_path, _levelFile));
            Stream nbtstr = nf.GetDataInputStream();
            if (nbtstr == null) {
                return false;
            }

            NBT_Tree tree = new NBT_Tree(nbtstr);

            _level = new Level(this);
            _level = _level.LoadTreeSafe(tree.Root);

            return _level != null;
        }


        #region INBTWorld Members

        public string WorldPath
        {
            get { return _path; }
        }

        public Level Level
        {
            get { return _level; }
        }

        IBlockManager INBTWorld.GetBlockManager ()
        {
            return GetBlockManager();
        }

        IBlockManager INBTWorld.GetBlockManager (int dim)
        {
            return GetBlockManager(dim);
        }

        IChunkManager INBTWorld.GetChunkManager ()
        {
            return GetChunkManager();
        }

        IChunkManager INBTWorld.GetChunkManager (int dim)
        {
            return GetChunkManager(dim);
        }

        #endregion
    }

    public class BetaWorld : INBTWorld {
        private const string _REGION_DIR = "region";
        protected string _path;
        protected string _levelFile = "level.dat";

        protected Level _level;

        private Dictionary<int, RegionManager> _regionMgrs;
        private Dictionary<int, ChunkManager> _chunkMgrs;
        private Dictionary<int, BlockManager> _blockMgrs;

        private BetaWorld ()
        {
            _regionMgrs = new Dictionary<int, RegionManager>();
            _chunkMgrs = new Dictionary<int, ChunkManager>();
            _blockMgrs = new Dictionary<int, BlockManager>();
        }

        public BlockManager GetBlockManager ()
        {
            return GetBlockManager(Dimension.DEFAULT);
        }

        public BlockManager GetBlockManager (int dim)
        {
            BlockManager rm;
            if (_blockMgrs.TryGetValue(dim, out rm)) {
                return rm;
            }

            OpenDimension(dim);
            return _blockMgrs[dim];
        }

        public ChunkManager GetChunkManager ()
        {
            return GetChunkManager(Dimension.DEFAULT);
        }

        public ChunkManager GetChunkManager (int dim)
        {
            ChunkManager rm;
            if (_chunkMgrs.TryGetValue(dim, out rm)) {
                return rm;
            }

            OpenDimension(dim);
            return _chunkMgrs[dim];
        }

        public RegionManager GetRegionManager ()
        {
            return GetRegionManager(Dimension.DEFAULT);
        }

        public RegionManager GetRegionManager (int dim)
        {
            RegionManager rm;
            if (_regionMgrs.TryGetValue(dim, out rm)) {
                return rm;
            }

            OpenDimension(dim);
            return _regionMgrs[dim];
        }

        public static BetaWorld Open (string path)
        {
            return new BetaWorld().OpenWorld(path) as BetaWorld;
        }

        public static BetaWorld Create (string path)
        {
            return new BetaWorld().CreateWorld(path) as BetaWorld;
        }

        public void Save ()
        {
            _level.Save();

            foreach (KeyValuePair<int, RegionManager> rm in _regionMgrs) {
                rm.Value.Save();
            }
        }

        private void OpenDimension (int dim)
        {
            string path = _path;
            if (dim == Dimension.DEFAULT) {
                path = Path.Combine(path, _REGION_DIR);
            }
            else {
                path = Path.Combine(path, "DIM" + dim);
                path = Path.Combine(path, _REGION_DIR);
            }

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            RegionManager rm = new RegionManager(path);
            ChunkManager cm = new ChunkManager(rm);
            BlockManager bm = new BlockManager(cm);

            _regionMgrs[dim] = rm;
            _chunkMgrs[dim] = cm;
            _blockMgrs[dim] = bm;
        }

        private BetaWorld OpenWorld (string path)
        {
            if (!Directory.Exists(path)) {
                if (File.Exists(path)) {
                    _levelFile = Path.GetFileName(path);
                    path = Path.GetDirectoryName(path);
                }
                else {
                    throw new DirectoryNotFoundException("Directory '" + path + "' not found");
                }
            }

            _path = path;

            string ldat = Path.Combine(path, _levelFile);
            if (!File.Exists(ldat)) {
                throw new FileNotFoundException("Data file '" + _levelFile + "' not found in '" + path + "'", ldat);
            }

            if (!LoadLevel()) {
                throw new Exception("Failed to load '" + _levelFile + "'");
            }

            return this;
        }

        private BetaWorld CreateWorld (string path)
        {
            if (!Directory.Exists(path)) {
                throw new DirectoryNotFoundException("Directory '" + path + "' not found");
            }

            string regpath = Path.Combine(path, _REGION_DIR);
            if (!Directory.Exists(regpath)) {
                Directory.CreateDirectory(regpath);
            }

            _path = path;
            _level = new Level(this);

            return this;
        }

        private bool LoadLevel ()
        {
            NBTFile nf = new NBTFile(Path.Combine(_path, _levelFile));
            Stream nbtstr = nf.GetDataInputStream();
            if (nbtstr == null) {
                return false;
            }

            NBT_Tree tree = new NBT_Tree(nbtstr);

            _level = new Level(this);
            _level = _level.LoadTreeSafe(tree.Root);

            return _level != null;
        }


        #region INBTWorld Members

        public string WorldPath
        {
            get { return _path; }
        }

        public Level Level
        {
            get { return _level; }
        }

        IBlockManager INBTWorld.GetBlockManager ()
        {
            return GetBlockManager();
        }

        IBlockManager INBTWorld.GetBlockManager (int dim)
        {
            return GetBlockManager(dim);
        }

        IChunkManager INBTWorld.GetChunkManager ()
        {
            return GetChunkManager();
        }

        IChunkManager INBTWorld.GetChunkManager (int dim)
        {
            return GetChunkManager(dim);
        }

        #endregion
    }

    public class DimensionNotFoundException : Exception { }
}
