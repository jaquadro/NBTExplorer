using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using NDesk.Options;
using Substrate;

namespace NBToolkit
{
    public class ReplaceOptions : TKOptions, IChunkFilterable, IBlockFilterable
    {
        private OptionSet _filterOpt = null;
        private ChunkFilter _chunkFilter = null;
        private BlockFilter _blockFilter = null;        

        public int? OPT_BEFORE = null;
        public int? OPT_AFTER = null;

        public int? OPT_DATA = null;
        public double? OPT_PROB = null;

        // Block coordinate conditions
        public int? BL_X_GE = null;
        public int? BL_X_LE = null;
        public int? BL_Y_GE = null;
        public int? BL_Y_LE = null;
        public int? BL_Z_GE = null;
        public int? BL_Z_LE = null;

        // Neighbor conditions
        public int? OPT_NEIGHBOR = null;
        public int? OPT_NEIGHBOR_SIDE = null;
        public int? OPT_NEIGHBOR_E = null;
        public int? OPT_NEIGHBOR_W = null;
        public int? OPT_NEIGHBOR_N = null;
        public int? OPT_NEIGHBOR_S = null;
        public int? OPT_NEIGHBOR_T = null;
        public int? OPT_NEIGHBOR_B = null;

        public ReplaceOptions () 
            : base()
        {
            _filterOpt = new OptionSet()
            {
                /*{ "b|before=", "Replace instances of block type {ID} with another block type.  This option is repeatable.",
                    v => _includedBlocks.Add(Convert.ToInt32(v) % 256) },*/
                { "a|after=", "Replace the selected blocks with block type {ID}",
                    v => OPT_AFTER = Convert.ToInt32(v) % 256 },
                { "d|data=", "Set the new block's data value to {VAL} (0-15)",
                    v => OPT_DATA = Convert.ToInt32(v) % 16 },
                /*{ "p|prob=", "Replace any matching block with probability {VAL} (0.0-1.0)",
                    v => { OPT_PROB = Convert.ToDouble(v, new CultureInfo("en-US")); 
                           OPT_PROB = Math.Max((double)OPT_PROB, 0.0); 
                           OPT_PROB = Math.Min((double)OPT_PROB, 1.0); } },
                { "bxr|BlockXRange=", "Update blocks with X-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank.",
                    (v1, v2) => { 
                        try { BL_X_GE = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { BL_X_LE = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "byr|BlockYRange=", "Update blocks with Y-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank",
                    (v1, v2) => { 
                        try { BL_Y_GE = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { BL_Y_LE = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "bzr|BlockZRange=", "Update blocks with Z-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank",
                    (v1, v2) => { 
                        try { BL_Z_GE = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { BL_Z_LE = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },*/
                /*{ "nb=", "Update blocks that have block type {ID} as any neighbor",
                    v => OPT_NEIGHBOR = Convert.ToInt32(v) % 256 },
                { "nbs=", "Update blocks that have block type {ID} as any x/z neighbor",
                    v => OPT_NEIGHBOR_SIDE = Convert.ToInt32(v) % 256 },
                { "nbxa=", "Update blocks that have block type {ID} as their south neighbor",
                    v => OPT_NEIGHBOR_S = Convert.ToInt32(v) % 256 },
                { "nbxb=", "Update blocks that have block type {ID} as their north neighbor",
                    v => OPT_NEIGHBOR_N = Convert.ToInt32(v) % 256 },
                { "nbya=", "Update blocks that have block type {ID} as their top neighbor",
                    v => OPT_NEIGHBOR_T = Convert.ToInt32(v) % 256 },
                { "nbyb=", "Update blocks that have block type {ID} as their bottom neighbor",
                    v => OPT_NEIGHBOR_B = Convert.ToInt32(v) % 256 },
                { "nbza=", "Update blocks that have block type {ID} as their west neighbor",
                    v => OPT_NEIGHBOR_W = Convert.ToInt32(v) % 256 },
                { "nbzb=", "Update blocks that have block type {ID} as their east neighbor",
                    v => OPT_NEIGHBOR_E = Convert.ToInt32(v) % 256 },*/
            };

            _chunkFilter = new ChunkFilter();
            _blockFilter = new BlockFilter();
        }

        public ReplaceOptions (string[] args)
            : this()
        {
            Parse(args);
        }

        public override void Parse (string[] args)
        {
            base.Parse(args);

            _filterOpt.Parse(args);
            _chunkFilter.Parse(args);
            _blockFilter.Parse(args);
        }

        public override void PrintUsage ()
        {
            Console.WriteLine("Usage: nbtoolkit replace -a <id> [options]");
            Console.WriteLine();
            Console.WriteLine("Options for command 'replace':");

            _filterOpt.WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            _chunkFilter.PrintUsage();

            Console.WriteLine();
            _blockFilter.PrintUsage();

            Console.WriteLine();
            base.PrintUsage();
        }

