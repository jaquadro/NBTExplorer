using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Substrate
{
    using NBT;

    public abstract class World
    {
        private string _worldPath;

        public string WorldPath
        {
            get { return _worldPath; }
        }

        protected World (string path)
        {
            _worldPath = path;

            if (!File.Exists(Path.Combine(WorldPath, "level.dat"))) {
                throw new Exception("Could not locate level.dat");
            }
        }
    }

    public abstract class NBTWorld : World
    {
        private Level _level;
        private PlayerManager _playerMan;

        public Level Level
        {
            get { return _level; }
        }

        public PlayerManager PlayerManager
        {
            get { return _playerMan; }
        }

        public abstract IChunkManager ChunkManager { get; }
        public abstract IBlockManager BlockManager { get; }

        protected NBTWorld (string path) 
            : base(path)
        {
            if (!LoadLevel()) {
                throw new Exception("Failed to load level.dat");
            }

            if (Directory.Exists(Path.Combine(path, "players"))) {
                _playerMan = new PlayerManager(Path.Combine(path, "players"));
            }
        }

        protected bool LoadLevel ()
        {
            NBTFile nf = new NBTFile(Path.Combine(WorldPath, "level.dat"));
            Stream nbtstr = nf.GetDataInputStream();
            if (nbtstr == null) {
                return false;
            }

            NBT_Tree tree = new NBT_Tree(nbtstr);

            _level = new Level(this);
            _level = _level.LoadTreeSafe(tree.Root);

            return _level != null;
        }
    }

    public class AlphaWorld : NBTWorld
    {
        private ChunkFileManager _chunkMan;
        private BlockManager _blockMan;

        private string _dim;

        public AlphaWorld (string path)
            : base(path)
        {
            _chunkMan = new ChunkFileManager(path);
            _blockMan = new BlockManager(_chunkMan);
        }

        public AlphaWorld (string path, string dim)
            : base(path)
        {
            _dim = dim;
            if (_dim.Length > 0) {
                path = Path.Combine(path, dim);
            }

            _chunkMan = new ChunkFileManager(path);
            _blockMan = new BlockManager(_chunkMan);
        }

        public override IChunkManager ChunkManager
        {
            get { return _chunkMan; }
        }

        public override IBlockManager BlockManager
        {
            get { return _blockMan; }
        }
    }

    public class BetaWorld : NBTWorld
    {
        private RegionManager _regionMan;
        private ChunkManager _chunkMan;
        private BlockManager _blockMan;

        private string _dim;
        private string _regionDir;

        public RegionManager RegionManager
        {
            get { return _regionMan; }
        }

        public BetaWorld (string path)
            : this(path, "region", "")
        {
        }

        public BetaWorld (string path, string region)
            : this(path, region, "")
        {
        }

        public BetaWorld (string path, string region, string dim)
            : base(path)
        {
            _regionDir = region;

            _dim = dim;
            if (_dim.Length > 0) {
                path = Path.Combine(path, dim);
            }

            if (!Directory.Exists(Path.Combine(path, _regionDir))) {
                throw new Exception("Could not find region directory");
            }

            _regionMan = new RegionManager(Path.Combine(path, _regionDir));
            _chunkMan = new ChunkManager(_regionMan);
            _blockMan = new BlockManager(_chunkMan);
        }

        public override IChunkManager ChunkManager
        {
            get { return _chunkMan; }
        }

        public override IBlockManager BlockManager
        {
            get { return _blockMan; }
        }
    }

}
