using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zlib;

namespace NBToolkit.Map
{
    class ChunkFile
    {
        private string _filename;

        public ChunkFile (string path)
        {
            _filename = path;
        }

        public ChunkFile (string path, int cx, int cz)
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

        public bool Exists ()
        {
            return File.Exists(_filename);
        }

        public bool Delete ()
        {
            File.Delete(_filename);
            return true;
        }

        public Stream GetChunkDataInputStream ()
        {
            FileStream fstr = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            long length = fstr.Seek(0, SeekOrigin.End);
            fstr.Seek(0, SeekOrigin.Begin);

            byte[] data = new byte[length];
            fstr.Read(data, 0, data.Length);

            fstr.Close();

            return new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
        }

        public Stream GetChunkDataOutputStream ()
        {
            return new ZlibStream(new ChunkBuffer(this), CompressionMode.Compress);
        }

        class ChunkBuffer : MemoryStream
        {
            private ChunkFile region;

            public ChunkBuffer (ChunkFile c)
                : base(8096)
            {
                this.region = c;
            }

            public override void Close ()
            {
                FileStream fstr = new FileStream(region._filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                fstr.Write(this.GetBuffer(), 0, (int)this.Length);
                fstr.Close();
            }
        }

    }
}
