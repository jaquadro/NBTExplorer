using Substrate.Core;
using Substrate.Nbt;
using System.Collections.Generic;
using System;
using System.Windows.Forms;

namespace NBTExplorer
{
    public class DataNodeOld
    {
        public DataNodeOld ()
        {
        }

        public DataNodeOld (DataNodeOld parent)
        {
            Parent = parent;
        }

        public DataNodeOld Parent { get; set; }

        private bool _modified;
        public bool Modified
        {
            get { return _modified; }
            set
            {
                if (value && Parent != null) {
                    Parent.Modified = value;
                }
                _modified = value;
            }
        }

        public bool Expanded { get; set; }
    }

    public class NbtDataNode : DataNodeOld
    {
        public NbtDataNode ()
        {
        }

        public NbtDataNode (DataNodeOld parent)
            : base(parent)
        {
        }

        public NbtTree Tree { get; set; }
    }

    public class RegionChunkData : NbtDataNode
    {
        public RegionChunkData (RegionFile file, int x, int z)
            : this(null, file, x, z)
        {
        }

        public RegionChunkData (DataNodeOld parent, RegionFile file, int x, int z)
            : base(parent)
        {
            Region = file;
            X = x;
            Z = z;
        }

        public RegionFile Region { get; private set; }
        public int X { get; private set; }
        public int Z { get; private set; }
    }

    public class RegionData : DataNodeOld
    {
        public RegionData (string path)
            : this(null, path)
        {
        }

        public RegionData (DataNodeOld parent, string path)
            : base(parent)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }

    public class NbtFileData : NbtDataNode
    {
        public NbtFileData (string path, CompressionType cztype)
            : this(null, path, cztype)
        {
        }

        public NbtFileData (DataNodeOld parent, string path, CompressionType cztype)
            : base(parent)
        {
            Path = path;
            CompressionType = cztype;
        }

        public string Path { get; private set; }
        public CompressionType CompressionType { get; private set; }
    }

    public class DirectoryData : DataNodeOld
    {
        public DirectoryData (string path)
            : this(null, path)
        {
        }

        public DirectoryData (DataNodeOld parent, string path)
            : base(parent)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}
