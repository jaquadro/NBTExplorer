using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    public class BlockRef : IDataBlock, IPropertyBlock, ILitBlock
    {
        protected IAlphaBlockCollection _container;

        protected int _x;
        protected int _y;
        protected int _z;

        /*public int X
        {
            get { return _container.BlockGlobalX(_x); }
        }

        public int Y
        {
            get { return _container.BlockGlobalY(_y); }
        }

        public int Z
        {
            get { return _container.BlockGlobalZ(_z); }
        }

        public int LocalX
        {
            get { return _container.BlockLocalX(_x); }
        }

        public int LocalY
        {
            get { return _container.BlockLocalZ(_z); }
        }

        public int LocalZ
        {
            get { return _z; }
        }*/


        public BlockRef (IAlphaBlockCollection container, int x, int y, int z)
        {
            _container = container;
            _x = x;
            _y = y;
            _z = z;
        }


        #region IBlock Members

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

        #endregion


        #region ILitBlock Members

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

        #endregion


        #region IPropertyBlock Members

        public TileEntity GetTileEntity ()
        {
            return _container.GetTileEntity(_x, _y, _z);
        }

        public void SetTileEntity (TileEntity te)
        {
            _container.SetTileEntity(_x, _y, _z, te);
        }

        public void ClearTileEntity ()
        {
            _container.ClearTileEntity(_x, _y, _z);
        }

        #endregion
    }
}
