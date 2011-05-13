using System;
using Substrate;

// FlatMap is an example of generating worlds from scratch with Substrate.
// It will produce a completely flat, solid map with grass, dirt, stone,
// and bedrock layers.  On a powerful workstation, creating 400 of these
// chunks only takes a few seconds.

namespace FlatMap
{
    class Program
    {
        static void Main (string[] args)
        {
            string dest = "F:\\Minecraft\\test";
            int xmin = -20;
            int xmax = 20;
            int zmin = -20;
            int zmaz = 20;

            // This will instantly create any necessary directory structure
            BetaWorld world = BetaWorld.Create(dest);
            ChunkManager cm = world.GetChunkManager();

            // We can set different world parameters
            world.Level.LevelName = "Flatlands";
            world.Level.SetDefaultPlayer();

            // We'll create chunks at chunk coordinates xmin,zmin to xmax,zmax
            for (int xi = xmin; xi < xmax; xi++) {
                for (int zi = zmin; zi < zmaz; zi++) {
                    // This line will create a default empty chunk, and create a
                    // backing region file if necessary (which will immediately be
                    // written to disk)
                    ChunkRef chunk = cm.CreateChunk(xi, zi);

                    // This will suppress generating caves, ores, and all those
                    // other goodies.
                    chunk.IsTerrainPopulated = true;

                    // Auto light recalculation is horrifically bad for creating
                    // chunks from scratch, because we're placing thousands
                    // of blocks.  Turn it off.
                    chunk.Blocks.AutoLight = false;

                    // Set the blocks
                    FlatChunk(chunk, 64);

                    // Reset and rebuild the lighting for the entire chunk at once
                    chunk.Blocks.RebuildBlockLight();
                    chunk.Blocks.RebuildSkyLight();

                    Console.WriteLine("Built Chunk {0},{1}", chunk.X, chunk.Z);

                    // Save the chunk to disk so it doesn't hang around in RAM
                    cm.Save();
                }
            }

            // Save all remaining data (including a default level.dat)
            // If we didn't save chunks earlier, they would be saved here
            world.Save();
        }

        static void FlatChunk (ChunkRef chunk, int height)
        {
            // Create bedrock
            for (int y = 0; y < 2; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        chunk.Blocks.SetID(x, y, z, (int)BlockType.BEDROCK);
                    }
                }
            }

            // Create stone
            for (int y = 2; y < height - 5; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        chunk.Blocks.SetID(x, y, z, (int)BlockType.STONE);
                    }
                }
            }

            // Create dirt
            for (int y = height - 5; y < height - 1; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        chunk.Blocks.SetID(x, y, z, (int)BlockType.DIRT);
                    }
                }
            }

            // Create grass
            for (int y = height - 1; y < height; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        chunk.Blocks.SetID(x, y, z, (int)BlockType.GRASS);
                    }
                }
            }
        }
    }
}
