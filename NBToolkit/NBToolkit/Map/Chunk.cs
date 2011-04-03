using System;
using System.IO;

namespace NBToolkit.Map
{
    using NBT;
    using Utility;

    public class Chunk : IChunk, ICopyable<Chunk>
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

        public NBT_Tree Tree
        {
            get { return _tree; }
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

            // List-type patch up
            if (_entities.Count == 0) {
                level["Entities"] = new NBT_List(NBT_Type.TAG_COMPOUND);
                _entities = level["Entities"] as NBT_List;
            }

            if (_tileEntities.Count == 0) {
                level["TileEntities"] = new NBT_List(NBT_Type.TAG_COMPOUND);
                _tileEntities = level["TileEntities"] as NBT_List;
            }

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

        public int BlockGlobalX (int x)
        {
            return _cx * BlockManager.CHUNK_XLEN + x;
        }

        public int BlockGlobalY (int y)
        {
            return y;
        }

        public int BlockGlobalZ (int z)
        {
            return _cz * BlockManager.CHUNK_ZLEN + z;
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

        public void SetBlock (int lx, int ly, int lz, Block block)
        {
            int index = lx << 11 | lz << 7 | ly;

            SetBlockID(lx, ly, lz, block.ID);
            SetBlockData(lx, ly, lz, block.Data);

            _blockLight[index] = block.BlockLight;
            _skyLight[index] = block.SkyLight;

            SetTileEntity(lx, ly, lz, block.GetTileEntity().Copy());
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

            if (BlockInfo.SchemaTable[_blocks[index]] != BlockInfo.SchemaTable[id]) {
                if (BlockInfo.SchemaTable[_blocks[index]] != null) {
                    ClearTileEntity(lx, ly, lz);
                }

                if (BlockInfo.SchemaTable[id] != null) {
                    TileEntity te = new TileEntity(BlockInfo.SchemaTable[id]);
                    te.X = BlockGlobalX(lx);
                    te.Y = BlockGlobalY(ly);
                    te.Z = BlockGlobalZ(lz);
                    _tileEntities.Add(te.Root);
                }
            }

            // Update height map

            if (BlockInfo.BlockTable[id] != null) {
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

            if (BlockManager.EnforceDataLimits && BlockInfo.BlockTable[_blocks[index]] != null) {
                if (!BlockInfo.BlockTable[_blocks[index]].TestData(data)) {
                    return false;
                }
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
            int x = BlockGlobalX(lx);
            int y = BlockGlobalY(ly);
            int z = BlockGlobalZ(lz);

            foreach (NBT_Compound te in _tileEntities) {
                if (te["x"].ToNBTInt().Data == x &&
                    te["y"].ToNBTInt().Data == y &&
                    te["z"].ToNBTInt().Data == z) {
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

            te.X = BlockGlobalX(lx);
            te.Y = BlockGlobalY(ly);
            te.Z = BlockGlobalZ(lz);

            _tileEntities.Add(te.Root);

            return true;
        }

        public bool ClearTileEntity (int lx, int ly, int lz)
        {
            TileEntity te = GetTileEntity(lx, ly, lz);
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

        #region ICopyable<Chunk> Members

        public Chunk Copy ()
        {
            return new Chunk(_tree.Copy());
        }

        #endregion
    }
}
