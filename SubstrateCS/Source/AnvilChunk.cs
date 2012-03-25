using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Nbt;
using Substrate.Core;
using System.IO;

namespace Substrate
{
    public class AnvilSection : INbtObject<AnvilSection>, ICopyable<AnvilSection>
    {
        public static SchemaNodeCompound SectionSchema = new SchemaNodeCompound()
        {
            new SchemaNodeArray("Blocks", 4096),
            new SchemaNodeArray("Data", 2048),
            new SchemaNodeArray("SkyLight", 2048),
            new SchemaNodeArray("BlockLight", 2048),
            new SchemaNodeScaler("Y", TagType.TAG_BYTE),
            new SchemaNodeArray("AddBlocks", 2048, SchemaOptions.OPTIONAL),
        };

        private const int XDIM = 16;
        private const int YDIM = 16;
        private const int ZDIM = 16;

        private const int MIN_Y = 0;
        private const int MAX_Y = 15;

        private TagNodeCompound _tree;

        private byte _y;
        private YZXByteArray _blocks;
        private YZXNibbleArray _data;
        private YZXNibbleArray _blockLight;
        private YZXNibbleArray _skyLight;
        private YZXNibbleArray _addBlocks;

        private AnvilSection ()
        {
        }

        public AnvilSection (int y)
        {
            if (y < MIN_Y || y > MAX_Y)
                throw new ArgumentOutOfRangeException();

            _y = (byte)y;
            BuildNbtTree();
        }

        public AnvilSection (TagNodeCompound tree)
        {
            LoadTree(tree);
        }

        public int Y
        {
            get { return _y; }
            set
            {
                if (value < MIN_Y || value > MAX_Y)
                    throw new ArgumentOutOfRangeException();

                _y = (byte)value;
                _tree["Y"].ToTagByte().Data = _y;
            }
        }

        public YZXByteArray Blocks
        {
            get { return _blocks; }
        }

        public YZXNibbleArray Data
        {
            get { return _data; }
        }

        public YZXNibbleArray BlockLight
        {
            get { return _blockLight; }
        }

        public YZXNibbleArray SkyLight
        {
            get { return _skyLight; }
        }

        public YZXNibbleArray AddBlocks
        {
            get { return _addBlocks; }
        }

        public bool CheckEmpty ()
        {
            return CheckBlocksEmpty() && CheckAddBlocksEmpty();
        }

        private bool CheckBlocksEmpty ()
        {
            for (int i = 0; i < _blocks.Length; i++)
                if (_blocks[i] != 0)
                    return false;
            return true;
        }

        private bool CheckAddBlocksEmpty ()
        {
            if (_addBlocks != null)
                for (int i = 0; i < _addBlocks.Length; i++)
                    if (_addBlocks[i] != 0)
                        return false;
            return true;
        }

        #region INbtObject<AnvilSection> Members

        public AnvilSection LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null) {
                return null;
            }

            _y = ctree["Y"] as TagNodeByte;

