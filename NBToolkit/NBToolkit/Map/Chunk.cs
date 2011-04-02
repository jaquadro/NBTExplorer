using System;
using System.IO;

namespace NBToolkit.Map
{
    using NBT;

    public interface IChunk
    {
        int X { get; }
        int Z { get; }

        bool IsTerrainPopulated { get; set; }

        bool Save (Stream outStream);

        Block GetBlock (int lx, int ly, int lz);
        BlockRef GetBlockRef (int lx, int ly, int lz);
        BlockInfo GetBlockInfo (int lx, int ly, int lz);

        int GetBlockID (int lx, int ly, int lz);
        int GetBlockData (int lx, int ly, int lz);
        int GetBlockLight (int lx, int ly, int lz);
        int GetBlockSkyLight (int lx, int ly, int lz);

        bool SetBlockID (int lx, int ly, int lz, int id);
        bool SetBlockData (int lx, int ly, int lz, int data);
        bool SetBlockLight (int lx, int ly, int lz, int light);
        bool SetBlockSkyLight (int lx, int ly, int lz, int light);

        int CountBlockID (int id);
        int CountBlockData (int id, int data);

        int GetHeight (int lx, int lz);

        TileEntity GetTileEntity (int lx, int ly, int lz);
        bool SetTileEntity (int lx, int ly, int lz, TileEntity te);
        bool ClearTileEntity (int lx, int ly, int lz);
    }

    public class Chunk : IChunk
    {
        private NBT_Tree _tree;

        private int _cx;
        private int _cz;

        protected NBT_ByteArray _blocks;
        protected NibbleArray _data;
        protected NibbleArray _blockLight;
        protected NibbleArray _skyLight;
        protected NBT_ByteArray _heightMap;

        protected NBT_List _entities;
        protected NBT_List _tileEntities;

        public int X
        {
            get { return _cx; }
        }

        public int Z
        {
            get { return _cz; }
        }

        public bool IsTerrainPopulated
        {
            get { return _tree.Root["Level"].ToNBTCompound()["TerrainPopulated"].ToNBTByte() == 1; }
            set { _tree.Root["Level"].ToNBTCompound()["TerrainPopulated"].ToNBTByte().Data = (byte)(value ? 1 : 0); }
        }

        public Chunk (int x, int z)
        {
            _cx = x;
            _cz = z;

            BuildNBTTree();
        }

        public Chunk (NBT_Tree tree)
        {
            _tree = tree;
            if (new ChunkVerifier(tree).Verify() == false) {
                throw new MalformedNBTTreeException();
            }

            NBT_Compound level = tree.Root["Level"] as NBT_Compound;

            _blocks = level["Blocks"] as NBT_ByteArray;
            _data = new NibbleArray(level["Data"].ToNBTByteArray().Data);
            _blockLight = new NibbleArray(level["BlockLight"].ToNBTByteArray().Data);
            _skyLight = new NibbleArray(level["SkyLight"].ToNBTByteArray().Data);
            _heightMap = level["HeightMap"] as NBT_ByteArray;

            _entities = level["Entities"] as NBT_List;
            _tileEntities = level["TileEntities"] as NBT_List;

            _cx = level["xPos"].ToNBTInt();
            _cz = level["zPos"].ToNBTInt();
        }

        private void BuildNBTTree ()
        {
            int elements2 = BlockManager.CHUNK_XLEN * BlockManager.CHUNK_ZLEN;
            int elements3 = elements2 * BlockManager.CHUNK_YLEN;

            _blocks = new NBT_ByteArray(new byte[elements3]);
            NBT_ByteArray data = new NBT_ByteArray(new byte[elements3 >> 1]);
            NBT_ByteArray blocklight = new NBT_ByteArray(new byte[elements3 >> 1]);
            NBT_ByteArray skylight = new NBT_ByteArray(new byte[elements3 >> 1]);
            _heightMap = new NBT_ByteArray(new byte[elements2]);
            _entities = new NBT_List(NBT_Type.TAG_COMPOUND);
            _tileEntities = new NBT_List(NBT_Type.TAG_COMPOUND);

            _data = new NibbleArray(data.Data);
            _blockLight = new NibbleArray(blocklight.Data);
            _skyLight = new NibbleArray(skylight.Data);

            NBT_Compound level = new NBT_Compound();
            level.Add("Blocks", _blocks);
            level.Add("Data", data);
            level.Add("SkyLight", blocklight);
            level.Add("BlockLight", skylight);
            level.Add("HeightMap", _heightMap);
            level.Add("Entities", _entities);
            level.Add("TileEntities", _tileEntities);
            level.Add("LastUpdate", new NBT_Long());
            level.Add("xPos", new NBT_Int());
            level.Add("zPos", new NBT_Int());
            level.Add("TerrainPopulated", new NBT_Byte());

            _tree = new NBT_Tree();
            _tree.Root.Add("Level", level);
        }

        public bool Save (Stream outStream)
        {
            if (outStream == null || !outStream.CanWrite) {
                return false;
            }

            _tree.WriteTo(outStream);
            outStream.Close();

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
            return BlockInfo.BlockTable[GetBlockID(lx, ly, lz)];
        }

