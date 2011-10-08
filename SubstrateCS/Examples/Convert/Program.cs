using System;
using Substrate;
using Substrate.Core;
using Substrate.Nbt;

// This example will convert worlds between alpha and beta format.
// This will convert chunks to and from region format, and copy level.dat
// Other data, like players and other dims, will not be handled.

namespace Convert
{
    class Program
    {
        static void Main (string[] args)
        {
            if (args.Length != 3) {
                Console.WriteLine("Usage: Convert <world> <dest> <a|b>");
                return;
            }

            string src = args[0];
            string dst = args[1];
            string srctype = args[2];

            // Open source and destrination worlds depending on conversion type
            NbtWorld srcWorld;
            NbtWorld dstWorld;
            if (srctype == "a") {
                srcWorld = AlphaWorld.Open(src);
                dstWorld = BetaWorld.Create(dst);
            }
            else {
                srcWorld = BetaWorld.Open(src);
                dstWorld = AlphaWorld.Create(dst);
            }

            // Grab chunk managers to copy chunks
            IChunkManager cmsrc = srcWorld.GetChunkManager();
            IChunkManager cmdst = dstWorld.GetChunkManager();

            // Copy each chunk from source to dest
            foreach (ChunkRef chunk in cmsrc) {
                cmdst.SetChunk(chunk.X, chunk.Z, chunk.GetChunkRef());
            }

            // Copy level data from source to dest
            dstWorld.Level.LoadTreeSafe(srcWorld.Level.BuildTree());

            // If we're creating an alpha world, get rid of the version field
            if (srctype == "b") {
                dstWorld.Level.Version = 0;
            }

            // Save level.dat
            dstWorld.Level.Save();
        }
    }
}
