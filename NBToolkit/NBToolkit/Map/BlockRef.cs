using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
    public class BlockRef : IBlock
    {
        protected ChunkRef _chunk;

        protected int _lx;
        protected int _ly;
        protected int _lz;

        public int X
        {
            get { return _lx + (_chunk.X * BlockManager.CHUNK_XLEN); }
        }

        public int Y
        {
            get { return _ly; }
        }

        public int Z
        {
            get { return _lz + (_chunk.Z * BlockManager.CHUNK_ZLEN); }
        }

        public int LocalX
        {
            get { return _lx; }
        }

        public int LocalY
        {
            get { return _ly; }
        }

        public int LocalZ
        {
            get { return _lz; }
        }

        public BlockInfo Info
        {
            get { return BlockInfo.BlockTable[_chunk.GetBlockID(_lx, _ly, _lz)]; }
        }

        public int ID
        {
            get { return _chunk.GetBlockID(_lx, _ly, _lz); }
            set { _chunk.SetBlockID(_lx, _ly, _lz, value); }
        }

        public int Data
        {
            get { return _chunk.GetBlockData(_lx, _ly, _lz); }
            set { _chunk.SetBlockData(_lx, _ly, _lz, value); }
        }

        public int BlockLight
        {
            get { return _chunk.GetBlockLight(_lx, _ly, _lz); }
            set { _chunk.SetBlockLight(_lx, _ly, _lz, value); }
        }

        public int SkyLight
        {
            get { return _chunk.GetBlockSkyLight(_lx, _ly, _lz); }
            set { _chunk.SetBlockSkyLight(_lx, _ly, _lz, value); }
        }

        public BlockRef (ChunkRef c, int lx, int ly, int lz)
        {
            _chunk = c;
            _lx = lx;
            _ly = ly;
            _lz = lz;
        }

        public void CopyFrom (IBlock block)
        {
            ID = block.ID;
            Data = block.Data;
            BlockLight = block.BlockLight;
            SkyLight = block.SkyLight;
        }
    }
}
