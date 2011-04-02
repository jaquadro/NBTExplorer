using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    using NBT;
    using Utility;

    public interface IBlock
    {
        BlockInfo Info { get; }
        int ID { get; set; }
        int Data { get; set; }
        int BlockLight { get; set; }
        int SkyLight { get; set; }

        TileEntity GetTileEntity ();
        bool SetTileEntity (TileEntity te);
        bool ClearTileEntity ();
    }

    public class Block : IBlock, ICopyable<Block>
    {
        private int _id;
        private int _data;
        private int _skylight;
        private int _blocklight;

        private TileEntity _tileEntity;

        public BlockInfo Info
        {
            get { return BlockInfo.BlockTable[_id]; }
        }

        public int ID
        {
            get { return _id; }
            set
            {
                if (BlockInfo.SchemaTable[_id] != BlockInfo.SchemaTable[value]) {
                    _tileEntity = null;
                }
                _id = value;
            }
        }

        public int Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public int SkyLight
        {
            get { return _skylight; }
            set { _skylight = value; }
        }

        public int BlockLight
        {
            get { return _blocklight; }
            set { _blocklight = value; }
        }

        public Block (int id)
        {
            _id = id;
        }

        public Block (int id, int data)
        {
            _id = id;
            _data = data;
        }

        public Block (IChunk chunk, int lx, int ly, int lz)
        {
            _id = chunk.GetBlockID(lx, ly, lz);
            _data = chunk.GetBlockData(lx, ly, lz);
            _skylight = chunk.GetBlockSkyLight(lx, ly, lz);
            _blocklight = chunk.GetBlockLight(lx, ly, lz);
            _tileEntity = chunk.GetTileEntity(lx, ly, lz).Copy();
        }

        public TileEntity GetTileEntity ()
        {
            return _tileEntity;
        }

        public bool SetTileEntity (TileEntity te)
        {
            NBTCompoundNode schema = BlockInfo.SchemaTable[_id];
            if (schema == null) {
                return false;
            }

            if (te.Verify(schema) == false) {
                return false;
            }

            _tileEntity = te;
            return true;
        }

        public bool ClearTileEntity ()
        {
            _tileEntity = null;
            return true;
        }

        #region ICopyable<Block> Members

        public Block Copy ()
        {
            Block block = new Block(_id, _data);
            block._blocklight = _blocklight;
            block._skylight = _skylight;
            block._tileEntity = _tileEntity.Copy();

            return block;
        }

        #endregion
    }
}
