using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using Substrate;

namespace NBToolkit
{
    public class OregenOptions : TKOptions, IChunkFilterable
    {
        private OptionSet _filterOpt = null;
        private ChunkFilter _chunkFilter = null;

        public int? OPT_ID = null;

        public int? OPT_DATA = null;

        public int? OPT_ROUNDS = null;
        public int? OPT_SIZE = null;
        public int? OPT_MIN = null;
        public int? OPT_MAX = null;

        public bool OPT_OO = false;
        public bool OPT_OA = false;

        public bool OPT_MATHFIX = true;
        
        public List<int> OPT_OB_INCLUDE = new List<int>();
        public List<int> OPT_OB_EXCLUDE = new List<int>();

        private class OreType
        {
            public int id;
            public string name;
            public int rounds;
            public int min;
            public int max;
            public int size;
        };

        private OreType[] oreList = new OreType[] {
            new OreType() { id = 16, name = "Coal", rounds = 20, min = 0, max = 127, size = 16 },
            new OreType() { id = 15, name = "Iron", rounds = 20, min = 0, max = 63, size = 8 },
            new OreType() { id = 14, name = "Gold", rounds = 2, min = 0, max = 31, size = 8 },
            new OreType() { id = 73, name = "Redstone", rounds = 8, min = 0, max = 31, size = 7 },
            new OreType() { id = 56, name = "Diamond", rounds = 1, min = 0, max = 15, size = 7 },
            new OreType() { id = 21, name = "Lapis", rounds = 1, min = 0, max = 31, size = 7 },
        };

        public OregenOptions ()
            : base()
        {            
            _filterOpt = new OptionSet()
            {
                { "b|Block=", "Generate blocks of type {ID} (0-255)",
                    v => OPT_ID = Convert.ToByte(v) % 256 },
                { "d|Data=", "Set the block's data value to {VAL} (0-15)",
                    v => OPT_DATA = Convert.ToInt32(v) % 16 },
                { "r|Rounds=", "Geneate {NUM} deposits per chunk",
                    v => OPT_ROUNDS = Convert.ToInt32(v) },
                { "min|MinDepth=", "Generates deposits no lower than depth {VAL} (0-127)",
                    v => OPT_MIN = Convert.ToInt32(v) % 128 },
                { "max|MaxDepth=", "Generates deposits no higher than depth {VAL} (0-127)",
                    v => OPT_MAX = Convert.ToInt32(v) % 128 },
                { "s|Size=", "Generates deposits containing roughly up to {VAL} blocks",
                    v => OPT_SIZE = Convert.ToInt32(v) % 128 },
                { "oo|OverrideOres", "Generated deposits can replace other existing ores",
                    v => OPT_OO = true },
                { "oa|OverrideAll", "Generated deposits can replace any existing block",
                    v => OPT_OA = true },
                { "oi|OverrideInclude=", "Generated deposits can replace the specified block type {ID} [repeatable]",
                    v => OPT_OB_INCLUDE.Add(Convert.ToInt32(v) % 256) },
                { "ox|OverrideExclude=", "Generated deposits can never replace the specified block type {ID} [repeatable]",
                    v => OPT_OB_EXCLUDE.Add(Convert.ToInt32(v) % 256) },
                { "nu|NativeUnpatched", "Use MC native ore generation algorithm without distribution evenness patch",
                    v => OPT_MATHFIX = false },
            };

            _chunkFilter = new ChunkFilter();
        }

        public OregenOptions (string[] args)
            : this()
        {
            Parse(args);
        }

        public override void Parse (string[] args)
        {
            base.Parse(args);

            _filterOpt.Parse(args);
            _chunkFilter.Parse(args);
        }

        public override void PrintUsage ()
        {
            Console.WriteLine("Usage: nbtoolkit oregen -b <id> -w <path> [options]");
            Console.WriteLine();
            Console.WriteLine("Options for command 'oregen':");

            _filterOpt.WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            _chunkFilter.PrintUsage();

            Console.WriteLine();
            base.PrintUsage();
        }

