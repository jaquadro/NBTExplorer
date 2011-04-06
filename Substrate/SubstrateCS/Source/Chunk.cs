using System;
using System.IO;

namespace Substrate.Map
{
    using NBT;
    using Utility;
    using System.Collections.Generic;

    public class Chunk : IChunk, INBTObject<Chunk>, ICopyable<Chunk>
    {
        public static NBTCompoundNode LevelSchema = new NBTCompoundNode()
        {
            new NBTCompoundNode("Level")
            {
                new NBTArrayNode("Blocks", 32768),
                new NBTArrayNode("Data", 16384),
                new NBTArrayNode("SkyLight", 16384),
                new NBTArrayNode("BlockLight", 16384),
                new NBTArrayNode("HeightMap", 256),
                new NBTListNode("Entities", NBT_Type.TAG_COMPOUND),
                new NBTListNode("TileEntities", NBT_Type.TAG_COMPOUND, TileEntity.BaseSchema),
                new NBTScalerNode("LastUpdate", NBT_Type.TAG_LONG),
                new NBTScalerNode("xPos", NBT_Type.TAG_INT),
                new NBTScalerNode("zPos", NBT_Type.TAG_INT),
                new NBTScalerNode("TerrainPopulated", NBT_Type.TAG_BYTE),
            },
        };

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

        protected Dictionary<BlockKey, NBT_Compound> _tileEntityTable;

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

            BuildTileEntityCache();
        }

        public Chunk (NBT_Tree tree)
        {
            if (LoadTreeSafe(tree.Root) == null) {
                throw new InvalidNBTObjectException();
            }
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
            int oldid = _blocks.Data[index];

            if (oldid == id) {
                return false;
            }

            // Update value

            _blocks.Data[index] = (byte)id;

            // Update tile entities

            BlockInfoEx info1 = BlockInfo.BlockTable[oldid] as BlockInfoEx;
            BlockInfoEx info2 = BlockInfo.BlockTable[id] as BlockInfoEx;

            if (info1 != info2) {
                if (info1 != null) {
                    ClearTileEntity(lx, ly, lz);
                }

                if (info2 != null) {
                    CreateTileEntity(lx, ly, lz);
                }
            }

            /*if (BlockInfo.SchemaTable[_blocks[index]] != BlockInfo.SchemaTable[id]) {
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
            }*/

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

        private void CreateTileEntity (int lx, int ly, int lz)
        {
            BlockInfoEx info = GetBlockInfo(lx, ly, lz) as BlockInfoEx;
            if (info == null) {
                return;
            }

            TileEntity te = TileEntityFactory.Create(info.TileEntityName);
            if (te == null) {
                return;
            }

            te.X = BlockGlobalX(lx);
            te.Y = BlockGlobalY(ly);
            te.Z = BlockGlobalZ(lz);

            _tileEntities.Add(te.BuildTree());
        }

        public TileEntity GetTileEntity (int lx, int ly, int lz)
        {
            int x = BlockGlobalX(lx);
            int y = BlockGlobalY(ly);
            int z = BlockGlobalZ(lz);

            BlockKey key = new BlockKey(x, y, z);
            NBT_Compound te;

            if (!_tileEntityTable.TryGetValue(key, out te)) {
                return null;
            }

            return TileEntityFactory.Create(te);
        }

        public bool SetTileEntity (int lx, int ly, int lz, TileEntity te)
        {
            BlockInfoEx info = GetBlockInfo(lx, ly, lz) as BlockInfoEx;
            if (info == null) {
                return false;
            }

            if (te.GetType() != TileEntityFactory.Lookup(info.TileEntityName)) {
                return false;
            }

            int x = BlockGlobalX(lx);
            int y = BlockGlobalY(ly);
            int z = BlockGlobalZ(lz);

            BlockKey key = new BlockKey(x, y, z);
            NBT_Compound oldte;

            if (_tileEntityTable.TryGetValue(key, out oldte)) {
                _tileEntities.Remove(oldte);
            }

            te.X = x;
            te.Y = y;
            te.Z = z;

            NBT_Compound tree = te.BuildTree() as NBT_Compound;

            _tileEntities.Add(tree);
            _tileEntityTable[key] = tree;            

            return true;
        }

        public bool ClearTileEntity (int lx, int ly, int lz)
        {
            int x = BlockGlobalX(lx);
            int y = BlockGlobalY(ly);
            int z = BlockGlobalZ(lz);

            BlockKey key = new BlockKey(x, y, z);
            NBT_Compound te;

            if (!_tileEntityTable.TryGetValue(key, out te)) {
                return false;
            }

            _tileEntities.Remove(te);
            _tileEntityTable.Remove(key);

            return true;
        }

        public virtual void SetLocation (int x, int z)
        {
            int diffx = x - _cx;
            int diffz = z - _cz;

            _cx = x;
            _cz = z;

            BuildTileEntityCache();
        }

        private void BuildTileEntityCache ()
        {
            _tileEntityTable = new Dictionary<BlockKey, NBT_Compound>();

            foreach (NBT_Compound te in _tileEntities) {
                int tex = te["x"].ToNBTInt();
                int tey = te["y"].ToNBTInt();
                int tez = te["z"].ToNBTInt();

                BlockKey key = new BlockKey(tex, tey, tez);
                _tileEntityTable[key] = te;
            }
        }

        #region ICopyable<Chunk> Members

        public Chunk Copy ()
        {
            return new Chunk(_tree.Copy());
        }

        #endregion

        #region INBTObject<Chunk> Members

        public Chunk LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null) {
                return null;
            }

            _tree = new NBT_Tree(ctree);
 
            NBT_Compound level = _tree.Root["Level"] as NBT_Compound;

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

            BuildTileEntityCache();

            return this;
        }

        public Chunk LoadTreeSafe (NBT_Value tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public NBT_Value BuildTree ()
        {
            return _tree.Root;
        }

        public bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, LevelSchema).Verify();
        }

        #endregion
    }
}
