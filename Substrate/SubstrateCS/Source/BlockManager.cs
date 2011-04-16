using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{

    public class BlockManager : IBlockManager
    {
        public const int MIN_X = -32000000;
        public const int MAX_X = 32000000;
        public const int MIN_Y = 0;
        public const int MAX_Y = 128;
        public const int MIN_Z = -32000000;
        public const int MAX_Z = 32000000;

        protected int _chunkXDim;
        protected int _chunkYDim;
        protected int _chunkZDim;
        protected int _chunkXMask;
        protected int _chunkYMask;
        protected int _chunkZMask;
        protected int _chunkXLog;
        protected int _chunkYLog;
        protected int _chunkZLog;

        protected IChunkManager _chunkMan;

        protected ChunkRef _cache;

        private bool _autoLight = true;

        public bool AutoRecalcLight
        {
            get { return _autoLight; }
            set { _autoLight = value; }
        }

        public BlockManager (IChunkManager cm)
        {
            _chunkMan = cm;

            Chunk c = new Chunk(0, 0);

            _chunkXDim = c.XDim;
            _chunkYDim = c.YDim;
            _chunkZDim = c.ZDim;
            _chunkXMask = _chunkXDim - 1;
            _chunkYMask = _chunkYDim - 1;
            _chunkZMask = _chunkZDim - 1;
            _chunkXLog = Log2(_chunkXDim);
            _chunkYLog = Log2(_chunkYDim);
            _chunkZLog = Log2(_chunkZDim);
        }

        public Block GetBlock (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return null;
            }

            return _cache.GetBlock(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public BlockRef GetBlockRef (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return null;
            }

            return _cache.GetBlockRef(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public void SetBlock (int x, int y, int z, Block block)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            _cache.SetBlock(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, block);
        }

        protected ChunkRef GetChunk (int x, int y, int z)
        {
            x >>= _chunkXLog;
            z >>= _chunkZLog;
            return _chunkMan.GetChunkRef(x, z);
        }

        private int Log2 (int x)
        {
            int c = 0;
            while (x > 1) {
                x >>= 1;
                c++;
            }
            return c;
        }

        /// <summary>
        /// Called by other block-specific 'get' and 'set' functions to filter
        /// out operations on some blocks.  Override this method in derrived
        /// classes to filter the entire BlockManager.
        /// </summary>
        protected virtual bool Check (int x, int y, int z)
        {
            return (x >= MIN_X) && (x < MAX_X) &&
                (y >= MIN_Y) && (y < MAX_Y) &&
                (z >= MIN_Z) && (z < MAX_Z);
        }

        #region IBlockContainer Members

        IBlock IBlockContainer.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IBlock IBlockContainer.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, IBlock block)
        {
            _cache.SetBlock(x, y, z, block);
        }

        public BlockInfo GetBlockInfo (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return null;
            }

            return _cache.GetBlockInfo(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public int GetBlockID (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null) {
                return 0;
            }

            return _cache.GetBlockID(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public int GetBlockData (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null) {
                return 0;
            }

            return _cache.GetBlockData(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public bool SetBlockID (int x, int y, int z, int id)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return false;
            }

            bool autolight = _cache.AutoRecalcLight;
            _cache.AutoRecalcLight = _autoLight;

            bool ret = _cache.SetBlockID(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, id);

            _cache.AutoRecalcLight = autolight;

            return ret;
        }

        public bool SetBlockData (int x, int y, int z, int data)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return false;
            }

            return _cache.SetBlockData(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, data);
        }

        #endregion


        #region ILitBlockContainer Members

        ILitBlock ILitBlockContainer.GetBlock (int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        ILitBlock ILitBlockContainer.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, ILitBlock block)
        {
            _cache.SetBlock(x, y, z, block);
        }

        public int GetBlockLight (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null) {
                return 0;
            }

            return _cache.GetBlockLight(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public int GetBlockSkyLight (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null) {
                return 0;
            }

            return _cache.GetBlockSkyLight(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public bool SetBlockLight (int x, int y, int z, int light)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return false;
            }

            return _cache.SetBlockID(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, light);
        }

        public bool SetBlockSkyLight (int x, int y, int z, int light)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return false;
            }

            return _cache.SetBlockSkyLight(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, light);
        }

        #endregion


        #region IPropertyBlockContainer Members

        IPropertyBlock IPropertyBlockContainer.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IPropertyBlock IPropertyBlockContainer.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, IPropertyBlock block)
        {
            _cache.SetBlock(x, y, z, block);
        }

        public TileEntity GetTileEntity (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return null;
            }

            return _cache.GetTileEntity(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public bool SetTileEntity (int x, int y, int z, TileEntity te)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return false;
            }

            return _cache.SetTileEntity(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, te);
        }

        public bool ClearTileEntity (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return false;
            }

            return _cache.ClearTileEntity(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        #endregion
    }
}
