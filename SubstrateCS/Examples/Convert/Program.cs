using System;
using Substrate;
using Substrate.Core;
using Substrate.Nbt;
using System.IO;

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
                Console.WriteLine("Usage: Convert <world> <dest> <alpha|beta|anvil>");
                return;
            }

            string src = args[0];
            string dst = args[1];
            string srctype = args[2];

            if (!Directory.Exists(dst))
                Directory.CreateDirectory(dst);

            // Open source and destrination worlds depending on conversion type
            NbtWorld srcWorld = NbtWorld.Open(src);
            NbtWorld dstWorld;
            switch (srctype) {
                case "alpha": dstWorld = AlphaWorld.Create(dst); break;
                case "beta": dstWorld = BetaWorld.Create(dst); break;
                case "anvil": dstWorld = AnvilWorld.Create(dst); break;
                default: throw new Exception("Invalid conversion type");
            }

            // Grab chunk managers to copy chunks
            IChunkManager cmsrc = srcWorld.GetChunkManager();
            IChunkManager cmdst = dstWorld.GetChunkManager();

            // Copy each chunk from source to dest
            foreach (ChunkRef chunk in cmsrc) {
                cmdst.SetChunk(chunk.X, chunk.Z, chunk.GetChunkRef());
                Console.WriteLine("Copying chunk: {0}, {1}", chunk.X, chunk.Z);
            }

            // Copy level data from source to dest
            dstWorld.Level.LoadTreeSafe(srcWorld.Level.BuildTree());

            // Save level.dat
            dstWorld.Level.Save();
        }
    }
}