        public override void SetDefaults ()
        {
            base.SetDefaults();

            foreach (OreType ore in oreList) {
                if (OPT_ID != ore.id) {
                    continue;
                }

                if (OPT_ROUNDS == null) {
                    OPT_ROUNDS = ore.rounds;
                }
                if (OPT_MIN == null) {
                    OPT_MIN = ore.min;
                }
                if (OPT_MAX == null) {
                    OPT_MAX = ore.max;
                }
                if (OPT_SIZE == null) {
                    OPT_SIZE = ore.size;
                }
            }

            // Check for required parameters
            if (OPT_ID == null) {
                Console.WriteLine("Error: You must specify a Block ID");
                Console.WriteLine();
                PrintUsage();

                throw new TKOptionException();
            }

            if (OPT_ROUNDS == null) {
                OPT_ROUNDS = 1;
            }

            if (OPT_MIN == null || OPT_MAX == null || OPT_SIZE == null) {
                if (OPT_MIN == null) {
                    Console.WriteLine("Error: You must specify the minimum depth for non-ore blocks");
                }
                if (OPT_MAX == null) {
                    Console.WriteLine("Error: You must specify the maximum depth for non-ore blocks");
                }
                if (OPT_SIZE == null) {
                    Console.WriteLine("Error: You must specify the deposit size for non-ore blocks");
                }

                Console.WriteLine();
                PrintUsage();

                throw new TKOptionException();
            }
        }

        public IChunkFilter GetChunkFilter ()
        {
            return _chunkFilter;
        }
    }

    public class Oregen : TKFilter
    {
        private OregenOptions opt;

        private static Random rand = new Random();

        public Oregen (OregenOptions o)
        {
            opt = o;
        }

        public override void Run ()
        {
            World world = new World(opt.OPT_WORLD);

            IChunkManager cm = world.GetChunkManager();
            FilteredChunkManager fcm = new FilteredChunkManager(cm, opt.GetChunkFilter());

            int affectedChunks = 0;
            foreach (ChunkRef chunk in fcm) {
                if (chunk == null || !chunk.IsTerrainPopulated) {
                    continue;
                }

                if (opt.OPT_V) {
                    Console.WriteLine("Processing Chunk (" + chunk.X + "," + chunk.Z + ")");
                }

                affectedChunks++;

                ApplyChunk(world, chunk);

                world.GetChunkManager().Save();
            }

            Console.WriteLine("Affected Chunks: " + affectedChunks);
        }

        public void ApplyChunk (World world, ChunkRef chunk)
        {
            if (opt.OPT_V) {
                Console.WriteLine("Generating {0} size {1} deposits of {2} between {3} and {4}",
                    opt.OPT_ROUNDS, opt.OPT_SIZE, opt.OPT_ID, opt.OPT_MIN, opt.OPT_MAX);
            }

            IGenerator generator;
            if (opt.OPT_DATA == null) {
                generator = new NativeGenOre((int)opt.OPT_ID, (int)opt.OPT_SIZE);
                ((NativeGenOre)generator).MathFix = opt.OPT_MATHFIX;
            }
            else {
                generator = new NativeGenOre((int)opt.OPT_ID, (int)opt.OPT_DATA, (int)opt.OPT_SIZE);
                ((NativeGenOre)generator).MathFix = opt.OPT_MATHFIX;
            }

            BlockManager bm = new GenOreBlockManager(world.GetChunkManager(), opt);

            for (int i = 0; i < opt.OPT_ROUNDS; i++) {
                if (opt.OPT_VV) {
                    Console.WriteLine("Generating round {0}...", i);
                }

                int x = chunk.X * chunk.XDim + rand.Next(chunk.XDim);
                int y = (int)opt.OPT_MIN + rand.Next((int)opt.OPT_MAX - (int)opt.OPT_MIN);
                int z = chunk.Z * chunk.ZDim + rand.Next(chunk.ZDim);

                generator.Generate(bm, rand, x, y, z);
            }
        }
    }

    public class GenOreBlockManager : BlockManager
    {
        public const int BLOCK_STONE = 1;
        public const int BLOCK_DIRT = 3;
        public const int BLOCK_GRAVEL = 13;
        public const int BLOCK_GOLD = 14;
        public const int BLOCK_IRON = 15;
        public const int BLOCK_COAL = 16;
        public const int BLOCK_LAPIS = 21;
        public const int BLOCK_DIAMOND = 56;
        public const int BLOCK_REDSTONE = 73;

