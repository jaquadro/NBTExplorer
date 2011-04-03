using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    public class BlockRef : IBlock
    {
        protected IBlockContainer _container;

        protected int _x;
        protected int _y;
        protected int _z;

        public int X
        {
            get { return _container.GlobalX(_x); }
        }

        public int Y
        {
            get { return _container.GlobalY(_y); }
        }

        public int Z
        {
            get { return _container.GlobalZ(_z); }
        }

        public int LocalX
        {
            get { return _container.LocalX(_x); }
        }

        public int LocalY
        {
            get { return _container.LocalZ(_z); }
        }

        public int LocalZ
        {
            get { return _z; }
        }

        public BlockInfo Info
        {
            get { return BlockInfo.BlockTable[_container.GetBlockID(_x, _y, _z)]; }
        }

        public int ID
        {
            get { return _container.GetBlockID(_x, _y, _z); }
            set { _container.SetBlockID(_x, _y, _z, value); }
        }

        public int Data
        {
            get { return _container.GetBlockData(_x, _y, _z); }
            set { _container.SetBlockData(_x, _y, _z, value); }
        }

        public int BlockLight
        {
            get { return _container.GetBlockLight(_x, _y, _z); }
            set { _container.SetBlockLight(_x, _y, _z, value); }
        }

        public int SkyLight
        {
            get { return _container.GetBlockSkyLight(_x, _y, _z); }
            set { _container.SetBlockSkyLight(_x, _y, _z, value); }
        }

        public BlockRef (IBlockContainer container, int x, int y, int z)
        {
            _container = container;
            _x = x;
            _y = y;
            _z = z;
        }

        public void CopyFrom (IBlock block)
        {
            ID = block.ID;
            Data = block.Data;
            BlockLight = block.BlockLight;
            SkyLight = block.SkyLight;

            SetTileEntity(block.GetTileEntity().Copy());
        }

        public TileEntity GetTileEntity ()
        {
            return _container.GetTileEntity(_x, _y, _z);
        }

        public bool SetTileEntity (TileEntity te)
        {
            return _container.SetTileEntity(_x, _y, _z, te);
        }

        public bool ClearTileEntity ()
        {
            return _container.ClearTileEntity(_x, _y, _z);
        }
    }

    /*public class BlockRef : IBlock
    {
        protected IChunk _chunk;

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

        public BlockRef (IChunk c, int lx, int ly, int lz)
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

        public TileEntity GetTileEntity ()
        {
            return _chunk.GetTileEntity(_lx, _ly, _lz);
        }

        public bool SetTileEntity (TileEntity te)
        {
            return _chunk.SetTileEntity(_lx, _ly, _lz, te);
        }

        public bool ClearTileEntity ()
        {
            return _chunk.ClearTileEntity(_lx, _ly, _lz);
        }
    }*/
}
