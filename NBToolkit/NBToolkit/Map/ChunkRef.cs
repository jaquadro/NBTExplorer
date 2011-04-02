using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
    using NBT;

    public interface IChunk
    {
        bool IsPopulated { get; set; }

        BlockInfo GetBlockInfo (int lx, int ly, int lz);

        int GetBlockID (int lx, int ly, int lz);
        int GetBlockData (int lx, int ly, int lz);
        int GetBlockLight (int lx, int ly, int lz);
        int GetBlockSkyLight (int lx, int ly, int lz);

        void GetBlockID (int lx, int ly, int lz, int id);
        void GetBlockData (int lx, int ly, int lz, int data);
        void GetBlockLight (int lx, int ly, int lz, int light);
        void GetBlockSkyLight (int lx, int ly, int lz, int light);

        //int CountBlocks (Predicate<BlockRef> match);

        int CountBlockID (int id);
        int CountBlockData (int id, int data);

        int GetHeight (int lx, int lz);

        NBT_Compound GetTileEntity (int lx, int ly, int lz);

        void AddTileEntity (int lx, int ly, int lz, string id, NBT_Compound data);
        void RemoveTileEntity (int lx, int ly, int lz);

        //IEnumerable<BlockRef> FilterBlocks (Predicate<BlockRef> match);
    }

    public class ChunkRef
    {
        protected int _cx;
        protected int _cz;

        protected NBT_Tree _nbt = null;
        protected NBT_ByteArray _blocks = null;
        protected NibbleArray _data = null;
        protected NibbleArray _blockLight = null;
        protected NibbleArray _skyLight = null;

        protected bool _dirty = false;

        protected ChunkManager _chunkMan;

        public int X
        {
            get
            {
                return _cx;
            }
        }

        public int Z
        {
            get
            {
                return _cz;
            }
        }

        public int LocalX
        {
            get
            {
                return _cx & ChunkManager.REGION_XMASK;
            }
        }

        public int LocalZ
        {
            get
            {
                return _cz & ChunkManager.REGION_ZMASK;
            }
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

        public bool Save ()
        {
            if (_dirty) {
                if (SaveTree()) {
                    _dirty = false;
                    return true;
                }
                return false;
            }

            return true;
        }

        protected NBT_Tree GetTree ()
        {
            if (_nbt != null) {
                return _nbt;
            }

            Region r = _chunkMan.GetRegion(_cx, _cz);
            if (r == null || !r.ChunkExists(LocalX, LocalZ)) {
                throw new MissingChunkException();
            }

            _nbt = r.GetChunkTree(LocalX, LocalZ);
            ChunkVerifier cv = new ChunkVerifier(_nbt);
            cv.Verify();

            return _nbt;
        }

        protected bool SaveTree ()
        {
            if (_nbt != null) {
                _blocks = null;
                _data = null;

                Region r = _chunkMan.GetRegion(_cx, _cz);
                if (r == null || !r.ChunkExists(LocalX, LocalZ)) {
                    throw new MissingChunkException();
                }

                return r.SaveChunkTree(LocalX, LocalZ, _nbt);
            }

            return false;
        }

        public Block GetBlock (int lx, int ly, int lz)
        {
            return new Block(this, lx, ly, lz);
        }

        public BlockRef GetBlockRef (int lx, int ly, int lz)
        {
            return new BlockRef(this, lx, ly, lz);
        }

        public int GetBlockID (int x, int y, int z)
        {
            if (_blocks == null) {
                _blocks = GetTree().Root["Level"].ToNBTCompound()["Blocks"].ToNBTByteArray();
            }

            return _blocks.Data[x << 11 | z << 7 | y];
        }

        public bool SetBlockID (int x, int y, int z, int id)
        {
            if (_blocks == null) {
                _blocks = GetTree().Root["Level"].ToNBTCompound()["Blocks"].ToNBTByteArray();
            }

            int index = x << 11 | z << 7 | y;
            if (_blocks.Data[index] == id) {
                return false;
            }

            _blocks.Data[index] = (byte)id;
            MarkDirty();

            return true;
        }

        public int CountBlockID (int id)
        {
            if (_blocks == null) {
                _blocks = GetTree().Root["Level"].ToNBTCompound()["Blocks"].ToNBTByteArray();
            }

            int c = 0;
            for (int i = 0; i < _blocks.Length; i++) {
                if (_blocks.Data[i] == id) {
                    c++;
                }
            }

            return c;
        }

        public int GetBlockData (int x, int y, int z)
        {
            if (_data == null) {
                _data = new NibbleArray(GetTree().Root["Level"].ToNBTCompound()["Data"].ToNBTByteArray().Data);
            }

            return _data[x << 11 | z << 7 | y];
        }

        public bool SetBlockData (int x, int y, int z, int data)
        {
            if (_data == null) {
                _data = new NibbleArray(GetTree().Root["Level"].ToNBTCompound()["Data"].ToNBTByteArray().Data);
            }

            int index = x << 11 | z << 7 | y;
            if (_data[index] == data) {
                return false;
            }

            _data[index] = data;
            MarkDirty();

            return true;
        }

        public int GetBlockLight (int x, int y, int z)
        {
            if (_blockLight == null) {
                _blockLight = new NibbleArray(GetTree().Root["Level"].ToNBTCompound()["BlockLight"].ToNBTByteArray().Data);
            }

            return _blockLight[x << 11 | z << 7 | y];
        }

        public bool SetBlockLight (int x, int y, int z, int light)
        {
            if (_blockLight == null) {
                _blockLight = new NibbleArray(GetTree().Root["Level"].ToNBTCompound()["BlockLight"].ToNBTByteArray().Data);
            }

            int index = x << 11 | z << 7 | y;
            if (_blockLight[index] == light) {
                return false;
            }

            _blockLight[index] = light;
            MarkDirty();

            return true;
        }

        public int GetBlockSkyLight (int x, int y, int z)
        {
            if (_skyLight == null) {
                _skyLight = new NibbleArray(GetTree().Root["Level"].ToNBTCompound()["SkyLight"].ToNBTByteArray().Data);
            }

            return _skyLight[x << 11 | z << 7 | y];
        }

        public bool SetBlockSkyLight (int x, int y, int z, int light)
        {
            if (_skyLight == null) {
                _skyLight = new NibbleArray(GetTree().Root["Level"].ToNBTCompound()["SkyLight"].ToNBTByteArray().Data);
            }

            int index = x << 11 | z << 7 | y;
            if (_skyLight[index] == light) {
                return false;
            }

            _skyLight[index] = light;
            MarkDirty();

            return true;
        }

        public bool IsPopulated ()
        {
            return GetTree().Root["Level"].ToNBTCompound()["TerrainPopulated"].ToNBTByte().Data == 1;
        }

        public NBT_Compound GetTileEntity (int x, int y, int z)
        {
            NBT_List telist = GetTree().Root["Level"].ToNBTCompound()["TileEntities"].ToNBTList();

            foreach (NBT_Compound te in telist) {
                if (te["x"].ToNBTInt().Data == x &&
                    te["y"].ToNBTInt().Data == y &&
                    te["z"].ToNBTInt().Data == z) {
                    return te;
                }
            }

            return null;
        }

        public bool RemoveTileEntity (int x, int y, int z)
        {
            NBT_Compound te = GetTileEntity(x, y, z);
            if (te == null) {
                return false;
            }

            NBT_List telist = GetTree().Root["Level"].ToNBTCompound()["TileEntities"].ToNBTList();

            return telist.Remove(te);
        }

        protected bool MarkDirty ()
        {
            if (_dirty) {
                return false;
            }

            _dirty = true;
            _chunkMan.MarkChunkDirty(this);
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
    }
}
