using System;
using System.Collections.Generic;
using System.Text;
using NBT;

namespace NBToolkit
{
    public class Chunk
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

        public Chunk (ChunkManager cm, int cx, int cz)
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

        public BlockRef GetBlockRef (int lx, int ly, int lz)
        {
            return new BlockRef(this, lx, ly, lz);
        }

        public int GetBlockID (int x, int y, int z)
        {
            if (_blocks == null) {
                _blocks = GetTree().Root.FindTagByName("Level").FindTagByName("Blocks").value.toByteArray();
            }

            return _blocks.Data[x << 11 | z << 7 | y];
        }

        public bool SetBlockID (int x, int y, int z, int id)
        {
            if (_blocks == null) {
                _blocks = GetTree().Root.FindTagByName("Level").FindTagByName("Blocks").value.toByteArray();
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
                _blocks = GetTree().Root.FindTagByName("Level").FindTagByName("Blocks").value.toByteArray();
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
                _data = new NibbleArray(GetTree().Root.FindTagByName("Level").FindTagByName("Data").value.toByteArray().Data);
            }

            return _data[x << 11 | z << 7 | y];
        }

        public bool SetBlockData (int x, int y, int z, int data)
        {
            if (_data == null) {
                _data = new NibbleArray(GetTree().Root.FindTagByName("Level").FindTagByName("Data").value.toByteArray().Data);
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
                _blockLight = new NibbleArray(GetTree().Root.FindTagByName("Level").FindTagByName("BlockLight").value.toByteArray().Data);
            }

            return _blockLight[x << 11 | z << 7 | y];
        }

        public bool SetBlockLight (int x, int y, int z, int light)
        {
            if (_blockLight == null) {
                _blockLight = new NibbleArray(GetTree().Root.FindTagByName("Level").FindTagByName("BlockLight").value.toByteArray().Data);
            }

            int index = x << 11 | z << 7 | y;
            if (_blockLight[index] == light) {
                return false;
            }

            _blockLight[index] = light;
            MarkDirty();

            return true;
        }

        public int GetSkyLight (int x, int y, int z)
        {
            if (_skyLight == null) {
                _skyLight = new NibbleArray(GetTree().Root.FindTagByName("Level").FindTagByName("SkyLight").value.toByteArray().Data);
            }

            return _skyLight[x << 11 | z << 7 | y];
        }

        public bool SetSkyLight (int x, int y, int z, int light)
        {
            if (_skyLight == null) {
                _skyLight = new NibbleArray(GetTree().Root.FindTagByName("Level").FindTagByName("SkyLight").value.toByteArray().Data);
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
            return GetTree().Root.FindTagByName("Level").FindTagByName("TerrainPopulated").value.toByte().Data == 1;
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

        public Chunk GetNorthNeighbor ()
        {
            return _chunkMan.GetChunk(_cx - 1, _cz);
        }

        public Chunk GetSouthNeighbor ()
        {
            return _chunkMan.GetChunk(_cx + 1, _cz);
        }

        public Chunk GetEastNeighbor ()
        {
            return _chunkMan.GetChunk(_cx, _cz - 1);
        }

        public Chunk GetWestNeighbor ()
        {
            return _chunkMan.GetChunk(_cx, _cz + 1);
        }
    }
}
