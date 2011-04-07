using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Substrate
{
    using NBT;

    public class World
    {
        protected RegionManager _regionMan;
        protected IChunkManager _chunkMan;
        protected IBlockManager _blockMan;
        protected PlayerManager _playerMan;

        protected string _worldPath;

        protected Level _level;

        public string WorldPath
        {
            get { return _worldPath; }
        }

        public World (string world)
        {
            _worldPath = world;

            if (!File.Exists(Path.Combine(_worldPath, "level.dat"))) {
                throw new Exception("Could not locate level.dat");
            }

            if (!LoadLevel()) {
                throw new Exception("Failed to load level.dat");
            }

            if (Directory.Exists(Path.Combine(world, "region"))) {
                _regionMan = new RegionManager(Path.Combine(world, "region"));
                _chunkMan = new ChunkManager(_regionMan);
            }
            else if (Directory.Exists(Path.Combine(world, "0"))) {
                _chunkMan = new ChunkFileManager(world);
            }
            else {
                throw new Exception("Could not locate any world data");
            }

            _blockMan = new BlockManager(_chunkMan);

            if (Directory.Exists(Path.Combine(world, "players"))) {
                _playerMan = new PlayerManager(Path.Combine(world, "players"));
            }
        }

        protected bool LoadLevel ()
        {
            NBTFile nf = new NBTFile(Path.Combine(_worldPath, "level.dat"));
            Stream nbtstr = nf.GetDataInputStream();
            if (nbtstr == null) {
                return false;
            }

            _level = new Level(this).LoadTreeSafe(new NBT_Tree(nbtstr).Root);

            return _level != null;
        }

        public RegionManager GetRegionManager ()
        {
            return _regionMan;
        }

        public IChunkManager GetChunkManager ()
        {
            return _chunkMan;
        }

        public IBlockManager GetBlockManager ()
        {
            return _blockMan;
        }

        public PlayerManager GetPlayerManager ()
        {
            return _playerMan;
        }
    }
}
