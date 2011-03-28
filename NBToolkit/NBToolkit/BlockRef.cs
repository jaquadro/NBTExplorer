using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
    public class BlockRef
    {
        protected Chunk _chunk;

        protected int _lx;
        protected int _ly;
        protected int _lz;

        public int X
        {
            get
            {
                return _lx + (_chunk.X * BlockManager.CHUNK_XLEN);
            }
        }

        public int Y
        {
            get
            {
                return _ly;
            }
        }

        public int Z
        {
            get
            {
                return _lz + (_chunk.Z * BlockManager.CHUNK_ZLEN);
            }
        }

        public int LocalX
        {
            get
            {
                return _lx;
            }
        }

        public int LocalY
        {
            get
            {
                return _ly;
            }
        }

        public int LocalZ
        {
            get
            {
                return _lz;
            }
        }

        public int ID
        {
            get
            {
                return _chunk.GetBlockID(_lx, _ly, _lz);
            }

            set
            {
                _chunk.SetBlockID(_lx, _ly, _lz, value);
            }
        }

        public int Data
        {
            get
            {
                return _chunk.GetBlockData(_lx, _ly, _lz);
            }

            set
            {
                _chunk.SetBlockData(_lx, _ly, _lz, value);
            }
        }

        public int BlockLight
        {
            get
            {
                return _chunk.GetBlockLight(_lx, _ly, _lz);
            }

            set
            {
                _chunk.SetBlockLight(_lx, _ly, _lz, value);
            }
        }

        public int SkyLight
        {
            get
            {
                return _chunk.GetSkyLight(_lx, _ly, _lz);
            }

            set
            {
                _chunk.SetSkyLight(_lx, _ly, _lz, value);
            }
        }

        public BlockRef (BlockManager bm, int x, int y, int z)
        {
            _chunk = bm.GetChunk(x, y, z);
            _lx = x - _chunk.X * BlockManager.CHUNK_XLEN;
            _ly = y;
            _lz = z - _chunk.Z * BlockManager.CHUNK_ZLEN;
        }

        public BlockRef (Chunk c, int lx, int ly, int lz)
        {
            _chunk = c;
            _lx = lx;
            _ly = ly;
            _lz = lz;
        }
    }
}