            _blocks = new YZXByteArray(XDIM, YDIM, ZDIM, ctree["Blocks"] as TagNodeByteArray);
            _data = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["Data"] as TagNodeByteArray);
            _skyLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["SkyLight"] as TagNodeByteArray);
            _blockLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["BlockLight"] as TagNodeByteArray);

            if (!ctree.ContainsKey("AddBlocks"))
                ctree["AddBlocks"] = new TagNodeByteArray(new byte[2048]);
            _addBlocks = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["AddBlocks"] as TagNodeByteArray);

            _tree = ctree;

            return this;
        }

        public AnvilSection LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public TagNode BuildTree ()
        {
            TagNodeCompound copy = new TagNodeCompound();
            foreach (KeyValuePair<string, TagNode> node in _tree) {
                copy.Add(node.Key, node.Value);
            }

            if (CheckAddBlocksEmpty())
                copy.Remove("AddBlocks");

            return copy;
        }

        public bool ValidateTree (TagNode tree)
        {
            NbtVerifier v = new NbtVerifier(tree, SectionSchema);
            return v.Verify();
        }

        #endregion

        #region ICopyable<AnvilSection> Members

        public AnvilSection Copy ()
        {
            return new AnvilSection().LoadTree(_tree.Copy());
        }

        #endregion

        private void BuildNbtTree ()
        {
            int elements3 = XDIM * YDIM * ZDIM;

            TagNodeByteArray blocks = new TagNodeByteArray(new byte[elements3]);
            TagNodeByteArray data = new TagNodeByteArray(new byte[elements3 >> 1]);
            TagNodeByteArray skyLight = new TagNodeByteArray(new byte[elements3 >> 1]);
            TagNodeByteArray blockLight = new TagNodeByteArray(new byte[elements3 >> 1]);
            TagNodeByteArray addBlocks = new TagNodeByteArray(new byte[elements3 >> 1]);

            _blocks = new YZXByteArray(XDIM, YDIM, ZDIM, blocks);
            _data = new YZXNibbleArray(XDIM, YDIM, ZDIM, data);
            _skyLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, skyLight);
            _blockLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, blockLight);
            _addBlocks = new YZXNibbleArray(XDIM, YDIM, ZDIM, addBlocks);

            TagNodeCompound tree = new TagNodeCompound();
            tree.Add("Y", new TagNodeByte(_y));
            tree.Add("Blocks", blocks);
            tree.Add("Data", data);
            tree.Add("SkyLight", skyLight);
            tree.Add("BlockLight", blockLight);
            tree.Add("AddBlocks", addBlocks);

            _tree = tree;
        }
    }

    

    public class CompositeYZXByteArray : IDataArray3
    {
        private YZXByteArray[] _sections;

        public CompositeYZXByteArray (YZXByteArray[] sections)
        {
            for (int i = 0; i < sections.Length; i++)
                if (sections[i] == null)
                    throw new ArgumentException("sections argument cannot have null entries.");

            for (int i = 0; i < sections.Length; i++) {
                if (sections[i].Length != sections[0].Length
                    || sections[i].XDim != sections[0].XDim
                    || sections[i].YDim != sections[0].YDim
                    || sections[i].ZDim != sections[0].ZDim)
                    throw new ArgumentException("All elements in sections argument must have same metrics.");
            }

            _sections = sections;
        }

        #region IByteArray3 Members

        public int this[int x, int y, int z]
        {
            get
            {
                int ydiv = y / _sections[0].YDim;
                int yrem = y - (ydiv * _sections[0].YDim);
                return _sections[ydiv][x, yrem, z];
            }

            set
            {
                int ydiv = y / _sections[0].YDim;
                int yrem = y - (ydiv * _sections[0].YDim);
                _sections[ydiv][x, yrem, z] = value;
            }
        }

        public int XDim
        {
            get { return _sections[0].XDim; }
        }

        public int YDim
        {
            get { return _sections[0].YDim * _sections.Length; }
        }

        public int ZDim
        {
            get { return _sections[0].ZDim; }
        }

        public int GetIndex (int x, int y, int z)
        {
            int ydiv = y / _sections[0].YDim;
            int yrem = y - (ydiv * _sections[0].YDim);
            return ydiv * _sections[ydiv].GetIndex(x, yrem, z);
        }

        public void GetMultiIndex (int index, out int x, out int y, out int z)
        {
            int idiv = index / _sections[0].Length;
            int irem = index - (idiv * _sections[0].Length);
            _sections[idiv].GetMultiIndex(irem, out x, out y, out z);
        }

        #endregion

        #region IByteArray Members

        public int this[int i]
        {
            get
            {
                int idiv = i / _sections[0].Length;
                int irem = i - (idiv * _sections[0].Length);
                return _sections[idiv][irem];
            }
            set
            {
                int idiv = i / _sections[0].Length;
                int irem = i - (idiv * _sections[0].Length);
                _sections[idiv][irem] = value;
            }
        }

        public int Length
        {
            get { return _sections[0].Length * _sections.Length; }
        }

        public void Clear ()
        {
            for (int i = 0; i < _sections.Length; i++)
                _sections[i].Clear();
        }

        #endregion
    }

    public class CompositeYZXNibbleArray : IDataArray3
    {
        private YZXNibbleArray[] _sections;

        public CompositeYZXNibbleArray (YZXNibbleArray[] sections)
        {
            for (int i = 0; i < sections.Length; i++)
                if (sections[i] == null)
                    throw new ArgumentException("sections argument cannot have null entries.");

            for (int i = 0; i < sections.Length; i++) {
                if (sections[i].Length != sections[0].Length
                    || sections[i].XDim != sections[0].XDim
                    || sections[i].YDim != sections[0].YDim
                    || sections[i].ZDim != sections[0].ZDim)
                    throw new ArgumentException("All elements in sections argument must have same metrics.");
            }

            _sections = sections;
        }

        #region IByteArray3 Members

        public int this[int x, int y, int z]
        {
            get
            {
                int ydiv = y / _sections[0].YDim;
                int yrem = y - (ydiv * _sections[0].YDim);
                return _sections[ydiv][x, yrem, z];
            }

            set
            {
                int ydiv = y / _sections[0].YDim;
                int yrem = y - (ydiv * _sections[0].YDim);
                _sections[ydiv][x, yrem, z] = (byte)value;
            }
        }

        public int XDim
        {
            get { return _sections[0].XDim; }
        }

        public int YDim
        {
            get { return _sections[0].YDim * _sections.Length; }
        }

        public int ZDim
        {
            get { return _sections[0].ZDim; }
        }

        public int GetIndex (int x, int y, int z)
        {
            int ydiv = y / _sections[0].YDim;
            int yrem = y - (ydiv * _sections[0].YDim);
            return ydiv * _sections[ydiv].GetIndex(x, yrem, z);
        }

        public void GetMultiIndex (int index, out int x, out int y, out int z)
        {
            int idiv = index / _sections[0].Length;
            int irem = index - (idiv * _sections[0].Length);
            _sections[idiv].GetMultiIndex(irem, out x, out y, out z);
        }

        #endregion

        #region IByteArray Members

        public int this[int i]
        {
            get
            {
                int idiv = i / _sections[0].Length;
                int irem = i - (idiv * _sections[0].Length);
                return _sections[idiv][irem];
            }
            set
            {
                int idiv = i / _sections[0].Length;
                int irem = i - (idiv * _sections[0].Length);
                _sections[idiv][irem] = (byte)value;
            }
        }

        public int Length
        {
            get { return _sections[0].Length * _sections.Length; }
        }

        public void Clear ()
        {
            for (int i = 0; i < _sections.Length; i++)
                _sections[i].Clear();
        }

        #endregion
    }

    public class Chunk : IChunk, INbtObject<Chunk>, ICopyable<Chunk>
    {
        public static SchemaNodeCompound LevelSchema = new SchemaNodeCompound()
        {
            new SchemaNodeCompound("Level")
            {
                new SchemaNodeList("Sections", TagType.TAG_COMPOUND, new SchemaNodeCompound() {
                    new SchemaNodeArray("Blocks", 4096),
                    new SchemaNodeArray("Data", 2048),
                    new SchemaNodeArray("SkyLight", 2048),
                    new SchemaNodeArray("BlockLight", 2048),
                    new SchemaNodeScaler("Y", TagType.TAG_BYTE),
                    new SchemaNodeArray("AddBlocks", 2048, SchemaOptions.OPTIONAL),
                }),
                new SchemaNodeArray("Biomes", 256, SchemaOptions.OPTIONAL),
                new SchemaNodeIntArray("HeightMap", 256),
                new SchemaNodeList("Entities", TagType.TAG_COMPOUND, SchemaOptions.CREATE_ON_MISSING),
                new SchemaNodeList("TileEntities", TagType.TAG_COMPOUND, TileEntity.Schema, SchemaOptions.CREATE_ON_MISSING),
                new SchemaNodeList("TileTicks", TagType.TAG_COMPOUND, TileTick.Schema, SchemaOptions.OPTIONAL),
                new SchemaNodeScaler("LastUpdate", TagType.TAG_LONG, SchemaOptions.CREATE_ON_MISSING),
                new SchemaNodeScaler("xPos", TagType.TAG_INT),
                new SchemaNodeScaler("zPos", TagType.TAG_INT),
                new SchemaNodeScaler("TerrainPopulated", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
            },
        };

        private const int XDIM = 16;
        private const int YDIM = 256;
        private const int ZDIM = 16;

        private NbtTree _tree;

        private int _cx;
        private int _cz;

        private AnvilSection[] _sections;

        private IDataArray3 _blocks;
        private IDataArray3 _data;
        private IDataArray3 _blockLight;
        private IDataArray3 _skyLight;

        private ZXIntArray _heightMap;
        private ZXByteArray _biomes;

        private TagNodeList _entities;
        private TagNodeList _tileEntities;
        private TagNodeList _tileTicks;

        private AlphaBlockCollection _blockManager;
        private EntityCollection _entityManager;


        private Chunk ()
        {
            _sections = new AnvilSection[16];
        }

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

        public NbtTree Tree
        {
            get { return _tree; }
        }

        public bool IsTerrainPopulated
        {
            get { return _tree.Root["Level"].ToTagCompound()["TerrainPopulated"].ToTagByte() == 1; }
            set { _tree.Root["Level"].ToTagCompound()["TerrainPopulated"].ToTagByte().Data = (byte)(value ? 1 : 0); }
        }

        public static Chunk Create (int x, int z)
        {
            Chunk c = new Chunk();

            c._cx = x;
            c._cz = z;

            c.BuildNBTTree();
            return c;
        }

        public static Chunk Create (NbtTree tree)
        {
            Chunk c = new Chunk();

            return c.LoadTree(tree.Root);
        }

        public static Chunk CreateVerified (NbtTree tree)
        {
            Chunk c = new Chunk();

            return c.LoadTreeSafe(tree.Root);
        }

        /// <summary>
        /// Updates the chunk's global world coordinates.
        /// </summary>
        /// <param name="x">Global X-coordinate.</param>
        /// <param name="z">Global Z-coordinate.</param>
        public virtual void SetLocation (int x, int z)
        {
            int diffx = (x - _cx) * XDIM;
            int diffz = (z - _cz) * ZDIM;

            // Update chunk position

            _cx = x;
            _cz = z;

            _tree.Root["Level"].ToTagCompound()["xPos"].ToTagInt().Data = x;
            _tree.Root["Level"].ToTagCompound()["zPos"].ToTagInt().Data = z;

            // Update tile entity coordinates

            List<TileEntity> tileEntites = new List<TileEntity>();
            foreach (TagNodeCompound tag in _tileEntities) {
                TileEntity te = TileEntityFactory.Create(tag);
                if (te == null) {
                    te = TileEntity.FromTreeSafe(tag);
                }

                if (te != null) {
                    te.MoveBy(diffx, 0, diffz);
                    tileEntites.Add(te);
                }
            }

            _tileEntities.Clear();
            foreach (TileEntity te in tileEntites) {
                _tileEntities.Add(te.BuildTree());
            }

            // Update tile tick coordinates

            if (_tileTicks != null) {
                List<TileTick> tileTicks = new List<TileTick>();
                foreach (TagNodeCompound tag in _tileTicks) {
                    TileTick tt = TileTick.FromTreeSafe(tag);

                    if (tt != null) {
                        tt.MoveBy(diffx, 0, diffz);
                        tileTicks.Add(tt);
                    }
                }

                _tileTicks.Clear();
                foreach (TileTick tt in tileTicks) {
                    _tileTicks.Add(tt.BuildTree());
                }
            }

            // Update entity coordinates

            List<TypedEntity> entities = new List<TypedEntity>();
            foreach (TypedEntity entity in _entityManager) {
                entity.MoveBy(diffx, 0, diffz);
                entities.Add(entity);
            }

            _entities.Clear();
            foreach (TypedEntity entity in entities) {
                _entityManager.Add(entity);
            }
        }

        public bool Save (Stream outStream)
        {
            if (outStream == null || !outStream.CanWrite) {
                return false;
            }

            BuildConditional();

            NbtTree tree = new NbtTree();
            tree.Root["Level"] = BuildTree();

            tree.WriteTo(outStream);
            outStream.Close();

            return true;
        }

        #region INbtObject<AnvilChunk> Members

        public Chunk LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null) {
                return null;
            }

            _tree = new NbtTree(ctree);

            TagNodeCompound level = _tree.Root["Level"] as TagNodeCompound;

            TagNodeList sections = level["Sections"] as TagNodeList;
            foreach (TagNodeCompound section in sections) {
                AnvilSection anvilSection = new AnvilSection(section);
                if (anvilSection.Y < 0 || anvilSection.Y >= _sections.Length)
                    continue;
                _sections[anvilSection.Y] = anvilSection;
            }

            YZXByteArray[] blocksBA = new YZXByteArray[_sections.Length];
            YZXNibbleArray[] dataBA = new YZXNibbleArray[_sections.Length];
            YZXNibbleArray[] skyLightBA = new YZXNibbleArray[_sections.Length];
            YZXNibbleArray[] blockLightBA = new YZXNibbleArray[_sections.Length];

            for (int i = 0; i < _sections.Length; i++) {
                if (_sections[i] == null)
                    _sections[i] = new AnvilSection(i);

                blocksBA[i] = _sections[i].Blocks;
                dataBA[i] = _sections[i].Data;
                skyLightBA[i] = _sections[i].SkyLight;
                blockLightBA[i] = _sections[i].BlockLight;
            }

            _blocks = new CompositeYZXByteArray(blocksBA);
            _data = new CompositeYZXNibbleArray(dataBA);
            _skyLight = new CompositeYZXNibbleArray(skyLightBA);
            _blockLight = new CompositeYZXNibbleArray(blockLightBA);
            
            _heightMap = new ZXIntArray(XDIM, ZDIM, level["HeightMap"] as TagNodeIntArray);

            if (level.ContainsKey("Biomes"))
                _biomes = new ZXByteArray(XDIM, ZDIM, level["Biomes"] as TagNodeByteArray);
            else {
                level["Biomes"] = new TagNodeByteArray(new byte[256]);
                _biomes = new ZXByteArray(XDIM, ZDIM, level["Biomes"] as TagNodeByteArray);
                for (int x = 0; x < XDIM; x++)
                    for (int z = 0; z < ZDIM; z++)
                        _biomes[x, z] = BiomeType.Default;
            }

            _entities = level["Entities"] as TagNodeList;
            _tileEntities = level["TileEntities"] as TagNodeList;

            if (level.ContainsKey("TileTicks"))
                _tileTicks = level["TileTicks"] as TagNodeList;
            else
                _tileTicks = new TagNodeList(TagType.TAG_COMPOUND);

            // List-type patch up
            if (_entities.Count == 0) {
                level["Entities"] = new TagNodeList(TagType.TAG_COMPOUND);
                _entities = level["Entities"] as TagNodeList;
            }

            if (_tileEntities.Count == 0) {
                level["TileEntities"] = new TagNodeList(TagType.TAG_COMPOUND);
                _tileEntities = level["TileEntities"] as TagNodeList;
            }

            if (_tileTicks.Count == 0) {
                level["TileTicks"] = new TagNodeList(TagType.TAG_COMPOUND);
                _tileTicks = level["TileTicks"] as TagNodeList;
            }

            _cx = level["xPos"].ToTagInt();
            _cz = level["zPos"].ToTagInt();

            _blockManager = new AlphaBlockCollection(_blocks, _data, _blockLight, _skyLight, _heightMap, _tileEntities, _tileTicks);
            _entityManager = new EntityCollection(_entities);

            return this;
        }

        public Chunk LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public TagNode BuildTree ()
        {
            TagNodeCompound level = _tree.Root["Level"] as TagNodeCompound;
            TagNodeCompound levelCopy = new TagNodeCompound();
            foreach (KeyValuePair<string, TagNode> node in level)
                levelCopy.Add(node.Key, node.Value);

            TagNodeList sections = new TagNodeList(TagType.TAG_COMPOUND);
            for (int i = 0; i < _sections.Length; i++)
                if (!_sections[i].CheckEmpty())
                    sections.Add(_sections[i].BuildTree());

            levelCopy["Sections"] = sections;

            if (_tileTicks.Count == 0)
                levelCopy.Remove("TileTicks");

            return levelCopy;
        }

        public bool ValidateTree (TagNode tree)
        {
            NbtVerifier v = new NbtVerifier(tree, LevelSchema);
            return v.Verify();
        }

        #endregion

        #region ICopyable<AnvilChunk> Members

        public Chunk Copy ()
        {
            return Chunk.Create(_tree.Copy());
        }

        #endregion

        private void BuildConditional ()
        {
            TagNodeCompound level = _tree.Root["Level"] as TagNodeCompound;
            if (_tileTicks != _blockManager.TileTicks && _blockManager.TileTicks.Count > 0) {
                _tileTicks = _blockManager.TileTicks;
                level["TileTicks"] = _tileTicks;
            }
        }

        private void BuildNBTTree ()
        {
            int elements2 = XDIM * ZDIM;

            _sections = new AnvilSection[16];
            TagNodeList sections = new TagNodeList(TagType.TAG_COMPOUND);

            for (int i = 0; i < _sections.Length; i++) {
                _sections[i] = new AnvilSection(i);
                sections.Add(_sections[i].BuildTree());
            }

            YZXByteArray[] blocksBA = new YZXByteArray[_sections.Length];
            YZXNibbleArray[] dataBA = new YZXNibbleArray[_sections.Length];
            YZXNibbleArray[] skyLightBA = new YZXNibbleArray[_sections.Length];
            YZXNibbleArray[] blockLightBA = new YZXNibbleArray[_sections.Length];

            for (int i = 0; i < _sections.Length; i++) {
                blocksBA[i] = _sections[i].Blocks;
                dataBA[i] = _sections[i].Data;
                skyLightBA[i] = _sections[i].SkyLight;
                blockLightBA[i] = _sections[i].BlockLight;
            }

            _blocks = new CompositeYZXByteArray(blocksBA);
            _data = new CompositeYZXNibbleArray(dataBA);
            _skyLight = new CompositeYZXNibbleArray(skyLightBA);
            _blockLight = new CompositeYZXNibbleArray(blockLightBA);

            TagNodeIntArray heightMap = new TagNodeIntArray(new int[elements2]);
            _heightMap = new ZXIntArray(XDIM, ZDIM, heightMap);

            TagNodeByteArray biomes = new TagNodeByteArray(new byte[elements2]);
            _biomes = new ZXByteArray(XDIM, ZDIM, biomes);
            for (int x = 0; x < XDIM; x++)
                for (int z = 0; z < ZDIM; z++)
                    _biomes[x, z] = BiomeType.Default;

            _entities = new TagNodeList(TagType.TAG_COMPOUND);
            _tileEntities = new TagNodeList(TagType.TAG_COMPOUND);
            _tileTicks = new TagNodeList(TagType.TAG_COMPOUND);

            TagNodeCompound level = new TagNodeCompound();
            level.Add("Sections", sections);
            level.Add("HeightMap", heightMap);
            level.Add("Biomes", biomes);
            level.Add("Entities", _entities);
            level.Add("TileEntities", _tileEntities);
            level.Add("TileTicks", _tileTicks);
            level.Add("LastUpdate", new TagNodeLong(Timestamp()));
            level.Add("xPos", new TagNodeInt(_cx));
            level.Add("zPos", new TagNodeInt(_cz));
            level.Add("TerrainPopulated", new TagNodeByte());

            _tree = new NbtTree();
            _tree.Root.Add("Level", level);

            _blockManager = new AlphaBlockCollection(_blocks, _data, _blockLight, _skyLight, _heightMap, _tileEntities);
            _entityManager = new EntityCollection(_entities);
        }

        private int Timestamp ()
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (int)((DateTime.UtcNow - epoch).Ticks / (10000L * 1000L));
        }
    }
}