        protected OregenOptions opt;

        public GenOreBlockManager (IChunkManager bm, OregenOptions o)
            : base(bm)
        {
            opt = o;
        }

        protected override bool Check (int x, int y, int z)
        {
            if (!base.Check(x, y, z)) {
                return false;
            }

            int blockID = _cache.GetBlockID(x & _chunkXMask, y & _chunkYMask, z & _chunkZMask);

            if (
                ((opt.OPT_OA) && (blockID != opt.OPT_ID)) ||
                ((opt.OPT_OO) && (
                    blockID == BLOCK_COAL || blockID == BLOCK_IRON ||
                    blockID == BLOCK_GOLD || blockID == BLOCK_REDSTONE ||
                    blockID == BLOCK_DIAMOND || blockID == BLOCK_LAPIS ||
                    blockID == BLOCK_DIRT || blockID == BLOCK_GRAVEL) && (blockID != opt.OPT_ID)) ||
                (opt.OPT_OB_INCLUDE.Count > 0) ||
                (blockID == BLOCK_STONE)
            ) {
                // If overriding list of ores, check membership
                if (opt.OPT_OB_INCLUDE.Count > 0 && !opt.OPT_OB_INCLUDE.Contains(blockID)) {
                    return false;
                }

                // Check for any excluded block
                if (opt.OPT_OB_EXCLUDE.Contains(blockID)) {
                    return false;
                }

                // We're allowed to update the block
                return true;
            }

            return false;
        }

        /*public override BlockRef GetBlockRef (int x, int y, int z)
        {
            BlockRef block;
            try {
                block = base.GetBlockRef(x, y, z);
            }
            catch {
                return null;
            }

            if (block == null) {
                return null;
            }

            int blockID = block.ID;

            if (
                ((opt.OPT_OA) && (blockID != opt.OPT_ID)) ||
                ((opt.OPT_OO) && (
                    blockID == BLOCK_COAL || blockID == BLOCK_IRON ||
                    blockID == BLOCK_GOLD || blockID == BLOCK_REDSTONE ||
                    blockID == BLOCK_DIAMOND || blockID == BLOCK_LAPIS ||
                    blockID == BLOCK_DIRT || blockID == BLOCK_GRAVEL) && (blockID != opt.OPT_ID)) ||
                (opt.OPT_OB_INCLUDE.Count > 0) ||
                (blockID == BLOCK_STONE)
            ) {
                // If overriding list of ores, check membership
                if (opt.OPT_OB_INCLUDE.Count > 0 && !opt.OPT_OB_INCLUDE.Contains(blockID)) {
                    return null;
                }

                // Check for any excluded block
                if (opt.OPT_OB_EXCLUDE.Contains(blockID)) {
                    return null;
                }

                // We're allowed to update the block
                return block;
            }

            return null;
        }

        public override bool SetBlockID (int x, int y, int z, int id)
        {
            int blockID = 0;
            try {
                blockID = GetBlockID(x, y, z);
            }
            catch {
                return false;
            }

            if (
                ((opt.OPT_OA) && (blockID != opt.OPT_ID)) ||
                ((opt.OPT_OO) && (
                    blockID == BLOCK_COAL || blockID == BLOCK_IRON ||
                    blockID == BLOCK_GOLD || blockID == BLOCK_REDSTONE ||
                    blockID == BLOCK_DIAMOND || blockID == BLOCK_LAPIS ||
                    blockID == BLOCK_DIRT || blockID == BLOCK_GRAVEL) && (blockID != opt.OPT_ID)) ||
                (opt.OPT_OB_INCLUDE.Count > 0) ||
                (blockID == BLOCK_STONE)
            ) {
                // If overriding list of ores, check membership
                if (opt.OPT_OB_INCLUDE.Count > 0 && !opt.OPT_OB_INCLUDE.Contains(blockID)) {
                    return false;
                }

                // Check for any excluded block
                if (opt.OPT_OB_EXCLUDE.Contains(blockID)) {
                    return false;
                }

                // We're allowed to update the block
                return base.SetBlockID(x, y, z, id);
            }

            return false;
        }*/
    }
}
