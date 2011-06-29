using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zlib;

namespace Substrate.Core
{
    public class PlayerFile : NBTFile
    {
        public PlayerFile (string path)
            : base(path)
        {
        }

        public PlayerFile (string path, string name)
            : base("")
        {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            string file = name + ".dat";
            _filename = Path.Combine(path, file);
        }
    }
}
