using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Substrate.Map
{
    using NBT;

    public class ChunkRef : IChunk
    {
        private IChunkContainer _container;
        private IChunkCache _cache;
        private Chunk _chunk;

        private int _cx;
        private int _cz;

        private bool _dirty;

        public int X
        {
            get { return _container.ChunkGlobalX(_cx); }
        }

        public int Z
        {
            get { return _container.ChunkGlobalZ(_cz); }
        }

        public int LocalX
        {
            get { return _container.ChunkLocalX(_cx); }
        }

        public int LocalZ
        {
            get { return _container.ChunkLocalZ(_cz); }
        }

        public ChunkRef (IChunkContainer container, IChunkCache cache, int cx, int cz)
        {
            _container = container;
            _cache = cache;
            _cx = cx;
            _cz = cz;

            if (!_container.ChunkExists(cx, cz)) {
                throw new MissingChunkException();
            }
        }

        public int BlockGlobalX (int x)
        {
            return _container.ChunkGlobalX(_cx) * BlockManager.CHUNK_XLEN + x;
        }

        public int BlockGlobalY (int y)
        {
            return y;
        }

        public int BlockGlobalZ (int z)
        {
            return _container.ChunkGlobalZ(_cz) * BlockManager.CHUNK_ZLEN + z;
        }

        public int BlockLocalX (int x)
        {
            return x;
        }

        public int BlockLocalY (int y)
        {
            return y;
        }

        public int BlockLocalZ (int z)
        {
            return z;
        }

        private Chunk GetChunk ()
        {
            if (_chunk == null) {
                _chunk = _container.GetChunk(_cx, _cz);
            }
            return _chunk;
        }

        private bool MarkDirty ()
        {
            if (_dirty) {
                return false;
            }

            _dirty = true;
            _cache.MarkChunkDirty(this);
            return true;
        }

        public ChunkRef GetNorthNeighbor ()
        {
            return _container.GetChunkRef(_cx - 1, _cz);
        }

        public ChunkRef GetSouthNeighbor ()
        {
            return _container.GetChunkRef(_cx + 1, _cz);
        }

        public ChunkRef GetEastNeighbor ()
        {
            return _container.GetChunkRef(_cx, _cz - 1);
        }

        public ChunkRef GetWestNeighbor ()
        {
            return _container.GetChunkRef(_cx, _cz + 1);
        }

        public Chunk GetChunkCopy ()
        {
            return GetChunk().Copy();
        }

        public Chunk GetChunkRef ()
        {
            Chunk chunk = GetChunk();
            _chunk = null;

            return chunk;
        }

        public void SetChunkRef (Chunk chunk)
        {
            _chunk = chunk;
            _chunk.SetLocation(_cx, _cz);
            MarkDirty();
        }

        #region IChunk Members

        public bool IsTerrainPopulated
        {
	        get { return GetChunk().IsTerrainPopulated; }
	        set 
            {
                if (GetChunk().IsTerrainPopulated != value) {
                    GetChunk().IsTerrainPopulated = value; 
                    MarkDirty();
                }
            }
        }

        public bool Save (Stream outStream)
        {
            if (_dirty) {
                if (GetChunk().Save(outStream)) {
                    _dirty = false;
                    return true;
                }
                return false;
            }
            return true;
        }

        public Block GetBlock (int lx, int ly, int lz)
        {
 	        return new Block(this, lx, ly, lz);
        }

        public BlockRef GetBlockRef (int lx, int ly, int lz)
        {
 	        return new BlockRef(this, lx, ly, lz);
        }

        public BlockInfo GetBlockInfo (int lx, int ly, int lz)
        {
 	        return GetChunk().GetBlockInfo(lx, ly, lz);
        }

        public void SetBlock (int lx, int ly, int lz, Block block)
        {
            GetChunk().SetBlock(lx, ly, lz, block);
        }

        public int GetBlockID (int lx, int ly, int lz)
        {
 	        return GetChunk().GetBlockID(lx, ly, lz);
        }

        public int GetBlockData (int lx, int ly, int lz)
        {
 	        return GetChunk().GetBlockData(lx, ly, lz);
        }

        public int GetBlockLight (int lx, int ly, int lz)
        {
 	        return GetChunk().GetBlockSkyLight(lx, ly, lz);
        }

        public int GetBlockSkyLight (int lx, int ly, int lz)
        {
 	        return GetChunk().GetBlockSkyLight(lx, ly, lz);
        }

        public bool SetBlockID (int lx, int ly, int lz, int id)
        {
 	        if (GetChunk().SetBlockID(lx, ly, lz, id)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        public bool SetBlockData (int lx, int ly, int lz, int data)
        {
 	        if (GetChunk().SetBlockData(lx, ly, lz, data)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        public bool SetBlockLight (int lx, int ly, int lz, int light)
        {
 	        if (GetChunk().SetBlockLight(lx, ly, lz, light)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        public bool SetBlockSkyLight (int lx, int ly, int lz, int light)
        {
 	        if (GetChunk().SetBlockSkyLight(lx, ly, lz, light)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        public int CountBlockID (int id)
        {
 	        return GetChunk().CountBlockID(id);
        }

        public int CountBlockData (int id, int data)
        {
 	        return GetChunk().CountBlockData(id, data);
        }

        public int GetHeight (int lx, int lz)
        {
 	        return GetChunk().GetHeight(lx, lz);
        }

        public TileEntity GetTileEntity (int lx, int ly, int lz)
        {
 	        return GetChunk().GetTileEntity(lx, ly, lz);
        }

        public bool SetTileEntity (int lx, int ly, int lz, TileEntity te)
        {
 	        if (GetChunk().SetTileEntity(lx, ly, lz, te)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        public bool ClearTileEntity (int lx, int ly, int lz)
        {
 	        if (GetChunk().ClearTileEntity(lx, ly, lz)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        #endregion
    }

        /*public bool VerifyTileEntities ()
        {
            bool pass = true;

            NBT_List telist = GetTree().Root["Level"].ToNBTCompound()["TileEntities"].ToNBTList();

            foreach (NBT_Value val in telist) {
                NBT_Compound tree = val as NBT_Compound;
                if (tree == null) {
                    pass = false;
                    continue;
                }

                if (new NBTVerifier(tree, TileEntity.BaseSchema).Verify() == false) {
                    pass = false;
                    continue;
                }

                int x = tree["x"].ToNBTInt() & BlockManager.CHUNK_XMASK;
                int y = tree["y"].ToNBTInt() & BlockManager.CHUNK_YMASK;
                int z = tree["z"].ToNBTInt() & BlockManager.CHUNK_ZMASK;
                int id = GetBlockID(x, y, z);

                NBTCompoundNode schema = BlockInfo.SchemaTable[id];
                if (schema == null) {
                    pass = false;
                    continue;
                }

                pass = new NBTVerifier(tree, schema).Verify() && pass;
            }

            return pass;
        }

        private static bool LocalBounds (int lx, int ly, int lz)
        {
            return lx >= 0 && lx < BlockManager.CHUNK_XLEN &&
                ly >= 0 && ly < BlockManager.CHUNK_YLEN &&
                lz >= 0 && lz < BlockManager.CHUNK_ZLEN;
        }*/

    public class MalformedNBTTreeException : Exception { }
}
