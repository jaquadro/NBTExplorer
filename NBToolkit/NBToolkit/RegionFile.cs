using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zlib;
using System.Collections;

namespace NBToolkit
{
    public class RegionFile : IDisposable {

        private const int VERSION_GZIP = 1;
        private const int VERSION_DEFLATE = 2;

        private const int SECTOR_BYTES = 4096;
        private const int SECTOR_INTS = SECTOR_BYTES / 4;

        const int CHUNK_HEADER_SIZE = 5;

        private static byte[] emptySector = new byte[4096];

        private string fileName;
        private FileStream file;
        private int[] offsets;
        private int[] chunkTimestamps;
        private List<Boolean> sectorFree;
        private int sizeDelta;
        private long lastModified = 0;

        protected bool _disposed = false;

        public RegionFile(string path) {
            offsets = new int[SECTOR_INTS];
            chunkTimestamps = new int[SECTOR_INTS];

            fileName = path;
            Debugln("REGION LOAD " + fileName);

            sizeDelta = 0;

            ReadFile();
        }

        ~RegionFile ()
        {
            Dispose(false);
        }

        public void Dispose ()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (!_disposed) {
                if (disposing) {
                    // Cleanup managed resources
                }

                // Cleanup unmanaged resources
                if (file != null) {
                    file.Close();
                    file = null;
                }
            }
            _disposed = true;
        }

        protected void ReadFile ()
        {
            // Get last udpate time
            long newModified = 0;
            try {
                if (File.Exists(fileName)) {
                    newModified = Timestamp(File.GetLastWriteTime(fileName));
                }
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(e.Message);
                return;
            }

            // If it hasn't been modified, we don't need to do anything
            if (newModified == lastModified) {
                return;
            }

            try {
                file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                //using (file) {
                    if (file.Length < SECTOR_BYTES) {
                        byte[] int0 = BitConverter.GetBytes((int)0);

                        /* we need to write the chunk offset table */
                        for (int i = 0; i < SECTOR_INTS; ++i) {
                            file.Write(int0, 0, 4);
                        }
                        // write another sector for the timestamp info
                        for (int i = 0; i < SECTOR_INTS; ++i) {
                            file.Write(int0, 0, 4);
                        }

                        sizeDelta += SECTOR_BYTES * 2;
                    }

                    if ((file.Length & 0xfff) != 0) {
                        /* the file size is not a multiple of 4KB, grow it */
                        for (int i = 0; i < (file.Length & 0xfff); ++i) {
                            file.WriteByte(0);
                        }
                    }

                    /* set up the available sector map */
                    int nSectors = (int)file.Length / SECTOR_BYTES;
                    sectorFree = new List<Boolean>(nSectors);

                    for (int i = 0; i < nSectors; ++i) {
                        sectorFree.Add(true);
                    }

                    sectorFree[0] = false; // chunk offset table
                    sectorFree[1] = false; // for the last modified info

                    file.Seek(0, SeekOrigin.Begin);
                    for (int i = 0; i < SECTOR_INTS; ++i) {
                        byte[] offsetBytes = new byte[4];
                        file.Read(offsetBytes, 0, 4);

                        if (BitConverter.IsLittleEndian) {
                            Array.Reverse(offsetBytes);
                        }
                        int offset = BitConverter.ToInt32(offsetBytes, 0);

                        offsets[i] = offset;
                        if (offset != 0 && (offset >> 8) + (offset & 0xFF) <= sectorFree.Count) {
                            for (int sectorNum = 0; sectorNum < (offset & 0xFF); ++sectorNum) {
                                sectorFree[(offset >> 8) + sectorNum] = false;
                            }
                        }
                    }
                    for (int i = 0; i < SECTOR_INTS; ++i) {
                        byte[] modBytes = new byte[4];
                        file.Read(modBytes, 0, 4);

                        if (BitConverter.IsLittleEndian) {
                            Array.Reverse(modBytes);
                        }
                        int lastModValue = BitConverter.ToInt32(modBytes, 0);

                        chunkTimestamps[i] = lastModValue;
                    }
                //}
            }
            catch (IOException e) {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine(e.StackTrace);
            }
        }

        /* the modification date of the region file when it was first opened */
        public long LastModified() {
            return lastModified;
        }

        /* gets how much the region file has grown since it was last checked */
        public int GetSizeDelta() {
            int ret = sizeDelta;
            sizeDelta = 0;
            return ret;
        }

