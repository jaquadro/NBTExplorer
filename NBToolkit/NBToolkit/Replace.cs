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
                { "bxa=", "Update blocks with X-coord equal to or above {VAL}",
                    v => BL_X_GE = Convert.ToInt32(v) },
                { "bxb=", "Update blocks with X-coord equal to or below {VAL}",
                    v => BL_X_LE = Convert.ToInt32(v) },
                { "bxr=", "Update blocks with X-coord between {0:V1} and {1:V2} [inclusive]",
                    (v1, v2) => { BL_X_GE = Convert.ToInt32(v1); BL_X_LE = Convert.ToInt32(v2); } },
                { "bya=", "Update blocks with Y-coord equal to or above {VAL}",
                    v => BL_Y_GE = Convert.ToInt32(v) },
                { "byb=", "Update blocks with Y-coord equal to or below {VAL}",
                    v => BL_Y_LE = Convert.ToInt32(v) },
                { "byr=", "Update blocks with Y-coord between {0:V1} and {1:V2} [inclusive]",
                    (v1, v2) => { BL_Y_GE = Convert.ToInt32(v1); BL_Y_LE = Convert.ToInt32(v2); } },
                { "bza=", "Update blocks with Z-coord equal to or above {VAL}",
                    v => BL_Z_GE = Convert.ToInt32(v) },
                { "bzb=", "Update blocks with Z-coord equal to or below {VAL}",
                    v => BL_Z_LE = Convert.ToInt32(v) },
                { "bzr=", "Update blocks with Z-coord between {0:V1} and {1:V2} [inclusive]",
                    (v1, v2) => { BL_Z_GE = Convert.ToInt32(v1); BL_Z_LE = Convert.ToInt32(v2); } },
                { "nb=", "Update blocks that have block type {ID} as any neighbor",
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
                    v => OPT_NEIGHBOR_E = Convert.ToInt32(v) % 256 },
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

            NBT_Tag xPos = level.findTagByName("xPos");
            NBT_Tag zPos = level.findTagByName("zPos");
            if (xPos == null || zPos == null || xPos.type != NBT_Type.TAG_INT || zPos.type != NBT_Type.TAG_INT) {
                return;
            }

            int xBase = xPos.value.toInt().data * 16;
            int zBase = zPos.value.toInt().data * 16;

            NBT_ByteArray blocks_ary = blocks.value.toByteArray();
            NBT_ByteArray data_ary = data.value.toByteArray();

            // Determine X range
            int xmin = 0;
            int xmax = 15;

            if (opt.BL_X_GE != null) {
                xmin = (int)opt.BL_X_GE - xBase;
            }
            if (opt.BL_X_LE != null) {
                xmax = (int)opt.BL_X_LE - xBase;
            }

            xmin = (xmin < 0) ? 0 : xmin;
            xmax = (xmin > 15) ? 15 : xmax;

            if (xmin > 15 || xmax < 0 || xmin > xmax) {
                return;
            }

            // Determine Y range
            int ymin = 0;
            int ymax = 127;

            if (opt.BL_Y_GE != null) {
                ymin = (int)opt.BL_Y_GE;
            }
            if (opt.BL_Y_LE != null) {
                ymax = (int)opt.BL_Y_LE;
            }

            if (ymin > ymax) {
                return;
            }

            // Determine X range
            int zmin = 0;
            int zmax = 15;

            if (opt.BL_Z_GE != null) {
                zmin = (int)opt.BL_Z_GE - zBase;
            }
            if (opt.BL_Z_LE != null) {
                zmax = (int)opt.BL_Z_LE - zBase;
            }

            zmin = (zmin < 0) ? 0 : zmin;
            zmax = (zmin > 15) ? 15 : zmax;

            if (zmin > 15 || zmax < 0 || zmin > zmax) {
                return;
            }

            // Process Chunk
            for (int y = ymin; y <= ymax; y++) {
                for (int x = xmin; x <= xmax; x++) {
                    for (int z = zmin; z <= zmax; z++) {
                        // Probability test
                        if (opt.OPT_PROB != null) {
                            double c = rand.NextDouble();
                            if (c > opt.OPT_PROB) {
                                continue;
                            }
                        }

                        // Attempt to replace block
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
