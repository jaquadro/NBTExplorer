using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    public class BlockManager
    {
        public const int MIN_X = -32000000;
        public const int MAX_X = 32000000;
        public const int MIN_Y = 0;
        public const int MAX_Y = 128;
        public const int MIN_Z = -32000000;
        public const int MAX_Z = 32000000;

        public const int CHUNK_XLEN = 16;
        public const int CHUNK_YLEN = 128;
        public const int CHUNK_ZLEN = 16;

        public const int CHUNK_XLOG = 4;
        public const int CHUNK_YLOG = 7;
        public const int CHUNK_ZLOG = 4;

        public const int CHUNK_XMASK = 0xF;
        public const int CHUNK_YMASK = 0x7F;
        public const int CHUNK_ZMASK = 0xF;

        protected ChunkManager _chunkMan;

        protected ChunkRef _cache;

        public BlockManager (ChunkManager cm)
        {
            _chunkMan = cm;
        }

        public BlockManager (BlockManager bm)
        {
            _chunkMan = bm._chunkMan;
        }

        public virtual Block GetBlock (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return null;
            }

            return new Block(_cache, x & CHUNK_XMASK, y, z & CHUNK_ZMASK);
        }

        public virtual BlockRef GetBlockRef (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return null;
            }

            return new BlockRef(_cache, x & CHUNK_XMASK, y, z & CHUNK_ZMASK);
        }
        
        public virtual bool GetBlockID (int x, int y, int z, out int id)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                id = 0;
                return false;
            }

            id = _cache.GetBlockID(x & CHUNK_XMASK, y, z & CHUNK_ZMASK);
            return true;
        }

        public virtual bool GetBlockData (int x, int y, int z, out int data) {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                data = 0;
                return false;
            }

            data = _cache.GetBlockData(x & CHUNK_XMASK, y, z & CHUNK_ZMASK);
            return true;
        }

        public virtual bool GetBlockLight (int x, int y, int z, out int light) {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                light = 0;
                return false;
            }

            light = _cache.GetBlockLight(x & CHUNK_XMASK, y, z & CHUNK_ZMASK);
            return true;
        }

        public virtual bool GetBlockSkyLight (int x, int y, int z, out int light) {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                light = 0;
                return false;
            }

            light = _cache.GetBlockSkyLight(x & CHUNK_XMASK, y, z & CHUNK_ZMASK);
            return true;
        }

        public virtual bool SetBlockID (int x, int y, int z, int id)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return false;
            }

            return _cache.SetBlockID(x & CHUNK_XMASK, y, z & CHUNK_ZMASK, id);
        }

        public virtual bool SetBlockData (int x, int y, int z, int data)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return false;
            }

            return _cache.SetBlockData(x & CHUNK_XMASK, y, z & CHUNK_ZMASK, data);
        }

        public ChunkRef GetChunk (int x, int y, int z)
        {
            x >>= CHUNK_XLOG;
            z >>= CHUNK_ZLOG;
            return _chunkMan.GetChunk(x, z);
        }

        /// <summary>
        /// Called by other block-specific 'get' and 'set' functions to filter
        /// out operations on some blocks.  Override this method in derrived
        /// classes to filter the entire BlockManager.
        /// </summary>
        protected virtual bool Check (int x, int y, int z) {
            return (x >= MIN_X) && (x < MAX_X) &&
                (y >= MIN_Y) && (y < MAX_Y) &&
                (z >= MIN_Z) && (z < MAX_Z);
        }
    }
}
