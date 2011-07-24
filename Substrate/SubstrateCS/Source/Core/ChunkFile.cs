using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zlib;

namespace Substrate.Core
{
    public class ChunkFile : NBTFile
    {
        public ChunkFile (string path)
            : base(path)
        {
        }

        public ChunkFile (string path, int cx, int cz)
            : base("")
        {
            string cx64 = Base36.Encode(cx);
            string cz64 = Base36.Encode(cz);
            string file = "c." + cx64 + "." + cz64 + ".dat";

            while (cx < 0) {
                cx += (64 * 64);
            }
            while (cz < 0) {
                cz += (64 * 64);
            }

            string dir1 = Base36.Encode(cx % 64);
            string dir2 = Base36.Encode(cz % 64);

            FileName = Path.Combine(path, dir1);
            if (!Directory.Exists(FileName)) {
                Directory.CreateDirectory(FileName);
            }

            FileName = Path.Combine(FileName, dir2);
            if (!Directory.Exists(FileName)) {
                Directory.CreateDirectory(FileName);
            }

            FileName = Path.Combine(FileName, file);
        }
    }
}
