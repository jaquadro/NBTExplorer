using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NBToolkit.Map
{
    public class World
    {
        protected RegionManager _regionMan;
        protected ChunkManager _chunkMan;
        protected BlockManager _blockMan;

        protected string _worldPath;

        public World (string world)
        {
            _worldPath = world;

            _regionMan = new RegionManager(Path.Combine(world, "region"));
            _chunkMan = new ChunkManager(_regionMan);
            _blockMan = new BlockManager(_chunkMan);
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
    }
}
