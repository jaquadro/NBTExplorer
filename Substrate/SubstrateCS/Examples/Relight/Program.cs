using System;
using Substrate;
using Substrate.Core;

// This example will reset and rebuild the lighting (heightmap, block light,
// skylight) for all chunks in a map.

// Note: If it looks silly to reset the lighting, loading and saving
// all the chunks, just to load and save them again later: it's not.
// If the world lighting is not correct, it must be completely reset
// before rebuilding the light in any chunks.  That's just how the
// algorithms work, in order to limit the number of chunks that must
// be loaded at any given time.

namespace Relight
{
    class Program
    {
        static void Main (string[] args)
        {
            if (args.Length < 1) {
                Console.WriteLine("You must specify a target directory");
                return;
            }
            string dest = args[0];

            // Load the world, supporting either alpha or beta format
            /*INBTWorld world;
            if (args.Length >= 2 && args[1] == "alpha") {
                world = AlphaWorld.Open(dest);
            }
            else {
                world = BetaWorld.Open(dest);
            }*/

            NbtWorld world = NbtWorld.Open(dest);

            // Grab a generic chunk manager reference
            IChunkManager cm = world.GetChunkManager();

            // First blank out all of the lighting in all of the chunks
            foreach (ChunkRef chunk in cm) {
                chunk.Blocks.RebuildHeightMap();
                chunk.Blocks.ResetBlockLight();
                chunk.Blocks.ResetSkyLight();

                cm.Save();

                Console.WriteLine("Reset Chunk {0},{1}", chunk.X, chunk.Z);
            }

            // In a separate pass, reconstruct the light
            foreach (ChunkRef chunk in cm) {
                chunk.Blocks.RebuildBlockLight();
                chunk.Blocks.RebuildSkyLight();

                // Save the chunk to disk so it doesn't hang around in RAM
                cm.Save();

                Console.WriteLine("Lit Chunk {0},{1}", chunk.X, chunk.Z);
            }
        }
    }
}
