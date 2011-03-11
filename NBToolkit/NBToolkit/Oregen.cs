using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using NBT;

namespace NBToolkit
{
    public class OregenOptions : TKOptions
    {
        private OptionSet filterOpt = null;

        public int? OPT_ID = null;

        public int? OPT_DATA = null;

        public int? OPT_ROUNDS = null;
        public int? OPT_SIZE = null;
        public int? OPT_MIN = null;
        public int? OPT_MAX = null;

        public bool OPT_OO = false;
        public bool OPT_OA = false;
        
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

        public OregenOptions (string[] args) : base(args)
        {
            filterOpt = new OptionSet()
            {
                { "b|block=", "Generate blocks of type {ID} (0-255)",
                    v => OPT_ID = Convert.ToByte(v) % 256 },
                { "d|data=", "Set the block's data value to {VAL} (0-15)",
                    v => OPT_DATA = Convert.ToInt32(v) % 16 },
                { "r|rounds=", "Geneate {NUM} deposits per chunk",
                    v => OPT_ROUNDS = Convert.ToInt32(v) },
                { "min=", "Generates deposits no lower than depth {VAL} (0-127)",
                    v => OPT_MIN = Convert.ToInt32(v) % 128 },
                { "max=", "Generates deposits no higher than depth {VAL} (0-127)",
                    v => OPT_MAX = Convert.ToInt32(v) % 128 },
                { "s|size=", "Generates deposits containing roughly up to {VAL} blocks",
                    v => OPT_SIZE = Convert.ToInt32(v) % 128 },
                { "oo=", "Generated deposits can replace other existing ores",
                    v => OPT_OO = true },
                { "oa=", "Generated deposits can replace any existing block (incl. air)",
                    v => OPT_OA = true },
                { "oi=", "Generated deposits can replace the specified block type [repeatable]",
                    v => OPT_OB_INCLUDE.Add(Convert.ToInt32(v) % 256) },
                { "ox=", "Generated deposits can never replace the specified block type [repeatable]",
                    v => OPT_OB_EXCLUDE.Add(Convert.ToInt32(v) % 256) },
            };

            filterOpt.Parse(args);
        }

        public override void PrintUsage ()
        {
            Console.WriteLine("Usage: nbtoolkit oregen -b <id> -w <path> [options]");
            Console.WriteLine();
            Console.WriteLine("Options for command 'oregen':");

            filterOpt.WriteOptionDescriptions(Console.Out);

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
    }

    public class MathHelper
    {
        private static float[] trigTable = new float[65536];

        static MathHelper ()
        {
            for (int i = 0; i < 65536; i++) {
                trigTable[i] = (float)Math.Sin(i * Math.PI * 2.0D / 65536.0D);
            }
        }

        public static float Sin(float angle)
        {
          return trigTable[((int)(angle * 10430.378F) & 0xFFFF)];
        }

        public static float Cos(float angle) {
          return trigTable[((int)(angle * 10430.378F + 16384.0F) & 0xFFFF)];
        }
    }

    public class NativeOreGen {
        private int _blockId;
        private int _size;

        private static Random rand = new Random();

        public NativeOreGen(int blockId, int size)
        {
            _blockId = blockId;
            _size = size;
        }