        // various small debug printing helpers
        private void Debug(String str) {
    //        System.Consle.Write(str);
        }

        private void Debugln(String str) {
            Debug(str + "\n");
        }

        private void Debug(String mode, int x, int z, String str) {
            Debug("REGION " + mode + " " + fileName + "[" + x + "," + z + "] = " + str);
        }

        private void Debug(String mode, int x, int z, int count, String str) {
            Debug("REGION " + mode + " " + fileName + "[" + x + "," + z + "] " + count + "B = " + str);
        }

        private void Debugln(String mode, int x, int z, String str) {
            Debug(mode, x, z, str + "\n");
        }

        /*
         * gets an (uncompressed) stream representing the chunk data returns null if
         * the chunk is not found or an error occurs
         */
        public Stream GetChunkDataInputStream(int x, int z) {
            if (OutOfBounds(x, z)) {
                Debugln("READ", x, z, "out of bounds");
                return null;
            }

            try {
                int offset = GetOffset(x, z);
                if (offset == 0) {
                    // Debugln("READ", x, z, "miss");
                    return null;
                }

                int sectorNumber = offset >> 8;
                int numSectors = offset & 0xFF;

                if (sectorNumber + numSectors > sectorFree.Count) {
                    Debugln("READ", x, z, "invalid sector");
                    return null;
                }

                file.Seek(sectorNumber * SECTOR_BYTES, SeekOrigin.Begin);
                byte[] lengthBytes = new byte[4];
                file.Read(lengthBytes, 0, 4);

                if (BitConverter.IsLittleEndian) {
                    Array.Reverse(lengthBytes);
                }
                int length = BitConverter.ToInt32(lengthBytes, 0);

                if (length > SECTOR_BYTES * numSectors) {
                    Debugln("READ", x, z, "invalid length: " + length + " > 4096 * " + numSectors);
                    return null;
                }

                byte version = (byte)file.ReadByte();
                if (version == VERSION_GZIP) {
                    byte[] data = new byte[length - 1];
                    file.Read(data, 0, data.Length);
                    Stream ret = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
                    // Debug("READ", x, z, " = found");
                    return ret;
                } else if (version == VERSION_DEFLATE) {
                    byte[] data = new byte[length - 1];
                    file.Read(data, 0, data.Length);
                    Stream ret = new ZlibStream(new MemoryStream(data), CompressionMode.Decompress, true);
                    // Debug("READ", x, z, " = found");
                    return ret;
                }

                Debugln("READ", x, z, "unknown version " + version);
                return null;
            } catch (IOException) {
                Debugln("READ", x, z, "exception");
                return null;
            }
        }

        public Stream GetChunkDataOutputStream(int x, int z) {
            if (OutOfBounds(x, z)) return null;

            return new ZlibStream(new ChunkBuffer(this, x, z), CompressionMode.Compress);
        }

        /*
         * lets chunk writing be multithreaded by not locking the whole file as a
         * chunk is serializing -- only writes when serialization is over
         */
        class ChunkBuffer : MemoryStream {
            private int x, z;
            private RegionFile region;

            public ChunkBuffer(RegionFile r, int x, int z) : base(8096) {
                // super(8096); // initialize to 8KB
                this.region = r;
                this.x = x;
                this.z = z;
            }

            public override void Close() {
                region.Write(x, z, this.GetBuffer(), (int)this.Length);
            }
        }

