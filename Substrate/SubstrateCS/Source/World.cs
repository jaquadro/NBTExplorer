using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Substrate
{
    public class World
    {
        protected RegionManager _regionMan;
        protected ChunkManager _chunkMan;
        protected BlockManager _blockMan;
        protected PlayerManager _playerMan;

        protected string _worldPath;

        public World (string world)
        {
            _worldPath = world;

            if (Directory.Exists(Path.Combine(world, "region"))) {
                _regionMan = new RegionManager(Path.Combine(world, "region"));
                _chunkMan = new ChunkManager(_regionMan);
            }
            else if (Directory.Exists(Path.Combine(world, "0"))) {
                //_chunkMan = new ChunkFileManager(world);
            }

            _blockMan = new BlockManager(_chunkMan);

            if (Directory.Exists(Path.Combine(world, "players"))) {
                _playerMan = new PlayerManager(Path.Combine(world, "players"));
            }
        }

        public RegionManager GetRegionManager ()
        {
            return _regionMan;
        }

        public ChunkManager GetChunkManager ()
        {
            return _chunkMan;
        }

        public BlockManager GetBlockManager ()
        {
            return _blockMan;
        }

        public PlayerManager GetPlayerManager ()
        {
            return _playerMan;
        }
    }
}
