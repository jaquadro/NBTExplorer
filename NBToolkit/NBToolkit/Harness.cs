using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NBT;

namespace NBToolkit
{
    class Harness
    {
        public void Run (TKOptions opt, TKFilter filter) {
            World world = new World(opt.OPT_WORLD);
            RegionList regions = new RegionList(world.GetRegionManager());

            foreach (Region region in regions) {
                Console.WriteLine(region.GetFileName());

                for (int x = 0; x < ChunkManager.REGION_XLEN; x++) {
                    for (int z = 0; z < ChunkManager.REGION_ZLEN; z++) {
                        if (!region.ChunkExists(x, z)) {
                            continue;
                        }
                    }
                }
            }
            
            /*string[] regions = RegionFileCache.GetRegionFileList(opt.OPT_WORLD);

            foreach (string p in regions) {
                Console.WriteLine(p);
                RegionFile rfile = RegionFileCache.GetRegionFile(opt.OPT_WORLD, p);

                for (int x = 0; x < 32; x++) {
                    for (int z = 0; z < 32; z++) {
                        NBT_Tree tree = new NBT_Tree(rfile.GetChunkDataInputStream(x, z));

                        // Check that tree exists
                        if (tree == null || tree.getRoot() == null) {
                            continue;
                        }

                        NBT_Tag level = tree.getRoot().findTagByName("Level");
                        if (level == null) {
                            continue;
                        }

                        NBT_Tag tagcx = level.findTagByName("xPos");
                        NBT_Tag tagcz = level.findTagByName("zPos");

                        if (tagcx == null || tagcz == null) {
                            continue;
                        }

                        int cx = tagcx.value.toInt().data;
                        int cz = tagcz.value.toInt().data;

                        // Exclude chunks out of range
                        if (opt.CH_X_GE != null && cx < opt.CH_X_GE) {
                            continue;
                        }
                        if (opt.CH_X_LE != null && cx > opt.CH_X_LE) {
                            continue;
                        }
                        if (opt.CH_Z_GE != null && cz < opt.CH_Z_GE) {
                            continue;
                        }
                        if (opt.CH_Z_LE != null && cz > opt.CH_Z_LE) {
                            continue;
                        }

                        // Verify that chunk contains all of the INCLUDE options
                        bool fail = false;
                        foreach (int v in opt.OPT_CH_INCLUDE) {
                            if (!BlockScan(level, v)) {
                                fail = true;
                                break;
                            }
                        }

                        // Verify that chunk does not contain any EXCLUDE options
                        foreach (int v in opt.OPT_CH_EXCLUDE) {
                            if (BlockScan(level, v)) {
                                fail = true;
                                break;
                            }
                        }

                        if (fail) {
                            continue;
                        }

                        if (opt.OPT_V) {
                            Console.WriteLine("Processing Chunk ({0}, {1})", cx, cz);
                        }

                        filter.ApplyChunk(tree);

                        Stream zipStream = RegionFileCache.getChunkDataOutputStream(opt.OPT_WORLD, cx, cz);
                        tree.WriteTo(zipStream);
                        zipStream.Close();
                    }
                }
            }*/
        }

        protected bool BlockScan (NBT_Tag level, int val)
        {
            NBT_Tag blocks = level.findTagByName("Blocks");
            if (blocks == null || blocks.type != NBT_Type.TAG_BYTE_ARRAY) {
                return false;
            }

            NBT_ByteArray blocks_ary = blocks.value.toByteArray();

            for (int x = 0; x < 16; x++) {
                for (int z = 0; z < 16; z++) {
                    for (int y = 0; y < 128; y++) {
                        int index = BlockIndex(x, y, z);
                        if (blocks_ary.data[index] == val) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        int BlockIndex (int x, int y, int z)
        {
            return y + (z * 128 + x * 128 * 16);
        }
    }
}
