using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
    using NBT;

    public interface IBlock
    {
        BlockInfo Info { get; }
        int ID { get; set; }
        int Data { get; set; }
        int BlockLight { get; set; }
        int SkyLight { get; set; }
    }

    public class Block : IBlock
    {
        protected int _id;
        protected int _data;
        protected int _skylight;
        protected int _blocklight;

        protected NBT_Compound _tileEntities;

        public BlockInfo Info
        {
            get { return BlockInfo.BlockTable[_id]; }
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
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

        public Block (Block block)
        {
            _id = block._id;
            _data = block._data;
            _skylight = block._skylight;
            _blocklight = block._blocklight;
        }

        public Block (IBlock block)
        {
            _id = block.ID;
            _data = block.Data;
            _skylight = block.SkyLight;
            _blocklight = block.BlockLight;
        }

        public Block (ChunkRef chunk, int lx, int ly, int lz)
        {
            _id = chunk.GetBlockID(lx, ly, lz);
            _data = chunk.GetBlockData(lx, ly, lz);
            _skylight = chunk.GetBlockSkyLight(lx, ly, lz);
            _blocklight = chunk.GetBlockLight(lx, ly, lz);
        }

        public Block (BlockManager bm, int x, int y, int z)
            : this(bm.GetBlockRef(x, y, z))
        {
        }
    }
}
