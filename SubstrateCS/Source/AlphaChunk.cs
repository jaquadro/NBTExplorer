using System;
using System.IO;
using System.Collections.Generic;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// A Minecraft Alpha- and Beta-compatible chunk data structure.
    /// </summary>
    /// <remarks>
    /// A Chunk internally wraps an NBT_Tree of raw chunk data.  Modifying the chunk will update the tree, and vice-versa.
    /// </remarks>
    public class AlphaChunk : IChunk, INbtObject<AlphaChunk>, ICopyable<AlphaChunk>
    {
        private const int XDIM = 16;
        private const int YDIM = 128;
        private const int ZDIM = 16;

        /// <summary>
        /// An NBT Schema definition for valid chunk data.
        /// </summary>
        public static SchemaNodeCompound LevelSchema = new SchemaNodeCompound()
        {
            new SchemaNodeCompound("Level")
            {
                new SchemaNodeArray("Blocks", 32768),
                new SchemaNodeArray("Data", 16384),
                new SchemaNodeArray("SkyLight", 16384),
                new SchemaNodeArray("BlockLight", 16384),
                new SchemaNodeArray("HeightMap", 256),
                new SchemaNodeList("Entities", TagType.TAG_COMPOUND, SchemaOptions.CREATE_ON_MISSING),
                new SchemaNodeList("TileEntities", TagType.TAG_COMPOUND, TileEntity.Schema, SchemaOptions.CREATE_ON_MISSING),
                new SchemaNodeList("TileTicks", TagType.TAG_COMPOUND, TileTick.Schema, SchemaOptions.OPTIONAL),
                new SchemaNodeScaler("LastUpdate", TagType.TAG_LONG, SchemaOptions.CREATE_ON_MISSING),
                new SchemaNodeScaler("xPos", TagType.TAG_INT),
                new SchemaNodeScaler("zPos", TagType.TAG_INT),
                new SchemaNodeScaler("TerrainPopulated", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
            },
        };

        private NbtTree _tree;

        private int _cx;
        private int _cz;

        private XZYByteArray _blocks;
        private XZYNibbleArray _data;
        private XZYNibbleArray _blockLight;
        private XZYNibbleArray _skyLight;
        private ZXByteArray _heightMap;

        private TagNodeList _entities;
        private TagNodeList _tileEntities;
        private TagNodeList _tileTicks;

        private AlphaBlockCollection _blockManager;
        private EntityCollection _entityManager;

        /// <summary>
        /// Gets the global X-coordinate of the chunk.
        /// </summary>
        public int X
        {
            get { return _cx; }
        }

        /// <summary>
        /// Gets the global Z-coordinate of the chunk.
        /// </summary>
        public int Z
        {
            get { return _cz; }
        }

        /// <summary>
        /// Gets the collection of all blocks and their data stored in the chunk.
        /// </summary>
        public AlphaBlockCollection Blocks
        {
            get { return _blockManager; }
        }

        /// <summary>
        /// Gets the collection of all entities stored in the chunk.
        /// </summary>
        public EntityCollection Entities
        {
            get { return _entityManager; }
        }

        /// <summary>
        /// Provides raw access to the underlying NBT_Tree.
        /// </summary>
        public NbtTree Tree
        {
            get { return _tree; }
        }

        /// <summary>
        /// Gets or sets the chunk's TerrainPopulated status.
        /// </summary>
        public bool IsTerrainPopulated
        {
            get { return _tree.Root["Level"].ToTagCompound()["TerrainPopulated"].ToTagByte() == 1; }
            set { _tree.Root["Level"].ToTagCompound()["TerrainPopulated"].ToTagByte().Data = (byte)(value ? 1 : 0); }
        }


        private AlphaChunk ()
        {
        }

        /// <summary>
        /// Creates a default (empty) chunk.
        /// </summary>
        /// <param name="x">Global X-coordinate of the chunk.</param>
        /// <param name="z">Global Z-coordinate of the chunk.</param>
        /// <returns>A new Chunk object.</returns>
        public static AlphaChunk Create (int x, int z)
        {
            AlphaChunk c = new AlphaChunk();

            c._cx = x;
            c._cz = z;

            c.BuildNBTTree();
            return c;
        }

        /// <summary>
        /// Creates a chunk object from an existing NBT_Tree.
        /// </summary>
        /// <param name="tree">An NBT_Tree conforming to the chunk schema definition.</param>
        /// <returns>A new Chunk object wrapping an existing NBT_Tree.</returns>
        public static AlphaChunk Create (NbtTree tree)
        {
            AlphaChunk c = new AlphaChunk();

            return c.LoadTree(tree.Root);
        }

        /// <summary>
        /// Creates a chunk object from a verified NBT_Tree.
        /// </summary>
        /// <param name="tree">An NBT_Tree conforming to the chunk schema definition.</param>
        /// <returns>A new Chunk object wrapping an existing NBT_Tree, or null on verification failure.</returns>
        public static AlphaChunk CreateVerified (NbtTree tree)
        {
            AlphaChunk c = new AlphaChunk();

            return c.LoadTreeSafe(tree.Root);
        }

        /// <summary>
        /// Updates the chunk's global world coordinates.
        /// </summary>
        /// <param name="x">Global X-coordinate.</param>
        /// <param name="z">Global Z-coordinate.</param>
        public void SetLocation (int x, int z)
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

        /// <summary>
        /// Saves a Chunk's underlying NBT_Tree to an output stream.
        /// </summary>
        /// <param name="outStream">An open, writable output stream.</param>
        /// <returns>True if the data is written out to the stream.</returns>
        public bool Save (Stream outStream)
        {
            if (outStream == null || !outStream.CanWrite) {
                return false;
            }

            BuildConditional();

            _tree.WriteTo(outStream);
            outStream.Close();

            return true;
        }


        #region INBTObject<Chunk> Members

        /// <summary>
        /// Loads the Chunk from an NBT tree rooted at the given TagValue node.
        /// </summary>
        /// <param name="tree">Root node of an NBT tree.</param>
        /// <returns>A reference to the current Chunk, or null if the tree is unparsable.</returns>
        public AlphaChunk LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null) {
                return null;
            }

            _tree = new NbtTree(ctree);

            TagNodeCompound level = _tree.Root["Level"] as TagNodeCompound;

            _blocks = new XZYByteArray(XDIM, YDIM, ZDIM, level["Blocks"] as TagNodeByteArray);
            _data = new XZYNibbleArray(XDIM, YDIM, ZDIM, level["Data"] as TagNodeByteArray);
            _blockLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, level["BlockLight"] as TagNodeByteArray);
            _skyLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, level["SkyLight"] as TagNodeByteArray);
            _heightMap = new ZXByteArray(XDIM, ZDIM, level["HeightMap"] as TagNodeByteArray);

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

        /// <summary>
        /// Loads the Chunk from a validated NBT tree rooted at the given TagValue node.
        /// </summary>
        /// <param name="tree">Root node of an NBT tree.</param>
        /// <returns>A reference to the current Chunk, or null if the tree does not conform to the chunk's NBT Schema definition.</returns>
        public AlphaChunk LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        /// <summary>
        /// Gets a valid NBT tree representing the Chunk.
        /// </summary>
        /// <returns>The root node of the Chunk's NBT tree.</returns>
        public TagNode BuildTree ()
        {
            BuildConditional();

            return _tree.Root;
        }

        /// <summary>
        /// Validates an NBT tree against the chunk's NBT schema definition.
        /// </summary>
        /// <param name="tree">The root node of the NBT tree to verify.</param>
        /// <returns>Status indicating if the tree represents a valid chunk.</returns>
        public bool ValidateTree (TagNode tree)
        {
            NbtVerifier v = new NbtVerifier(tree, LevelSchema);
            return v.Verify();
        }

        #endregion


        #region ICopyable<Chunk> Members

        /// <summary>
        /// Creates a deep copy of the Chunk and its underlying NBT tree.
        /// </summary>
        /// <returns>A new Chunk with copied data.</returns>
        public AlphaChunk Copy ()
        {
            return AlphaChunk.Create(_tree.Copy());
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
            int elements3 = elements2 * YDIM;

            TagNodeByteArray blocks = new TagNodeByteArray(new byte[elements3]);
            TagNodeByteArray data = new TagNodeByteArray(new byte[elements3 >> 1]);
            TagNodeByteArray blocklight = new TagNodeByteArray(new byte[elements3 >> 1]);
            TagNodeByteArray skylight = new TagNodeByteArray(new byte[elements3 >> 1]);
            TagNodeByteArray heightMap = new TagNodeByteArray(new byte[elements2]);

            _blocks = new XZYByteArray(XDIM, YDIM, ZDIM, blocks);
            _data = new XZYNibbleArray(XDIM, YDIM, ZDIM, data);
            _blockLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, blocklight);
            _skyLight = new XZYNibbleArray(XDIM, YDIM, ZDIM, skylight);
            _heightMap = new ZXByteArray(XDIM, ZDIM, heightMap);

            _entities = new TagNodeList(TagType.TAG_COMPOUND);
            _tileEntities = new TagNodeList(TagType.TAG_COMPOUND);
            _tileTicks = new TagNodeList(TagType.TAG_COMPOUND);

            TagNodeCompound level = new TagNodeCompound();
            level.Add("Blocks", blocks);
            level.Add("Data", data);
            level.Add("SkyLight", blocklight);
            level.Add("BlockLight", skylight);
            level.Add("HeightMap", heightMap);
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
