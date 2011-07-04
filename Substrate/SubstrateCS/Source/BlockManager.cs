using System;
using Substrate.Core;

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

        protected int chunkXDim;
        protected int chunkYDim;
        protected int chunkZDim;
        protected int chunkXMask;
        protected int chunkYMask;
        protected int chunkZMask;
        protected int chunkXLog;
        protected int chunkYLog;
        protected int chunkZLog;

        protected IChunkManager chunkMan;

        protected ChunkRef cache;

        private bool _autoLight = true;
        private bool _autoFluid = false;

        public bool AutoLight
        {
            get { return _autoLight; }
            set { _autoLight = value; }
        }

        public bool AutoFluid
        {
            get { return _autoFluid; }
            set { _autoFluid = value; }
        }

        public BlockManager (IChunkManager cm)
        {
            chunkMan = cm;

            Chunk c = Chunk.Create(0, 0);

            chunkXDim = c.Blocks.XDim;
            chunkYDim = c.Blocks.YDim;
            chunkZDim = c.Blocks.ZDim;
            chunkXMask = chunkXDim - 1;
            chunkYMask = chunkYDim - 1;
            chunkZMask = chunkZDim - 1;
            chunkXLog = Log2(chunkXDim);
            chunkYLog = Log2(chunkYDim);
            chunkZLog = Log2(chunkZDim);
        }

        public AlphaBlock GetBlock (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return null;
            }

            return cache.Blocks.GetBlock(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        public AlphaBlockRef GetBlockRef (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return new AlphaBlockRef();
            }

            return cache.Blocks.GetBlockRef(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        public void SetBlock (int x, int y, int z, AlphaBlock block)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return;
            }

            cache.Blocks.SetBlock(x & chunkXMask, y & chunkYMask, z & chunkZMask, block);
        }

        protected ChunkRef GetChunk (int x, int y, int z)
        {
            x >>= chunkXLog;
            z >>= chunkZLog;
            return chunkMan.GetChunkRef(x, z);
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
            cache.Blocks.SetBlock(x, y, z, block);
        }

        public BlockInfo GetInfo (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return null;
            }

            return cache.Blocks.GetInfo(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        public int GetID (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null) {
                return 0;
            }

            return cache.Blocks.GetID(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        public void SetID (int x, int y, int z, int id)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return;
            }

            bool autolight = cache.Blocks.AutoLight;
            bool autofluid = cache.Blocks.AutoFluid;

            cache.Blocks.AutoLight = _autoLight;
            cache.Blocks.AutoFluid = _autoFluid;

            cache.Blocks.SetID(x & chunkXMask, y & chunkYMask, z & chunkZMask, id);

            cache.Blocks.AutoFluid = autofluid;
            cache.Blocks.AutoLight = autolight;
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
            cache.Blocks.SetBlock(x, y, z, block);
        }

        public int GetData (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null) {
                return 0;
            }

            return cache.Blocks.GetData(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        public void SetData (int x, int y, int z, int data)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return;
            }

            cache.Blocks.SetData(x & chunkXMask, y & chunkYMask, z & chunkZMask, data);
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
            cache.Blocks.SetBlock(x, y, z, block);
        }

        public int GetBlockLight (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null) {
                return 0;
            }

            return cache.Blocks.GetBlockLight(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        public int GetSkyLight (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null) {
                return 0;
            }

            return cache.Blocks.GetSkyLight(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        public void SetBlockLight (int x, int y, int z, int light)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return;
            }

            cache.Blocks.SetBlockLight(x & chunkXMask, y & chunkYMask, z & chunkZMask, light);
        }

        public void SetSkyLight (int x, int y, int z, int light)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return;
            }

            cache.Blocks.SetSkyLight(x & chunkXMask, y & chunkYMask, z & chunkZMask, light);
        }

        public int GetHeight (int x, int z)
        {
            cache = GetChunk(x, 0, z);
            if (cache == null || !Check(x, 0, z)) {
                return 0;
            }

            return cache.Blocks.GetHeight(x & chunkXMask, z & chunkZMask);
        }

        public void SetHeight (int x, int z, int height)
        {
            cache = GetChunk(x, 0, z);
            if (cache == null || !Check(x, 0, z)) {
                return;
            }

            cache.Blocks.SetHeight(x & chunkXMask, z & chunkZMask, height);
        }

        public void UpdateBlockLight (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return;
            }

            cache.Blocks.UpdateBlockLight(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        public void UpdateSkyLight (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return;
            }

            cache.Blocks.UpdateBlockLight(x & chunkXMask, y & chunkYMask, z & chunkZMask);
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
            cache.Blocks.SetBlock(x, y, z, block);
        }

        public TileEntity GetTileEntity (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return null;
            }

            return cache.Blocks.GetTileEntity(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        public void SetTileEntity (int x, int y, int z, TileEntity te)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return;
            }

            cache.Blocks.SetTileEntity(x & chunkXMask, y & chunkYMask, z & chunkZMask, te);
        }

        public void CreateTileEntity (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return;
            }

            cache.Blocks.CreateTileEntity(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        public void ClearTileEntity (int x, int y, int z)
        {
            cache = GetChunk(x, y, z);
            if (cache == null || !Check(x, y, z)) {
                return;
            }

            cache.Blocks.ClearTileEntity(x & chunkXMask, y & chunkYMask, z & chunkZMask);
        }

        #endregion
    }
}
