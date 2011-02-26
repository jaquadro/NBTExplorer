using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using NBT;

namespace NBToolkit
{
    public class ReplaceOptions : TKOptions
    {
        private OptionSet filterOpt = null;

        public int? OPT_BEFORE = null;
        public int? OPT_AFTER = null;

        public int? OPT_DATA = null;
        public double? OPT_PROB = null;

        public ReplaceOptions (string[] args) : base(args)
        {
            filterOpt = new OptionSet()
            {
                { "b|before=", "Replace instances of block type {ID} with another block type",
                    v => OPT_BEFORE = Convert.ToInt32(v) % 256 },
                { "a|after=", "Replace the selected blocks with block type {ID}",
                    v => OPT_AFTER = Convert.ToInt32(v) % 256 },
                { "d|data=", "Set the new block's data value to {VAL} (0-15)",
                    v => OPT_DATA = Convert.ToInt32(v) % 16 },
                { "p|prob=", "Replace any matching block with probability {VAL} (0.0-1.0)",
                    v => { OPT_PROB = Convert.ToDouble(v); 
                           OPT_PROB = Math.Max((double)OPT_PROB, 0.0); 
                           OPT_PROB = Math.Min((double)OPT_PROB, 1.0); } },
            };

            filterOpt.Parse(args);
        }

        public override void PrintUsage ()
        {
            Console.WriteLine("Usage: nbtoolkit replace -b <id> -a <id> [options]");
            Console.WriteLine();
            Console.WriteLine("Options for command 'replace':");

            filterOpt.WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            base.PrintUsage();
        }

        public override void SetDefaults ()
        {
            base.SetDefaults();

            // Check for required parameters
            if (OPT_BEFORE == null || OPT_AFTER == null) {
                Console.WriteLine("Error: You must specify a before and after Block ID");
                Console.WriteLine();
                PrintUsage();

                throw new TKOptionException();
            }
        }
    }

    class Replace : TKFilter
    {
        private ReplaceOptions opt;

        private static Random rand = new Random();

        public Replace (ReplaceOptions o)
        {
            opt = o;
        }

        public override void ApplyChunk (NBT_Tree root)
        {
            if (root == null || root.getRoot() == null) {
                return;
            }

            NBT_Tag level = root.getRoot().findTagByName("Level");
            if (level == null || level.type != NBT_Type.TAG_COMPOUND) {
                return;
            }

            NBT_Tag blocks = level.findTagByName("Blocks");
            if (blocks == null || blocks.type != NBT_Type.TAG_BYTE_ARRAY) {
                return;
            }

            NBT_Tag data = level.findTagByName("Data");
            if (data == null || data.type != NBT_Type.TAG_BYTE_ARRAY) {
                return;
            }

            NBT_ByteArray blocks_ary = blocks.value.toByteArray();
            NBT_ByteArray data_ary = data.value.toByteArray();

            int ymin = 0;
            int ymax = 127;

            for (int y = ymin; y <= ymax; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        if (opt.OPT_PROB != null) {
                            double c = rand.NextDouble();
                            if (c > opt.OPT_PROB) {
                                continue;
                            }
                        }

                        int index = BlockIndex(x, y, z);
                        if (blocks_ary.data[index] == opt.OPT_BEFORE) {
                            blocks_ary.data[index] = (byte)opt.OPT_AFTER;

                            if (opt.OPT_VV) {
                                Console.WriteLine("Replaced block at {0},{1},{2}", x, y, z);
                            }

                            if (opt.OPT_DATA != null) {
                                if (index % 2 == 0) {
                                    data_ary.data[index / 2] = (byte)((data_ary.data[index / 2] & 0xF0) | (int)opt.OPT_DATA);
                                }
                                else {
                                    data_ary.data[index / 2] = (byte)((data_ary.data[index / 2] & 0x0F) | ((int)opt.OPT_DATA << 4));
                                }
                            }
                        }
                    }
                }
            }
        }

        int BlockIndex (int x, int y, int z)
        {
            return y + (z * 128 + x * 128 * 16);
        }
    }
}
