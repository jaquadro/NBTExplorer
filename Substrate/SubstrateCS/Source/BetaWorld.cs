using System;
using System.Collections.Generic;
using System.IO;
using Substrate.Core;
using Substrate.Nbt;

//TODO: Exceptions (+ Alpha)

namespace Substrate
{
    using IO = System.IO;

    /// <summary>
    /// Represents a Beta-compatible (Beta 1.3 or higher) Minecraft world.
    /// </summary>
    public class BetaWorld : NbtWorld
    {
        private const string _REGION_DIR = "region";
        private const string _PLAYER_DIR = "players";
        private string _levelFile = "level.dat";

        private Level _level;

        private Dictionary<int, RegionManager> _regionMgrs;
        private Dictionary<int, ChunkManager> _chunkMgrs;
        private Dictionary<int, BlockManager> _blockMgrs;

        private PlayerManager _playerMan;

        private BetaWorld ()
        {
            _regionMgrs = new Dictionary<int, RegionManager>();
            _chunkMgrs = new Dictionary<int, ChunkManager>();
            _blockMgrs = new Dictionary<int, BlockManager>();
        }

        /// <summary>
        /// Gets a reference to this world's <see cref="Level"/> object.
        /// </summary>
        public override Level Level
        {
            get { return _level; }
        }

        /// <summary>
        /// Gets a <see cref="BlockManager"/> for the default dimension.
        /// </summary>
        /// <returns>A <see cref="BlockManager"/> tied to the default dimension in this world.</returns>
        /// <remarks>Get a <see cref="BlockManager"/> if you need to manage blocks as a global, unbounded matrix.  This abstracts away
        /// any higher-level organizational divisions.  If your task is going to be heavily performance-bound, consider getting a
        /// <see cref="ChunkManager"/> instead and working with blocks on a chunk-local level.</remarks>
        public new BlockManager GetBlockManager ()
        {
            return GetBlockManagerVirt(Dimension.DEFAULT) as BlockManager;
        }

        /// <summary>
        /// Gets a <see cref="BlockManager"/> for the given dimension.
        /// </summary>
        /// <param name="dim">The id of the dimension to look up.</param>
        /// <returns>A <see cref="BlockManager"/> tied to the given dimension in this world.</returns>
        /// <remarks>Get a <see cref="BlockManager"/> if you need to manage blocks as a global, unbounded matrix.  This abstracts away
        /// any higher-level organizational divisions.  If your task is going to be heavily performance-bound, consider getting a
        /// <see cref="ChunkManager"/> instead and working with blocks on a chunk-local level.</remarks>
        public new BlockManager GetBlockManager (int dim)
        {
            return GetBlockManagerVirt(dim) as BlockManager;
        }

        /// <summary>
        /// Gets a <see cref="ChunkManager"/> for the default dimension.
        /// </summary>
        /// <returns>A <see cref="ChunkManager"/> tied to the default dimension in this world.</returns>
        /// <remarks>Get a <see cref="ChunkManager"/> if you you need to work with easily-digestible, bounded chunks of blocks.</remarks>
        public new ChunkManager GetChunkManager ()
        {
            return GetChunkManagerVirt(Dimension.DEFAULT) as ChunkManager;
        }

        /// <summary>
        /// Gets a <see cref="ChunkManager"/> for the given dimension.
        /// </summary>
        /// <param name="dim">The id of the dimension to look up.</param>
        /// <returns>A <see cref="ChunkManager"/> tied to the given dimension in this world.</returns>
        /// <remarks>Get a <see cref="ChunkManager"/> if you you need to work with easily-digestible, bounded chunks of blocks.</remarks>
        public new ChunkManager GetChunkManager (int dim)
        {
            return GetChunkManagerVirt(dim) as ChunkManager;
        }

        /// <summary>
        /// Gets a <see cref="RegionManager"/> for the default dimension.
        /// </summary>
        /// <returns>A <see cref="RegionManager"/> tied to the defaul dimension in this world.</returns>
        /// <remarks>Regions are a higher-level unit of organization for blocks unique to worlds created in Beta 1.3 and beyond.
        /// Consider using the <see cref="ChunkManager"/> if you are interested in working with blocks.</remarks>
        public RegionManager GetRegionManager ()
        {
            return GetRegionManager(Dimension.DEFAULT);
        }

        /// <summary>
        /// Gets a <see cref="RegionManager"/> for the given dimension.
        /// </summary>
        /// <param name="dim">The id of the dimension to look up.</param>
        /// <returns>A <see cref="RegionManager"/> tied to the given dimension in this world.</returns>
        /// <remarks>Regions are a higher-level unit of organization for blocks unique to worlds created in Beta 1.3 and beyond.
        /// Consider using the <see cref="ChunkManager"/> if you are interested in working with blocks.</remarks>
        public RegionManager GetRegionManager (int dim)
        {
            RegionManager rm;
            if (_regionMgrs.TryGetValue(dim, out rm)) {
                return rm;
            }

            OpenDimension(dim);
            return _regionMgrs[dim];
        }

        /// <summary>
        /// Gets a <see cref="PlayerManager"/> for maanging players on multiplayer worlds.
        /// </summary>
        /// <returns>A <see cref="PlayerManager"/> for this world.</returns>
        /// <remarks>To manage the player of a single-player world, get a <see cref="Level"/> object for the world instead.</remarks>
        public new PlayerManager GetPlayerManager ()
        {
            return GetPlayerManagerVirt() as PlayerManager;
        }

