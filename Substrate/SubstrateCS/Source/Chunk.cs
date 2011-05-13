using System;
using System.IO;
using System.Collections.Generic;

namespace Substrate
{
    using NBT;
    using Utility;

    public class Chunk : IChunk, INBTObject<Chunk>, ICopyable<Chunk>
    {
        private const int XDIM = 16;
        private const int YDIM = 128;
        private const int ZDIM = 16;

        public static NBTCompoundNode LevelSchema = new NBTCompoundNode()
        {
            new NBTCompoundNode("Level")
            {
                new NBTArrayNode("Blocks", 32768),
                new NBTArrayNode("Data", 16384),
                new NBTArrayNode("SkyLight", 16384),
                new NBTArrayNode("BlockLight", 16384),
                new NBTArrayNode("HeightMap", 256),
                new NBTListNode("Entities", TagType.TAG_COMPOUND, 0, NBTOptions.CREATE_ON_MISSING),
                new NBTListNode("TileEntities", TagType.TAG_COMPOUND, TileEntity.BaseSchema, NBTOptions.CREATE_ON_MISSING),
                new NBTScalerNode("LastUpdate", TagType.TAG_LONG, NBTOptions.CREATE_ON_MISSING),
                new NBTScalerNode("xPos", TagType.TAG_INT),
                new NBTScalerNode("zPos", TagType.TAG_INT),
                new NBTScalerNode("TerrainPopulated", TagType.TAG_BYTE, NBTOptions.CREATE_ON_MISSING),
            },
        };

        private NBT_Tree _tree;

        private int _cx;
        private int _cz;

        private XZYByteArray _blocks;
        private XZYNibbleArray _data;
        private XZYNibbleArray _blockLight;
        private XZYNibbleArray _skyLight;
        private ZXByteArray _heightMap;

        private TagList _entities;
        private TagList _tileEntities;

        private AlphaBlockCollection _blockManager;
        private EntityCollection _entityManager;

        public int X
        {
            get { return _cx; }
        }

        public int Z
        {
            get { return _cz; }
        }

        public AlphaBlockCollection Blocks
        {
            get { return _blockManager; }
        }

        public EntityCollection Entities
        {
            get { return _entityManager; }
        }

        public NBT_Tree Tree
        {
            get { return _tree; }
        }

        public bool IsTerrainPopulated
        {
            get { return _tree.Root["Level"].ToTagCompound()["TerrainPopulated"].ToTagByte() == 1; }
            set { _tree.Root["Level"].ToTagCompound()["TerrainPopulated"].ToTagByte().Data = (byte)(value ? 1 : 0); }
        }

        private Chunk ()
        {
        }

        public static Chunk Create (int x, int z)
        {
            Chunk c = new Chunk();

            c._cx = x;
            c._cz = z;

            c.BuildNBTTree();

            return c;
        }

        public static Chunk Create (NBT_Tree tree)
        {
            Chunk c = new Chunk();

            return c.LoadTree(tree.Root);
        }

        public static Chunk CreateVerified (NBT_Tree tree)
        {
            Chunk c = new Chunk();

            return c.LoadTreeSafe(tree.Root);
        }

        public virtual void SetLocation (int x, int z)
        {
            _cx = x;
            _cz = z;
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


        #region INBTObject<Chunk> Members

        public Chunk LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null) {
                return null;
            }

            _tree = new NBT_Tree(ctree);

            TagCompound level = _tree.Root["Level"] as TagCompound;

            _blocks = new XZYByteArray(XDIM, YDIM, ZDIM, level["Blocks"] as TagByteArray);
            _data = new XZYNibbleArray(XDIM, YDIM, ZDIM, level["Data"] as TagByteArray);
            _blockLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, level["BlockLight"] as TagByteArray);
            _skyLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, level["SkyLight"] as TagByteArray);
            _heightMap = new ZXByteArray(XDIM, ZDIM, level["HeightMap"] as TagByteArray);

            _entities = level["Entities"] as TagList;
            _tileEntities = level["TileEntities"] as TagList;

            // List-type patch up
            if (_entities.Count == 0) {
                level["Entities"] = new TagList(TagType.TAG_COMPOUND);
                _entities = level["Entities"] as TagList;
            }

            if (_tileEntities.Count == 0) {
                level["TileEntities"] = new TagList(TagType.TAG_COMPOUND);
                _tileEntities = level["TileEntities"] as TagList;
            }

            _cx = level["xPos"].ToTagInt();
            _cz = level["zPos"].ToTagInt();

            _blockManager = new AlphaBlockCollection(
                new BlockData(_blocks),
                new BlockDataData(_data),
                new BlockLightData(_blockLight, _skyLight, _heightMap),
                new BlockPropertyData(_tileEntities)
                );

            _entityManager = new EntityCollection(_entities);

            return this;
        }

        public Chunk LoadTreeSafe (TagValue tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public TagValue BuildTree ()
        {
            return _tree.Root;
        }

        public bool ValidateTree (TagValue tree)
        {
            NBTVerifier v = new NBTVerifier(tree, LevelSchema);
            return v.Verify();
        }

        #endregion


        #region ICopyable<Chunk> Members

        public Chunk Copy ()
        {
            return Chunk.Create(_tree.Copy());
        }

        #endregion


        private void BuildNBTTree ()
        {
            int elements2 = XDIM * ZDIM;
            int elements3 = elements2 * YDIM;

            TagByteArray blocks = new TagByteArray(new byte[elements3]);
            TagByteArray data = new TagByteArray(new byte[elements3 >> 1]);
            TagByteArray blocklight = new TagByteArray(new byte[elements3 >> 1]);
            TagByteArray skylight = new TagByteArray(new byte[elements3 >> 1]);
            TagByteArray heightMap = new TagByteArray(new byte[elements2]);

            _blocks = new XZYByteArray(XDIM, YDIM, ZDIM, blocks);
            _data = new XZYNibbleArray(XDIM, YDIM, ZDIM, data);
            _blockLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, blocklight);
            _skyLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, skylight);
            _heightMap = new ZXByteArray(XDIM, ZDIM, heightMap);

            _entities = new TagList(TagType.TAG_COMPOUND);
            _tileEntities = new TagList(TagType.TAG_COMPOUND);

            TagCompound level = new TagCompound();
            level.Add("Blocks", blocks);
            level.Add("Data", data);
            level.Add("SkyLight", blocklight);
            level.Add("BlockLight", skylight);
            level.Add("HeightMap", heightMap);
            level.Add("Entities", _entities);
            level.Add("TileEntities", _tileEntities);
            level.Add("LastUpdate", new TagLong(Timestamp()));
            level.Add("xPos", new TagInt(_cx));
            level.Add("zPos", new TagInt(_cz));
            level.Add("TerrainPopulated", new TagByte());

            _tree = new NBT_Tree();
            _tree.Root.Add("Level", level);

            _blockManager = new AlphaBlockCollection(
                new BlockData(_blocks),
                new BlockDataData(_data),
                new BlockLightData(_blockLight, _skyLight, _heightMap),
                new BlockPropertyData(_tileEntities)
                );

            _entityManager = new EntityCollection(_entities);
        }

        private int Timestamp ()
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (int)((DateTime.UtcNow - epoch).Ticks / (10000L * 1000L));
        }
    }
}