        public int GetBlockID (int lx, int ly, int lz)
        {
            return _blocks.Data[lx << 11 | lz << 7 | ly];
        }

        public int GetBlockData (int lx, int ly, int lz)
        {
            return _data[lx << 11 | lz << 7 | ly];
        }

        public int GetBlockLight (int lx, int ly, int lz)
        {
            return _blockLight[lx << 11 | lz << 7 | ly];
        }

        public int GetBlockSkyLight (int lx, int ly, int lz)
        {
            return _skyLight[lx << 11 | lz << 7 | ly];
        }

        public bool SetBlockID (int lx, int ly, int lz, int id)
        {
            int index = lx << 11 | lz << 7 | ly;
            if (_blocks.Data[index] == id) {
                return false;
            }

            // Update tile entities

            if (BlockInfo.SchemaTable[_blocks[index]] != null &&
                BlockInfo.SchemaTable[_blocks[index]] != BlockInfo.SchemaTable[id]) {
                ClearTileEntity(lx, ly, lz);
            }

            // Update height map

            int tileHeight = GetHeight(lx, lz);
            int newOpacity = BlockInfo.BlockTable[id].Opacity;

            if (ly > tileHeight && newOpacity > BlockInfo.MIN_OPACITY) {
                _heightMap[lz << 4 | lx] = (byte)ly;
            }
            else if (ly == tileHeight && newOpacity == BlockInfo.MIN_OPACITY) {
                for (int i = ly - 1; i >= 0; i--) {
                    if (BlockInfo.BlockTable[GetBlockID(lx, i, lz)].Opacity > BlockInfo.MIN_OPACITY) {
                        _heightMap[lz << 4 | lx] = (byte)i;
                        break;
                    }
                }
            }

            // Update value

            _blocks.Data[index] = (byte)id;
            return true;
        }

        public bool SetBlockData (int lx, int ly, int lz, int data)
        {
            int index = lx << 11 | lz << 7 | ly;
            if (_data[index] == data) {
                return false;
            }

            _data[index] = data;
            return true;
        }

        public bool SetBlockLight (int lx, int ly, int lz, int light)
        {
            int index = lx << 11 | lz << 7 | ly;
            if (_blockLight[index] == light) {
                return false;
            }

            _blockLight[index] = light;
            return true;
        }

        public bool SetBlockSkyLight (int lx, int ly, int lz, int light)
        {
            int index = lx << 11 | lz << 7 | ly;
            if (_skyLight[index] == light) {
                return false;
            }

            _skyLight[index] = light;
            return true;
        }

        public int CountBlockID (int id)
        {
            int c = 0;
            for (int i = 0; i < _blocks.Length; i++) {
                if (_blocks[i] == id) {
                    c++;
                }
            }

            return c;
        }

        public int CountBlockData (int id, int data)
        {
            int c = 0;
            for (int i = 0; i < _blocks.Length; i++) {
                if (_blocks[i] == id && _data[i] == data) {
                    c++;
                }
            }

            return c;
        }

        public int GetHeight (int lx, int lz)
        {
            return _heightMap[lz << 4 | lx];
        }

        public TileEntity GetTileEntity (int lx, int ly, int lz)
        {
            foreach (NBT_Compound te in _tileEntities) {
                if (te["x"].ToNBTInt().Data == lx &&
                    te["y"].ToNBTInt().Data == ly &&
                    te["z"].ToNBTInt().Data == lz) {
                    return new TileEntity(te);
                }
            }

            return null;
        }

        public bool SetTileEntity (int lx, int ly, int lz, TileEntity te)
        {
            int id = GetBlockID(lx, ly, lz);

            NBTCompoundNode schema = BlockInfo.SchemaTable[id];
            if (schema == null) {
                return false;
            }

            if (!te.Verify(schema)) {
                return false;
            }

            ClearTileEntity(lx, ly, lz);

            int x = BlockX(lx);
            int y = BlockY(ly);
            int z = BlockZ(lz);

            if (!te.LocatedAt(x, y, z)) {
                te = te.Copy();
                te.Relocate(x, y, z);
            }

            _tileEntities.Add(te.Root);

            return true;
        }

        public bool ClearTileEntity (int x, int y, int z)
        {
            TileEntity te = GetTileEntity(x, y, z);
            if (te == null) {
                return false;
            }

            return _tileEntities.Remove(te.Root);
        }

        public virtual void SetLocation (int x, int z)
        {
            int diffx = x - _cx;
            int diffz = z - _cz;

            _cx = x;
            _cz = z;

            foreach (NBT_Compound te in _tileEntities) {
                te["x"].ToNBTInt().Data += diffx * BlockManager.CHUNK_XLEN;
                te["z"].ToNBTInt().Data += diffz * BlockManager.CHUNK_ZLEN;
            }
        }

        protected int BlockX (int lx)
        {
            return _cx * BlockManager.CHUNK_XLEN + lx;
        }

        protected int BlockY (int ly)
        {
            return ly;
        }

        protected int BlockZ (int lz)
        {
            return _cz * BlockManager.CHUNK_ZLEN + lz;
        }
    }
}