        /* write a chunk at (x,z) with length bytes of data to disk */
        protected void Write(int x, int z, byte[] data, int length) {
            try {
                int offset = GetOffset(x, z);
                int sectorNumber = offset >> 8;
                int sectorsAllocated = offset & 0xFF;
                int sectorsNeeded = (length + CHUNK_HEADER_SIZE) / SECTOR_BYTES + 1;

                // maximum chunk size is 1MB
                if (sectorsNeeded >= 256) {
                    return;
                }

                if (sectorNumber != 0 && sectorsAllocated == sectorsNeeded) {
                    /* we can simply overwrite the old sectors */
                    Debug("SAVE", x, z, length, "rewrite");
                    Write(sectorNumber, data, length);
                } else {
                    /* we need to allocate new sectors */

                    /* mark the sectors previously used for this chunk as free */
                    for (int i = 0; i < sectorsAllocated; ++i) {
                        sectorFree[sectorNumber + i] = true;
                    }

                    /* scan for a free space large enough to store this chunk */
                    int runStart = sectorFree.FindIndex(b => b == true);
                    int runLength = 0;
                    if (runStart != -1) {
                        for (int i = runStart; i < sectorFree.Count; ++i) {
                            if (runLength != 0) {
                                if (sectorFree[i]) runLength++;
                                else runLength = 0;
                            } else if (sectorFree[i]) {
                                runStart = i;
                                runLength = 1;
                            }
                            if (runLength >= sectorsNeeded) {
                                break;
                            }
                        }
                    }

                    if (runLength >= sectorsNeeded) {
                        /* we found a free space large enough */
                        Debug("SAVE", x, z, length, "reuse");
                        sectorNumber = runStart;
                        SetOffset(x, z, (sectorNumber << 8) | sectorsNeeded);
                        for (int i = 0; i < sectorsNeeded; ++i) {
                            sectorFree[sectorNumber + i] = false;
                        }
                        Write(sectorNumber, data, length);
                    } else {
                        /*
                         * no free space large enough found -- we need to grow the
                         * file
                         */
                        Debug("SAVE", x, z, length, "grow");
                        file.Seek(0, SeekOrigin.End);
                        sectorNumber = sectorFree.Count;
                        for (int i = 0; i < sectorsNeeded; ++i) {
                            file.Write(emptySector, 0, emptySector.Length);
                            sectorFree.Add(false);
                        }
                        sizeDelta += SECTOR_BYTES * sectorsNeeded;

                        Write(sectorNumber, data, length);
                        SetOffset(x, z, (sectorNumber << 8) | sectorsNeeded);
                    }
                }
                SetTimestamp(x, z, Timestamp());
            } catch (IOException e) {
                Console.WriteLine(e.StackTrace);
            }
        }

        /* write a chunk data to the region file at specified sector number */
        private void Write(int sectorNumber, byte[] data, int length) {
            Debugln(" " + sectorNumber);
            file.Seek(sectorNumber * SECTOR_BYTES, SeekOrigin.Begin);

            byte[] bytes = BitConverter.GetBytes(length + 1);
            if (BitConverter.IsLittleEndian) {;
                Array.Reverse(bytes);
            }
            file.Write(bytes, 0, 4); // chunk length
            file.WriteByte(VERSION_DEFLATE); // chunk version number
            file.Write(data, 0, length); // chunk data
        }

        public void DeleteChunk (int x, int z)
        {
            int offset = GetOffset(x, z);
            int sectorNumber = offset >> 8;
            int sectorsAllocated = offset & 0xFF;

            file.Seek(sectorNumber * SECTOR_BYTES, SeekOrigin.Begin);
            for (int i = 0; i < sectorsAllocated; i++) {
                file.Write(emptySector, 0, SECTOR_BYTES);
            }

            SetOffset(x, z, 0);
            SetTimestamp(x, z, 0);
        }

        /* is this an invalid chunk coordinate? */
        private bool OutOfBounds (int x, int z)
        {
            return x < 0 || x >= 32 || z < 0 || z >= 32;
        }

        private int GetOffset(int x, int z) {
            return offsets[x + z * 32];
        }

        public bool HasChunk(int x, int z) {
            return GetOffset(x, z) != 0;
        }

        private void SetOffset(int x, int z, int offset) {
            offsets[x + z * 32] = offset;
            file.Seek((x + z * 32) * 4, SeekOrigin.Begin);

            byte[] bytes = BitConverter.GetBytes(offset);
            if (BitConverter.IsLittleEndian) {;
                Array.Reverse(bytes);
            }

            file.Write(bytes, 0, 4);
        }

        private int Timestamp () {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (int)((DateTime.UtcNow - epoch).Ticks / (10000L * 1000L));
        }

        private int Timestamp (DateTime time)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (int)((time - epoch).Ticks / (10000L * 1000L));
        }

        private void SetTimestamp(int x, int z, int value) {
            chunkTimestamps[x + z * 32] = value;
            file.Seek(SECTOR_BYTES + (x + z * 32) * 4, SeekOrigin.Begin);

            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {;
                Array.Reverse(bytes);
            }

            file.Write(bytes, 0, 4);
        }

        public void Close() {
            file.Close();
        }
    }
}