        /// <summary>
        /// Saves the world's <see cref="Level"/> data, and any <see cref="Chunk"/> objects known to have unsaved changes.
        /// </summary>
        public void Save ()
        {
            _level.Save();

            foreach (KeyValuePair<int, ChunkManager> cm in _chunkMgrs) {
                cm.Value.Save();
            }
        }

        /// <summary>
        /// Opens an existing Beta-compatible Minecraft world and returns a new <see cref="BetaWorld"/> to represent it.
        /// </summary>
        /// <param name="path">The path to the directory containing the world's level.dat, or the path to level.dat itself.</param>
        /// <returns>A new <see cref="BetaWorld"/> object representing an existing world on disk.</returns>
        public static BetaWorld Open (string path)
        {
            return new BetaWorld().OpenWorld(path) as BetaWorld;
        }

        /// <summary>
        /// Creates a new Beta-compatible Minecraft world and returns a new <see cref="BetaWorld"/> to represent it.
        /// </summary>
        /// <param name="path">The path to the directory where the new world should be stored.</param>
        /// <returns>A new <see cref="BetaWorld"/> object representing a new world.</returns>
        /// <remarks>This method will attempt to create the specified directory immediately if it does not exist, but will not
        /// write out any world data unless it is explicitly saved at a later time.</remarks>
        public static BetaWorld Create (string path)
        {
            return new BetaWorld().CreateWorld(path) as BetaWorld;
        }

        /// <exclude/>
        protected override IBlockManager GetBlockManagerVirt (int dim)
        {
            BlockManager rm;
            if (_blockMgrs.TryGetValue(dim, out rm)) {
                return rm;
            }

            OpenDimension(dim);
            return _blockMgrs[dim];
        }

        /// <exclude/>
        protected override IChunkManager GetChunkManagerVirt (int dim)
        {
            ChunkManager rm;
            if (_chunkMgrs.TryGetValue(dim, out rm)) {
                return rm;
            }

            OpenDimension(dim);
            return _chunkMgrs[dim];
        }

        /// <exclude/>
        protected override IPlayerManager GetPlayerManagerVirt ()
        {
            if (_playerMan != null) {
                return _playerMan;
            }

            string path = IO.Path.Combine(Path, _PLAYER_DIR);

            _playerMan = new PlayerManager(path);
            return _playerMan;
        }

        private void OpenDimension (int dim)
        {
            string path = Path;
            if (dim == Dimension.DEFAULT) {
                path = IO.Path.Combine(path, _REGION_DIR);
            }
            else {
                path = IO.Path.Combine(path, "DIM" + dim);
                path = IO.Path.Combine(path, _REGION_DIR);
            }

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            ChunkCache cc = new ChunkCache();

            RegionManager rm = new RegionManager(path, cc);
            ChunkManager cm = new ChunkManager(rm, cc);
            BlockManager bm = new BlockManager(cm);

            _regionMgrs[dim] = rm;
            _chunkMgrs[dim] = cm;
            _blockMgrs[dim] = bm;
        }

        private BetaWorld OpenWorld (string path)
        {
            if (!Directory.Exists(path)) {
                if (File.Exists(path)) {
                    _levelFile = IO.Path.GetFileName(path);
                    path = IO.Path.GetDirectoryName(path);
                }
                else {
                    throw new DirectoryNotFoundException("Directory '" + path + "' not found");
                }
            }

            Path = path;

            string ldat = IO.Path.Combine(path, _levelFile);
            if (!File.Exists(ldat)) {
                throw new FileNotFoundException("Data file '" + _levelFile + "' not found in '" + path + "'", ldat);
            }

            if (!LoadLevel()) {
                throw new Exception("Failed to load '" + _levelFile + "'");
            }

            return this;
        }

        private BetaWorld CreateWorld (string path)
        {
            if (!Directory.Exists(path)) {
                throw new DirectoryNotFoundException("Directory '" + path + "' not found");
            }

            string regpath = IO.Path.Combine(path, _REGION_DIR);
            if (!Directory.Exists(regpath)) {
                Directory.CreateDirectory(regpath);
            }

            Path = path;
            _level = new Level(this);

            return this;
        }

        private bool LoadLevel ()
        {
            NBTFile nf = new NBTFile(IO.Path.Combine(Path, _levelFile));
            Stream nbtstr = nf.GetDataInputStream();
            if (nbtstr == null) {
                return false;
            }

            NbtTree tree = new NbtTree(nbtstr);

            _level = new Level(this);
            _level = _level.LoadTreeSafe(tree.Root);

            return _level != null;
        }

        internal static void OnResolveOpen (object sender, OpenWorldEventArgs e)
        {
            try {
                BetaWorld world = new BetaWorld().OpenWorld(e.Path);
                if (world == null) {
                    return;
                }

                string regPath = IO.Path.Combine(e.Path, _REGION_DIR);
                if (!Directory.Exists(regPath)) {
                    return;
                }

                if (world.Level.Version < 19132) {
                    return;
                }

                e.AddHandler(Open);
            }
            catch (Exception) {
                return;
            }
        }
    }
}
