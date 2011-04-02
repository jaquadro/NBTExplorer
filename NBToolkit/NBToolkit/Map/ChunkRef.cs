using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NBToolkit.Map
{
    using NBT;

    public class ChunkRef : IChunk
    {
        private ChunkManager _chunkMan;
        private Chunk _chunk;

        private int _cx;
        private int _cz;

        private bool _dirty;

        public int X
        {
            get { return _cx; }
        }

        public int Z
        {
            get { return _cz; }
        }

        public int LocalX
        {
            get { return _cx & ChunkManager.REGION_XMASK; }
        }

        public int LocalZ
        {
            get { return _cz & ChunkManager.REGION_ZMASK; }
        }

        public ChunkRef (ChunkManager cm, int cx, int cz)
        {
            _chunkMan = cm;
            _cx = cx;
            _cz = cz;

            Region r = cm.GetRegion(cx, cz);
            if (r == null || !r.ChunkExists(LocalX, LocalZ)) {
                throw new MissingChunkException();
            }
        }

        private Chunk GetChunk ()
        {
            if (_chunk == null) {
                _chunk = new Chunk(GetTree());
            }
            return _chunk;
        }

        private NBT_Tree GetTree ()
        {
            Region r = _chunkMan.GetRegion(_cx, _cz);
            if (r == null || !r.ChunkExists(LocalX, LocalZ)) {
                throw new MissingChunkException();
            }

            return r.GetChunkTree(LocalX, LocalZ);
        }

        private bool MarkDirty ()
        {
            if (_dirty) {
                return false;
            }

            _dirty = true;
            _chunkMan.MarkChunkDirty(this);
            return true;
        }

        public bool Save ()
        {
            if (_dirty) {
                Region r = _chunkMan.GetRegion(_cx, _cz);
                if (r == null || !r.ChunkExists(LocalX, LocalZ)) {
                    throw new MissingChunkException();
                }

                if (GetChunk().Save(r.GetChunkOutStream(_cx, _cz))) {
                    _dirty = false;
                    return true;
                }
                return false;
            }
            return true;
        }

        public ChunkRef GetNorthNeighbor ()
        {
            return _chunkMan.GetChunk(_cx - 1, _cz);
        }

        public ChunkRef GetSouthNeighbor ()
        {
            return _chunkMan.GetChunk(_cx + 1, _cz);
        }

        public ChunkRef GetEastNeighbor ()
        {
            return _chunkMan.GetChunk(_cx, _cz - 1);
        }

        public ChunkRef GetWestNeighbor ()
        {
            return _chunkMan.GetChunk(_cx, _cz + 1);
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