        public bool GenerateDeposit (NBT_ByteArray blocks, NBT_ByteArray data, int x, int y, int z)
        {
            float rpi = (float)(rand.NextDouble() * Math.PI);

            double x1 = x + 8 + MathHelper.Sin(rpi) * _size / 8.0F;
            double x2 = x + 8 - MathHelper.Sin(rpi) * _size / 8.0F;
            double z1 = z + 8 + MathHelper.Cos(rpi) * _size / 8.0F;
            double z2 = z + 8 - MathHelper.Cos(rpi) * _size / 8.0F;

            double y1 = y + rand.Next(3) + 2;
            double y2 = y + rand.Next(3) + 2;

            for (int i = 0; i <= _size; i++) {
                double xPos = x1 + (x2 - x1) * i / _size;
                double yPos = y1 + (y2 - y1) * i / _size;
                double zPos = z1 + (z2 - z1) * i / _size;

                double fuzz = rand.NextDouble() * _size / 16.0D;
                double fuzzXZ = (MathHelper.Sin((float)(i * Math.PI / _size)) + 1.0F) * fuzz + 1.0D;
                double fuzzY = (MathHelper.Sin((float)(i * Math.PI / _size)) + 1.0F) * fuzz + 1.0D;

                int xStart = (int)(xPos - fuzzXZ / 2.0D);
                int yStart = (int)(yPos - fuzzY / 2.0D);
                int zStart = (int)(zPos - fuzzXZ / 2.0D);

                int xEnd = (int)(xPos + fuzzXZ / 2.0D);
                int yEnd = (int)(yPos + fuzzY / 2.0D);
                int zEnd = (int)(zPos + fuzzXZ / 2.0D);

                for (int ix = xStart; ix <= xEnd; ix++) {
                    double xThresh = (ix + 0.5D - xPos) / (fuzzXZ / 2.0D);
                    if (xThresh * xThresh < 1.0D) {
                        for (int iy = yStart; iy <= yEnd; iy++) {
                            double yThresh = (iy + 0.5D - yPos) / (fuzzY / 2.0D);
                            if (xThresh * xThresh + yThresh * yThresh < 1.0D) {
                                for (int iz = zStart; iz <= zEnd; iz++) {
                                    double zThresh = (iz + 0.5D - zPos) / (fuzzXZ / 2.0D);
                                    if (xThresh * xThresh + yThresh * yThresh + zThresh * zThresh < 1.0D) {
                                        //Apply
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        int BlockIndex (int x, int y, int z)
        {
            return y + (z * 128 + x * 128 * 16);
        }
    }

    public class Oregen : TKFilter
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

        private OregenOptions opt;

        private static Random rand = new Random();

        public Oregen (OregenOptions o)
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

            if (opt.OPT_V) {
                Console.WriteLine("Generating {0} size {1} deposits of {2} between {3} and {4}",
                    opt.OPT_ROUNDS, opt.OPT_SIZE, opt.OPT_ID, opt.OPT_MIN, opt.OPT_MAX);
            }

            for (int i = 0; i < opt.OPT_ROUNDS; i++) {
                if (opt.OPT_VV) {
                    Console.WriteLine("Generating round {0}...", i);
                }

                GenerateDeposit(blocks_ary, data_ary);
            }
        }

        protected void GenerateDeposit (NBT_ByteArray blocks, NBT_ByteArray data)
        {
            double x_scale = 0.25 + (rand.NextDouble() * 0.75);
            double y_scale = 0.25 + (rand.NextDouble() * 0.75);
            double z_scale = 0.25 + (rand.NextDouble() * 0.75);

            if (opt.OPT_VV) {
                Console.WriteLine("Selected scale: {0}, {1}, {2}", x_scale, y_scale, z_scale);
            }

            double x_len = (double)opt.OPT_SIZE / 8.0 * x_scale;
            double z_len = (double)opt.OPT_SIZE / 8.0 * z_scale;
            double y_len = ((double)opt.OPT_SIZE / 16.0 + 2.0) * y_scale;

            if (opt.OPT_VV) {
                Console.WriteLine("Selected length: {0}, {1}, {2}", x_len, y_len, z_len);
            }

            double xpos = rand.NextDouble() * (16.0 - x_len);
            double zpos = rand.NextDouble() * (16.0 - z_len);
            double ypos = (double)opt.OPT_MIN + (rand.NextDouble() * ((double)opt.OPT_MAX - (double)opt.OPT_MIN)) + 2.0;

            if (opt.OPT_VV) {
                Console.WriteLine("Selected initial position: {0}, {1}, {2}", xpos, ypos, zpos);
            }

            int sample_size = 2 * (int)opt.OPT_SIZE;
            double fuzz = 0.25;

            double x_step = x_len / sample_size;
            double y_step = y_len / sample_size;
            double z_step = z_len / sample_size;

            for (int i = 0; i < sample_size; i++) {
                int tx = (int)Math.Floor(xpos + i * x_step);
                int ty = (int)Math.Floor(ypos + i * y_step);
                int tz = (int)Math.Floor(zpos + i * z_step);
                int txp = (int)Math.Floor(xpos + i * x_step + fuzz);
                int typ = (int)Math.Floor(ypos + i * y_step + fuzz);
                int tzp = (int)Math.Floor(zpos + i * z_step + fuzz);

                if (tx < 0) tx = 0;
                if (ty < 0) ty = 0;
                if (tz < 0) tz = 0;

                if (tx >= 16) tx = 15;
                if (ty >= 128) ty = 127;
                if (tz >= 16) tz = 15;

                if (txp < 0) txp = 0;
                if (typ < 0) typ = 0;
                if (tzp < 0) tzp = 0;

                if (txp >= 16) txp = 15;
                if (typ >= 128) typ = 127;
                if (tzp >= 16) tzp = 15;

                UpdateBlock(blocks, data, tx, ty, tz);
                UpdateBlock(blocks, data, txp, ty, tz);
                UpdateBlock(blocks, data, tx, typ, tz);
                UpdateBlock(blocks, data, tx, ty, tzp);
                UpdateBlock(blocks, data, txp, typ, tz);
                UpdateBlock(blocks, data, tx, typ, tzp);
                UpdateBlock(blocks, data, txp, ty, tzp);
                UpdateBlock(blocks, data, txp, typ, tzp);
            }
        }

        protected void UpdateBlock (NBT_ByteArray blocks, NBT_ByteArray data, int x, int y, int z)
        {
            int index = BlockIndex(x, y, z);
	
            if (index < 0 || index >= 32768) {
                throw new Exception();
            }

	        if (
		        ((opt.OPT_OA) && (blocks.data[index] != opt.OPT_ID)) ||
		        ((opt.OPT_OO) && (
			        blocks.data[index] == BLOCK_COAL || blocks.data[index] == BLOCK_IRON ||
			        blocks.data[index] == BLOCK_GOLD || blocks.data[index] == BLOCK_REDSTONE ||
			        blocks.data[index] == BLOCK_DIAMOND || blocks.data[index] == BLOCK_LAPIS ||
			        blocks.data[index] == BLOCK_DIRT || blocks.data[index] == BLOCK_GRAVEL) && (blocks.data[index] != opt.OPT_ID)) ||
		        (opt.OPT_OB_INCLUDE.Count > 0) ||
		        (blocks.data[index] == BLOCK_STONE)
	        ) {
		        // If overriding list of ores, check membership
		        if (opt.OPT_OB_INCLUDE.Count > 0 && !opt.OPT_OB_INCLUDE.Contains(blocks.data[index])) {
                    return;
                }

                // Check for any excluded block
                if (opt.OPT_OB_EXCLUDE.Contains(blocks.data[index])) {
                    return;
                }

		        blocks.data[index] = (byte)opt.OPT_ID;
        		
		        if (opt.OPT_VV) {
			        Console.WriteLine("Added block at {0},{1},{2}", x, y, z);
		        }
        		
		        if (opt.OPT_DATA != null) {
			        if (index % 2 == 0) {
				        data.data[index / 2] = (byte)((data.data[index / 2] & 0xF0) | (int)opt.OPT_DATA);
			        }
			        else {
				        data.data[index / 2] = (byte)((data.data[index / 2] & 0x0F) | ((int)opt.OPT_DATA << 4));
			        }
		        }
	        }
        }

        int BlockIndex (int x, int y, int z) {
            return y + (z * 128 + x * 128 * 16);
        }
    }
}
