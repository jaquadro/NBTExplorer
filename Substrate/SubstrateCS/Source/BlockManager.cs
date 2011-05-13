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

        public bool AutoLight
        {
            get { return _autoLight; }
            set { _autoLight = value; }
        }

        public BlockManager (IChunkManager cm)
        {
            _chunkMan = cm;

            Chunk c = Chunk.Create(0, 0);

            _chunkXDim = c.Blocks.XDim;
            _chunkYDim = c.Blocks.YDim;
            _chunkZDim = c.Blocks.ZDim;
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

            return _cache.Blocks.GetBlock(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public BlockRef GetBlockRef (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return null;
            }

            return _cache.Blocks.GetBlockRef(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public void SetBlock (int x, int y, int z, Block block)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            _cache.Blocks.SetBlock(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, block);
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

        IBlock IBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IBlock IBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, IBlock block)
        {
            _cache.Blocks.SetBlock(x, y, z, block);
        }

        public BlockInfo GetInfo (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return null;
            }

            return _cache.Blocks.GetInfo(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public int GetID (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null) {
                return 0;
            }

            return _cache.Blocks.GetID(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public void SetID (int x, int y, int z, int id)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            bool autolight = _cache.Blocks.AutoLight;
            _cache.Blocks.AutoLight = _autoLight;

            _cache.Blocks.SetID(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, id);

            _cache.Blocks.AutoLight = autolight;
        }

        #endregion


        #region IDataBlockCollection Members

        IDataBlock IDataBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IDataBlock IDataBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, IDataBlock block)
        {
            _cache.Blocks.SetBlock(x, y, z, block);
        }

        public int GetData (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null) {
                return 0;
            }

            return _cache.Blocks.GetData(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public void SetData (int x, int y, int z, int data)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            _cache.Blocks.SetData(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, data);
        }

        #endregion


        #region ILitBlockContainer Members

        ILitBlock ILitBlockCollection.GetBlock (int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        ILitBlock ILitBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, ILitBlock block)
        {
            _cache.Blocks.SetBlock(x, y, z, block);
        }

        public int GetBlockLight (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null) {
                return 0;
            }

            return _cache.Blocks.GetBlockLight(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public int GetSkyLight (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null) {
                return 0;
            }

            return _cache.Blocks.GetSkyLight(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public void SetBlockLight (int x, int y, int z, int light)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            _cache.Blocks.SetBlockLight(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, light);
        }

        public void SetSkyLight (int x, int y, int z, int light)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            _cache.Blocks.SetSkyLight(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, light);
        }

        public int GetHeight (int x, int z)
        {
            _cache = GetChunk(x, 0, z);
            if (_cache == null || !Check(x, 0, z)) {
                return 0;
            }

            return _cache.Blocks.GetHeight(x & _chunkXMask, z & _chunkZMask);
        }

        public void SetHeight (int x, int z, int height)
        {
            _cache = GetChunk(x, 0, z);
            if (_cache == null || !Check(x, 0, z)) {
                return;
            }

            _cache.Blocks.SetHeight(x & _chunkXMask, z & _chunkZMask, height);
        }

        public void UpdateBlockLight (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            _cache.Blocks.UpdateBlockLight(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public void UpdateSkyLight (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            _cache.Blocks.UpdateBlockLight(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        #endregion


        #region IPropertyBlockContainer Members

        IPropertyBlock IPropertyBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IPropertyBlock IPropertyBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, IPropertyBlock block)
        {
            _cache.Blocks.SetBlock(x, y, z, block);
        }

        public TileEntity GetTileEntity (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return null;
            }

            return _cache.Blocks.GetTileEntity(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public void SetTileEntity (int x, int y, int z, TileEntity te)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            _cache.Blocks.SetTileEntity(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask, te);
        }

        public void CreateTileEntity (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            _cache.Blocks.CreateTileEntity(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        public void ClearTileEntity (int x, int y, int z)
        {
            _cache = GetChunk(x, y, z);
            if (_cache == null || !Check(x, y, z)) {
                return;
            }

            _cache.Blocks.ClearTileEntity(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);
        }

        #endregion
    }
}
