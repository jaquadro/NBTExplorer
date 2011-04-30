using System;
using System.Collections.Generic;
using Substrate;

namespace PurgeEntities
{
    class Program
    {
        static void Main (string[] args)
        {
            // Process arguments
            if (args.Length != 2 && args.Length != 6) {
                Console.WriteLine("Usage: PurgeEntities <world> <entityID> [<x1> <z1> <x2> <z2>]");
                return;
            }
            string dest = args[0];
            string eid = args[1];

            int x1 = BlockManager.MIN_X;
            int x2 = BlockManager.MAX_X;
            int z1 = BlockManager.MIN_Z;
            int z2 = BlockManager.MAX_Z;

            if (args.Length == 6) {
                x1 = Convert.ToInt32(args[2]);
                z1 = Convert.ToInt32(args[3]);
                x2 = Convert.ToInt32(args[4]);
                z2 = Convert.ToInt32(args[5]);
            }

            // Load world
            BetaWorld world = BetaWorld.Open(dest);
            ChunkManager cm = world.GetChunkManager();

            // Remove entities
            foreach (ChunkRef chunk in cm) {
                // Skip chunks that don't cover our selected area
                if (((chunk.X + 1) * chunk.XDim < x1) ||
                    (chunk.X * chunk.XDim >= x2) ||
                    ((chunk.Z + 1) * chunk.ZDim < z1) ||
                    (chunk.Z * chunk.ZDim >= z2)) {
                    continue;
                }

                // Delete the specified entities
                chunk.RemoveEntities(eid);
                cm.Save();
            }
        }
    }
}
