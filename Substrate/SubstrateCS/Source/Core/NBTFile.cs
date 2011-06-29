using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zlib;
using Substrate.NBT;

namespace Substrate.Core
{
    public class NBTFile
    {
        protected string _filename;

        public NBTFile (string path)
        {
            _filename = path;
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

        public virtual Stream GetDataInputStream ()
        {
            FileStream fstr = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            long length = fstr.Seek(0, SeekOrigin.End);
            fstr.Seek(0, SeekOrigin.Begin);

            byte[] data = new byte[length];
            fstr.Read(data, 0, data.Length);

            fstr.Close();

            return new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
        }

        public virtual Stream GetDataOutputStream ()
        {
            return new GZipStream(new NBTBuffer(this), CompressionMode.Compress);
        }

        class NBTBuffer : MemoryStream
        {
            private NBTFile file;

            public NBTBuffer (NBTFile c)
                : base(8096)
            {
                this.file = c;
            }

            public override void Close ()
            {
                FileStream fstr = new FileStream(file._filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                fstr.Write(this.GetBuffer(), 0, (int)this.Length);
                fstr.Close();
            }
        }

    }
}
