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

            return true;
        }
    }
}
