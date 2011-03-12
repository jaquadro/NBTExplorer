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

        protected bool _dirty = false;

        protected ChunkManager _chunkMan;

        public Chunk (ChunkManager cm, int cx, int cz)
        {
            _chunkMan = cm;
            _cx = cx;
            _cz = cz;
        }

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
            _nbt = r.GetChunkTree(_cx & ChunkManager.REGION_XMASK, _cz & ChunkManager.REGION_ZMASK);

            return _nbt;
        }

        protected bool SaveTree ()
        {
            if (_nbt != null) {
                _blocks = null;
                _data = null;

                Region r = _chunkMan.GetRegion(_cx, _cz);
                return r.SaveChunkTree(_cx & ChunkManager.REGION_XMASK, _cz & ChunkManager.REGION_ZMASK, _nbt);
            }

            return false;
        }

        public int GetBlockID (int x, int y, int z)
        {
            if (_blocks == null) {
                _blocks = GetTree().getRoot().findTagByName("Level").findTagByName("Blocks").value.toByteArray();
            }

            return _blocks.data[x << 11 | z << 7 | y];
        }

        public bool SetBlockID (int x, int y, int z, int id)
        {
            if (_blocks == null) {
                _blocks = GetTree().getRoot().findTagByName("Level").findTagByName("Blocks").value.toByteArray();
            }

            int index = x << 11 | z << 7 | y;
            if (_blocks.data[index] == id) {
                return false;
            }

            _blocks.data[index] = (byte)id;
            MarkDirty();

            return true;
        }

        public int CountBlockID (int id)
        {
            if (_blocks == null) {
                _blocks = GetTree().getRoot().findTagByName("Level").findTagByName("Blocks").value.toByteArray();
            }

            int c = 0;
            for (int i = 0; i < _blocks.length; i++) {
                if (_blocks.data[i] == id) {
                    c++;
                }
            }

            return c;
        }

        public int GetBlockData (int x, int y, int z)
        {
            if (_data == null) {
                _data = new NibbleArray(GetTree().getRoot().findTagByName("Level").findTagByName("Data").value.toByteArray().data);
            }

            return _data[x << 11 | z << 7 | y];
        }

        public bool SetBlockData (int x, int y, int z, int data)
        {
            if (_data == null) {
                _data = new NibbleArray(GetTree().getRoot().findTagByName("Level").findTagByName("Data").value.toByteArray().data);
            }

            int index = x << 11 | z << 7 | y;
            if (_data[index] == data) {
                return false;
            }

            _data[index] = data;
            MarkDirty();

            return true;
        }

        public bool IsPopulated ()
        {
            return GetTree().getRoot().findTagByName("Level").findTagByName("TerrainPopulated").value.toByte().data == 1;
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
    }
}
