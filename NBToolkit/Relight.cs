using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using Substrate;
using Substrate.Core;

namespace NBToolkit
{
    public class RelightOptions : TKOptions, IChunkFilterable
    {
        private OptionSet _filterOpt = null;
        private ChunkFilter _chunkFilter = null;

        public bool BlockLight = false;
        public bool SkyLight = false;
        public bool HeightMap = false;

        public RelightOptions ()
            : base()
        {
            _filterOpt = new OptionSet()
            {
                { "b|BlockLight", "Recalculate the block light values (block light sources) of selected chunks",
                    v => BlockLight = true },
                { "s|SkyLight", "Recalculate the skylight values (natural sunlight) of selected chunks",
                    v => SkyLight = true },
                { "m|HeightMap", "Recalculate the height map of selected chunks",
                    v => HeightMap = true },
            };

            _chunkFilter = new ChunkFilter();
        }

        public RelightOptions (string[] args)
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
            Console.WriteLine("Usage: nbtoolkit relight [options]");
            Console.WriteLine();
            Console.WriteLine("Options for command 'relight':");

            _filterOpt.WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            _chunkFilter.PrintUsage();

            Console.WriteLine();
            base.PrintUsage();
        }

        public override void SetDefaults ()
        {
            base.SetDefaults();
        }

        public IChunkFilter GetChunkFilter ()
        {
            return _chunkFilter;
        }
    }

    public class Relight : TKFilter
    {
        private RelightOptions opt;

        public Relight (RelightOptions o)
        {
            opt = o;
        }

        public override void Run ()
        {
            NbtWorld world = GetWorld(opt);
            IChunkManager cm = world.GetChunkManager(opt.OPT_DIM);
            FilteredChunkManager fcm = new FilteredChunkManager(cm, opt.GetChunkFilter());

            if (opt.OPT_V) {
                Console.WriteLine("Clearing existing chunk lighting...");
            }

            int affectedChunks = 0;

            foreach (ChunkRef chunk in fcm) {
                if (opt.OPT_VV) {
                    Console.WriteLine("Resetting chunk {0},{1}...", chunk.X, chunk.Z);
                }

                if (opt.HeightMap) {
                    chunk.Blocks.RebuildHeightMap();
                }
                if (opt.BlockLight) {
                    chunk.Blocks.ResetBlockLight();
                }
                if (opt.SkyLight) {
                    chunk.Blocks.ResetSkyLight();
                }
                fcm.Save();

                affectedChunks++;
            }

            if (opt.OPT_V) {
                Console.WriteLine("Rebuilding chunk lighting...");
            }

            foreach (ChunkRef chunk in fcm) {
                if (opt.OPT_VV) {
                    Console.WriteLine("Lighting chunk {0},{1}...", chunk.X, chunk.Z);
                }

                if (opt.BlockLight) {
                    chunk.Blocks.RebuildBlockLight();
                }
                if (opt.SkyLight) {
                    chunk.Blocks.RebuildSkyLight();
                }
                fcm.Save();
            }

            if (opt.OPT_V) {
                Console.WriteLine("Reconciling chunk edges...");
            }

            foreach (ChunkRef chunk in fcm) {
                if (opt.OPT_VV) {
                    Console.WriteLine("Stitching chunk {0},{1}...", chunk.X, chunk.Z);
                }

                if (opt.BlockLight) {
                    chunk.Blocks.StitchBlockLight();
                }
                if (opt.SkyLight) {
                    chunk.Blocks.StitchSkyLight();
                }
                fcm.Save();
            }

            Console.WriteLine("Relit Chunks: " + affectedChunks);
        }
    }
}
