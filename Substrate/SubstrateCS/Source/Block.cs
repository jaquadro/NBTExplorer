using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;
    using Utility;

    public class Block : IDataBlock, IPropertyBlock, ICopyable<Block>
    {
        private int _id;
        private int _data;

        private TileEntity _tileEntity;

        public Block (int id)
        {
            _id = id;
        }

        public Block (int id, int data)
        {
            _id = id;
            _data = data;
        }

        public Block (IAlphaBlockCollection chunk, int lx, int ly, int lz)
        {
            _id = chunk.GetID(lx, ly, lz);
            _data = chunk.GetData(lx, ly, lz);
            _tileEntity = chunk.GetTileEntity(lx, ly, lz).Copy();
        }


        #region IBlock Members

        public BlockInfo Info
        {
            get { return BlockInfo.BlockTable[_id]; }
        }

        public int ID
        {
            get { return _id; }
            set
            {
                BlockInfoEx info1 = BlockInfo.BlockTable[_id] as BlockInfoEx;
                BlockInfoEx info2 = BlockInfo.BlockTable[value] as BlockInfoEx;

                if (info1 != info2) {
                    if (info1 != null) {
                        _tileEntity = null;
                    }

                    if (info2 != null) {
                        _tileEntity = TileEntityFactory.Create(info2.TileEntityName);
                    }
                }

                _id = value;
            }
        }

        #endregion


        #region IDataBlock Members

        public int Data
        {
            get { return _data; }
            set
            {
                /*if (BlockManager.EnforceDataLimits && BlockInfo.BlockTable[_id] != null) {
                    if (!BlockInfo.BlockTable[_id].TestData(value)) {
                        return;
                    }
                }*/
                _data = value;
            }
        }

        #endregion


        #region IPropertyBlock Members

        public TileEntity GetTileEntity ()
        {
            return _tileEntity;
        }

        public void SetTileEntity (TileEntity te)
        {
            BlockInfoEx info = BlockInfo.BlockTable[_id] as BlockInfoEx;
            if (info == null) {
                return;
            }

            if (te.GetType() != TileEntityFactory.Lookup(info.TileEntityName)) {
                return;
            }

            _tileEntity = te;
        }

        public void ClearTileEntity ()
        {
            _tileEntity = null;
        }

        #endregion


        #region ICopyable<Block> Members

        public Block Copy ()
        {
            Block block = new Block(_id, _data);
            block._tileEntity = _tileEntity.Copy();

            return block;
        }

        #endregion
    }
}
