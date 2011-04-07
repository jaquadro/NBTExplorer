using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zlib;

namespace Substrate
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
            string cx64 = Base64(cx);
            string cz64 = Base64(cz);
            string file = "c." + cx64 + "." + cz64 + ".dat";

            string dir1 = Base64(cx % 64);
            string dir2 = Base64(cz % 64);

            _filename = Path.Combine(path, dir1);
            _filename = Path.Combine(_filename, dir2);
            _filename = Path.Combine(_filename, file);
        }

        private string Base64 (int val)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(val.ToString()));
        }
    }
}
