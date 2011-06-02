using System;
using Substrate;

// This example replaces all instances of one block ID with another in a world.
// Substrate will handle all of the lower-level headaches that can pop up, such
// as maintaining correct lighting or replacing TileEntity records for blocks
// that need them.

// For a more advanced Block Replace example, see replace.cs in NBToolkit.

namespace BlockReplace
{
    class Program
    {
        static void Main (string[] args)
        {
            if (args.Length != 3) {
                Console.WriteLine("Usage: BlockReplace <world> <before-id> <after-id>");
                return;
            }

            string dest = args[0];
            int before = Convert.ToInt32(args[1]);
            int after = Convert.ToInt32(args[2]);

            // Open our world
            BetaWorld world = BetaWorld.Open(dest);

            // The chunk manager is more efficient than the block manager for
            // this purpose, since we'll inspect every block
            ChunkManager cm = world.GetChunkManager();

            foreach (ChunkRef chunk in cm) {
                // You could hardcode your dimensions, but maybe some day they
                // won't always be 16.  Also the CLR is a bit stupid and has
                // trouble optimizing repeated calls to Chunk.Blocks.xx, so we
                // cache them in locals
                int xdim = chunk.Blocks.XDim;
                int ydim = chunk.Blocks.YDim;
                int zdim = chunk.Blocks.ZDim;

                // x, z, y is the most efficient order to scan blocks (not that
                // you should care about internal detail)
                for (int x = 0; x < xdim; x++) {
                    for (int z = 0; z < zdim; z++) {
                        for (int y = 0; y < ydim; y++) {

                            // Replace the block with after if it matches before
                            if (chunk.Blocks.GetID(x, y, z) == before) {
                                chunk.Blocks.SetID(x, y, z, after);
                            }
                        }
                    }
                }

                // Save the chunk
                cm.Save();
            }
        }
    }
}
