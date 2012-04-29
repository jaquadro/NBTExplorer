using System;
using Substrate;
using Substrate.Core;

// This example demonstrates adding explicit recognization of custom block types
// to your program (in this case, the LightSensor block form Risugami).  This
// program will scan a world and print out each instance of a LightSensor block
// that is found.

// The real usefulness in explicitly registering custom blocks that appear in
// your world is that you can properly support algorithms that depend on block's
// innate properties such as state of matter, opacity, and luminance.  This data
// lets the Substrate lighting and fluid calculations behave as expected.

namespace CustomBlocks
{
    class Program
    {
        static void Main (string[] args)
        {
            if (args.Length != 1) {
                Console.WriteLine("Usage: CustomBlock <world>");
                return;
            }

            string dest = args[0];

            // Open our world
            NbtWorld world = NbtWorld.Open(dest);

            // The chunk manager is more efficient than the block manager for
            // this purpose, since we'll inspect every block
            IChunkManager cm = world.GetChunkManager();

            foreach (ChunkRef chunk in cm) {
                // You could hardcode your dimensions, but maybe some day they
                // won't always be 16.  Also the CLR is a bit stupid and has
                // trouble optimizing repeated calls to Chunk.Blocks.xx, so we
                // cache them in locals
                int xdim = chunk.Blocks.XDim;
                int ydim = chunk.Blocks.YDim;
                int zdim = chunk.Blocks.ZDim;

                chunk.Blocks.AutoFluid = true;

                // x, z, y is the most efficient order to scan blocks (not that
                // you should care about internal detail)
                for (int x = 0; x < xdim; x++) {
                    for (int z = 0; z < zdim; z++) {
                        for (int y = 0; y < ydim; y++) {
                            BlockInfo info = chunk.Blocks.GetInfo(x, y, z);
                            if (info.ID == BlockInfoM.LightSensor.ID) {
                                Console.WriteLine("Found custom block '{0}' at {1}", info.Name, new BlockKey(x, y, z));
                            }
                        }
                    }
                }
            }
        }
    }

    // Convenience class -- like the BlockType class, it's not required that you define this
    static class BlockTypeM
    {
        public static int LIGHT_SENSOR = 131;
    }

    // A place to store a global instance of a LightSensor block, for our convenience.
    static class BlockInfoM
    {
        public static BlockInfo LightSensor;

        static BlockInfoM ()
        {
            // Creating a BlockInfo (or BlockInfoEx) will also automatically register the
            // block ID, lighting, and opacity data with internal tables in the BlockInfo
            // static class, making them available to GetInfo() calls.
            LightSensor = new BlockInfo(BlockTypeM.LIGHT_SENSOR, "Light Sensor").SetOpacity(0).SetState(BlockState.NONSOLID);

            // You can redefine already-registered blocks at any time by creating a new
            // BlockInfo object with the given ID.
        }
    }
}
