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
            FileName = Path.Combine(path, file);
        }

        public static string NameFromFilename (string filename)
        {
            if (filename.EndsWith(".dat")) {
                return filename.Remove(filename.Length - 4);
            }

            return filename;
        }
    }
}
