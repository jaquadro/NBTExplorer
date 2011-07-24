using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zlib;
using Substrate.Nbt;

namespace Substrate.Core
{
    public class NBTFile
    {
        private string _filename;

        public NBTFile (string path)
        {
            _filename = path;
        }

        public string FileName
        {
            get { return _filename; }
            protected set { _filename = value; }
        }

        public bool Exists ()
        {
            return File.Exists(_filename);
        }

        public void Delete ()
        {
            File.Delete(_filename);
        }

        public virtual Stream GetDataInputStream ()
        {
            try {
                FileStream fstr = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                long length = fstr.Seek(0, SeekOrigin.End);
                fstr.Seek(0, SeekOrigin.Begin);

                byte[] data = new byte[length];
                fstr.Read(data, 0, data.Length);

                fstr.Close();

                return new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
            }
            catch (Exception ex) {
                throw new NbtIOException("Failed to open compressed NBT data stream for input.", ex);
            }
        }

        public virtual Stream GetDataOutputStream ()
        {
            try {
                return new GZipStream(new NBTBuffer(this), CompressionMode.Compress);
            }
            catch (Exception ex) {
                throw new NbtIOException("Failed to initialize compressed NBT data stream for output.", ex);
            }
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
                FileStream fstr;
                try {
                    fstr = new FileStream(file._filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
                catch (Exception ex) {
                    throw new NbtIOException("Failed to open NBT data stream for output.", ex);
                }

                try {
                    fstr.Write(this.GetBuffer(), 0, (int)this.Length);
                    fstr.Close();
                }
                catch (Exception ex) {
                    throw new NbtIOException("Failed to write out NBT data stream.", ex);
                }
            }
        }

    }
}
