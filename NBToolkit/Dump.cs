using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NDesk.Options;
using Substrate;
using Substrate.NBT;

namespace NBToolkit
{
    public class DumpOptions : TKOptions, IChunkFilterable
    {
        private OptionSet _filterOpt = null;
        private ChunkFilter _chunkFilter = null;

        public string _outFile = "";

        public bool _dumpBlocks = false;
        public bool _dumpEntities = false;
        public bool _dumpTileEntities = false;

        public DumpOptions ()
            : base()
        {
            _filterOpt = new OptionSet()
            {
                { "out|OutFile=", "Path of file to write JSON data into",
                    var => _outFile = var },
                { "b|BlockData", "Dump block data (type, data, light arrays)",
                    var => _dumpBlocks = true },
                { "e|Entities", "Dump complete entity data",
                    var => _dumpEntities = true },
                { "t|TileEntities", "Dump complete tile entity data",
                    var => _dumpTileEntities = true },
            };

            _chunkFilter = new ChunkFilter();
        }

        public DumpOptions (string[] args)
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
            Console.WriteLine("Usage: nbtoolkit dump [options]");
            Console.WriteLine();
            Console.WriteLine("Options for command 'dump':");

            _filterOpt.WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            _chunkFilter.PrintUsage();

            Console.WriteLine();
            base.PrintUsage();
        }

        public override void SetDefaults ()
        {
            base.SetDefaults();

            if (_outFile.Length == 0) {
                Console.WriteLine("Error: You must specify an output file");
                Console.WriteLine();
                this.PrintUsage();

                throw new TKOptionException();
            }
        }

        public IChunkFilter GetChunkFilter ()
        {
            return _chunkFilter;
        }
    }

    class Dump : TKFilter
    {
        private DumpOptions opt;

        public Dump (DumpOptions o)
        {
            opt = o;
        }

        public override void Run ()
        {
            World world = new World(opt.OPT_WORLD);
            ChunkManager cm = world.GetChunkManager() as ChunkManager;

            StreamWriter fstr;
            try {
                fstr = new StreamWriter(opt._outFile, false);
            }
            catch (IOException e) {
                Console.WriteLine(e.Message);
                return;
            }

            fstr.WriteLine("[");

            bool first = true;
            foreach (ChunkRef chunk in new FilteredChunkList(cm, opt.GetChunkFilter())) {
                if (!first) {
                    fstr.Write(",");
                }

                Chunk c = chunk.GetChunkRef();

                if (!opt._dumpBlocks) {
                    c.Tree.Root["Level"].ToNBTCompound().Remove("Blocks");
                    c.Tree.Root["Level"].ToNBTCompound().Remove("Data");
                    c.Tree.Root["Level"].ToNBTCompound().Remove("BlockLight");
                    c.Tree.Root["Level"].ToNBTCompound().Remove("SkyLight");
                    c.Tree.Root["Level"].ToNBTCompound().Remove("HeightMap");
                }

                if (!opt._dumpEntities) {
                    c.Tree.Root["Level"].ToNBTCompound().Remove("Entities");
                }

                if (!opt._dumpTileEntities) {
                    c.Tree.Root["Level"].ToNBTCompound().Remove("TileEntities");
                }

                string s = JSONSerializer.Serialize(c.Tree.Root["Level"], 1);
                fstr.Write(s);

                first = false;
            }

            fstr.WriteLine();
            fstr.WriteLine("]");

            fstr.Close();
        }
    }
}
