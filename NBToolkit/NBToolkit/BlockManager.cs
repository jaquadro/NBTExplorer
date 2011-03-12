using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
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

        public BlockManager (ChunkManager cm)
        {
            _chunkMan = cm;
        }

        public BlockManager (BlockManager bm)
        {
            _chunkMan = bm._chunkMan;
        }
        
        public virtual int GetBlockID (int x, int y, int z)
        {
            if (x < MIN_X || x >= MAX_X)
                return 0;
            if (y < MIN_Y || y >= MAX_Y)
                return 0;
            if (z < MIN_Z || z >= MAX_Z)
                return 0;

            Chunk c = GetChunk(x, y, z);
            if (c == null) {
                return 0;
            }

            return c.GetBlockID(x & CHUNK_XMASK, y, z & CHUNK_ZMASK);
        }

        public virtual int GetBlockData (int x, int y, int z) {
            if (x < MIN_X || x >= MAX_X)
                return 0;
            if (y < MIN_Y || y >= MAX_Y)
                return 0;
            if (z < MIN_Z || z >= MAX_Z)
                return 0;

            Chunk c = GetChunk(x, y, z);
            if (c == null) {
                return 0;
            }

            return c.GetBlockData(x & CHUNK_XMASK, y, z & CHUNK_ZMASK);
        }

        public int GetBlockLight (int x, int y, int z) { return 0; }

        public int GetBlockSkylight (int x, int y, int z) { return 0; }

        public void SetBlock (int x, int y, int z, int id, int data) { }

        public virtual bool SetBlockID (int x, int y, int z, int id)
        {
            if (x < MIN_X || x >= MAX_X)
                return false;
            if (y < MIN_Y || y >= MAX_Y)
                return false;
            if (z < MIN_Z || z >= MAX_Z)
                return false;

            Chunk c = GetChunk(x, y, z);
            if (c == null) {
                return false;
            }

            return c.SetBlockID(x & CHUNK_XMASK, y, z & CHUNK_ZMASK, id);
        }

        public virtual bool SetBlockData (int x, int y, int z, int data)
        {
            if (x < MIN_X || x >= MAX_X)
                return false;
            if (y < MIN_Y || y >= MAX_Y)
                return false;
            if (z < MIN_Z || z >= MAX_Z)
                return false;

            Chunk c = GetChunk(x, y, z);
            if (c == null) {
                return false;
            }

            return c.SetBlockData(x & CHUNK_XMASK, y, z & CHUNK_ZMASK, data);
        }

        public Chunk GetChunk (int x, int y, int z)
        {
            x >>= CHUNK_XLOG;
            z >>= CHUNK_ZLOG;
            return _chunkMan.GetChunk(x, z);
        }
    }
}