        public override void SetDefaults ()
        {
            base.SetDefaults();

            // Check for required parameters
            if (OPT_AFTER == null) {
                Console.WriteLine("Error: You must specify a replacement Block ID");
                Console.WriteLine();
                PrintUsage();

                throw new TKOptionException();
            }
        }

        public IChunkFilter GetChunkFilter ()
        {
            return _chunkFilter;
        }

        public IBlockFilter GetBlockFilter ()
        {
            return _blockFilter;
        }
    }

    public class Replace : TKFilter
    {
        private ReplaceOptions opt;

        private static Random rand = new Random();

        public Replace (ReplaceOptions o)
        {
            opt = o;
        }

        public override void Run ()
        {
            INBTWorld world = GetWorld(opt);
            IChunkManager cm = world.GetChunkManager(opt.OPT_DIM);
            FilteredChunkManager fcm = new FilteredChunkManager(cm, opt.GetChunkFilter());

            int affectedChunks = 0;
            foreach (ChunkRef chunk in fcm) {
                affectedChunks++;

                ApplyChunk(world, chunk);

                fcm.Save();
            }

            Console.WriteLine("Affected Chunks: " + affectedChunks);
        }

        public void ApplyChunk (INBTWorld world, ChunkRef chunk)
        {
            IBlockFilter opt_b = opt.GetBlockFilter();

            int xBase = chunk.X * chunk.Blocks.XDim;
            int zBase = chunk.Z * chunk.Blocks.ZDim;

            // Determine X range
            int xmin = 0;
            int xmax = 15;

            if (opt_b.XAboveEq != null) {
                xmin = (int)opt_b.XAboveEq - xBase;
            }
            if (opt_b.XBelowEq != null) {
                xmax = (int)opt_b.XBelowEq - xBase;
            }

            xmin = (xmin < 0) ? 0 : xmin;
            xmax = (xmin > 15) ? 15 : xmax;

            if (xmin > 15 || xmax < 0 || xmin > xmax) {
                return;
            }

            // Determine Y range
            int ymin = 0;
            int ymax = 127;

            if (opt_b.YAboveEq != null) {
                ymin = (int)opt_b.YAboveEq;
            }
            if (opt_b.YBelowEq != null) {
                ymax = (int)opt_b.YBelowEq;
            }

            if (ymin > ymax) {
                return;
            }

            // Determine X range
            int zmin = 0;
            int zmax = 15;

            if (opt_b.ZAboveEq != null) {
                zmin = (int)opt_b.ZAboveEq - zBase;
            }
            if (opt_b.ZBelowEq != null) {
                zmax = (int)opt_b.ZBelowEq - zBase;
            }

            zmin = (zmin < 0) ? 0 : zmin;
            zmax = (zmin > 15) ? 15 : zmax;

            if (zmin > 15 || zmax < 0 || zmin > zmax) {
                return;
            }

            int xdim = chunk.Blocks.XDim;
            int ydim = chunk.Blocks.YDim;
            int zdim = chunk.Blocks.ZDim;

            // Process Chunk
            for (int y = ymin; y <= ymax; y++) {
                for (int x = xmin; x <= xmax; x++) {
                    for (int z = zmin; z <= zmax; z++) {
                        // Probability test
                        if (opt_b.ProbMatch != null) {
                            double c = rand.NextDouble();
                            if (c > opt_b.ProbMatch) {
                                continue;
                            }
                        }

                        int lx = x % xdim;
                        int ly = y % ydim;
                        int lz = z % zdim;

                        // Get the old block
                        int oldBlock = chunk.Blocks.GetID(lx , ly, lz);

                        // Skip block if it doesn't match the inclusion list
                        if (opt_b.IncludedBlockCount > 0) {
                            bool match = false;
                            foreach (int ib in opt_b.IncludedBlocks) {
                                if (oldBlock == ib) {
                                    match = true;
                                    break;
                                }
                            }

                            if (!match) {
                                continue;
                            }
                        }

                        // Skip block if it does match the exclusion list
                        if (opt_b.ExcludedBlockCount > 0) {
                            bool match = false;
                            foreach (int xb in opt_b.ExcludedBlocks) {
                                if (oldBlock == xb) {
                                    match = true;
                                    break;
                                }
                            }

                            if (match) {
                                continue;
                            }
                        }

                        // Replace the block
                        chunk.Blocks.SetID(lx, ly, lz, (int)opt.OPT_AFTER);

                        if (opt.OPT_VV) {
                            int gx = chunk.X * xdim + lx;
                            int gz = chunk.Z * zdim + lz;
                            Console.WriteLine("Replaced block at {0},{1},{2}", gx, ly, gz);
                        }

                        if (opt.OPT_DATA != null) {
                            chunk.Blocks.SetData(lx, ly, lz, (int)opt.OPT_DATA);
                        }
                    }
                }
            }
        }
    }
}
