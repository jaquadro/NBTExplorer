using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Core;
using Substrate.Nbt;
using System.IO;

namespace Substrate.Data
{
    /// <summary>
    /// Represents the complete data of a Map item.
    /// </summary>
    public class Map : INbtObject<Map>, ICopyable<Map>
    {
        private static SchemaNodeCompound _schema = new SchemaNodeCompound()
        {
            new SchemaNodeCompound("data")
            {
                new SchemaNodeScaler("scale", TagType.TAG_BYTE),
                new SchemaNodeScaler("dimension", TagType.TAG_BYTE),
                new SchemaNodeScaler("height", TagType.TAG_SHORT),
                new SchemaNodeScaler("width", TagType.TAG_SHORT),
                new SchemaNodeScaler("xCenter", TagType.TAG_INT),
                new SchemaNodeScaler("zCenter", TagType.TAG_INT),
                new SchemaNodeArray("colors"),
            },
        };

        private TagNodeCompound _source;

        private NbtWorld _world;
        private int _id;

        private byte _scale;
        private byte _dimension;
        private short _height;
        private short _width;
        private int _x;
        private int _z;

        private byte[] _colors;

        /// <summary>
        /// Creates a new default <see cref="Map"/> object.
        /// </summary>
        public Map ()
        {
            _scale = 3;
            _dimension = 0;
            _height = 128;
            _width = 128;

            _colors = new byte[_width * _height];
        }

        /// <summary>
        /// Creates a new <see cref="Map"/> object with copied data.
        /// </summary>
        /// <param name="p">A <see cref="Map"/> to copy data from.</param>
        protected Map (Map p)
        {
            _world = p._world;
            _id = p._id;

            _scale = p._scale;
            _dimension = p._dimension;
            _height = p._height;
            _width = p._width;
            _x = p._x;
            _z = p._z;

            _colors = new byte[_width * _height];
            if (p._colors != null) {
                p._colors.CopyTo(_colors, 0);
            }
        }

        /// <summary>
        /// Gets or sets the id value associated with this map.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id < 0 || _id >= 65536) {
                    throw new ArgumentOutOfRangeException("value", value, "Map Ids must be in the range [0, 65535].");
                }
                _id = value;
            }
        }

        /// <summary>
        /// Gets or sets the scale of the map.  Acceptable values are 0 (1:1) to 4 (1:16).
        /// </summary>
        public int Scale
        {
            get { return _scale; }
            set { _scale = (byte)value; }
        }

        /// <summary>
        /// Gets or sets the (World) Dimension of the map.
        /// </summary>
        public int Dimension
        {
            get { return _dimension; }
            set { _dimension = (byte)value; }
        }

        /// <summary>
        /// Gets or sets the height of the map.
        /// </summary>
        /// <remarks>If the new height dimension is different, the map's color data will be reset.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the new height value is zero or negative.</exception>
        public int Height
        {
            get { return _height; }
            set
            {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("value", "Height must be a positive number");
                }
                if (_height != value) {
                    _height = (short)value;
                    _colors = new byte[_width * _height];
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the map.
        /// </summary>
        /// <remarks>If the new width dimension is different, the map's color data will be reset.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the new width value is zero or negative.</exception>
        public int Width
        {
            get { return _width; }
            set
            {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("value", "Width must be a positive number");
                }
                if (_width != value) {
                    _width = (short)value;
                    _colors = new byte[_width * _height];
                }
            }
        }

        /// <summary>
        /// Gets or sets the global X-coordinate that this map is centered on, in blocks.
        /// </summary>
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Gets or sets the global Z-coordinate that this map is centered on, in blocks.
        /// </summary>
        public int Z
        {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        /// Gets the raw byte array of the map's color index values.
        /// </summary>
        public byte[] Colors
        {
            get { return _colors; }
        }

        /// <summary>
        /// Gets or sets a color index value within the map's internal colors bitmap.
        /// </summary>
        /// <param name="x">The X-coordinate to get or set.</param>
        /// <param name="z">The Z-coordinate to get or set.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown when the X- or Z-coordinates exceed the map dimensions.</exception>
        public byte this[int x, int z]
        {
            get
            {
                if (x < 0 || x >= _width || z < 0 || z >= _height) {
                    throw new IndexOutOfRangeException();
                }
                return _colors[x + _width * z];
            }

            set
            {
                if (x < 0 || x >= _width || z < 0 || z >= _height) {
                    throw new IndexOutOfRangeException();
                }
                _colors[x + _width * z] = value;
            }
        }


        /// <summary>
        /// Saves a <see cref="Map"/> object to disk as a standard compressed NBT stream.
        /// </summary>
        /// <returns>True if the map was saved; false otherwise.</returns>
        /// <exception cref="Exception">Thrown when an error is encountered writing out the level.</exception>
        public bool Save ()
        {
            if (_world == null) {
                return false;
            }

            try {
                string path = Path.Combine(_world.Path, _world.DataDirectory);
                NBTFile nf = new NBTFile(Path.Combine(path, "map_" + _id + ".dat"));

                Stream zipstr = nf.GetDataOutputStream();
                if (zipstr == null) {
                    NbtIOException nex = new NbtIOException("Failed to initialize compressed NBT stream for output");
                    nex.Data["Map"] = this;
                    throw nex;
                }

                new NbtTree(BuildTree() as TagNodeCompound).WriteTo(zipstr);
                zipstr.Close();

                return true;
            }
            catch (Exception ex) {
                Exception mex = new Exception("Could not save map file.", ex); // TODO: Exception Type
                mex.Data["Map"] = this;
                throw mex;
            }
        }


        #region INBTObject<Map> Members

        /// <summary>
        /// Attempt to load a Map subtree into the <see cref="Map"/> without validation.
        /// </summary>
        /// <param name="tree">The root node of a Map subtree.</param>
        /// <returns>The <see cref="Map"/> returns itself on success, or null if the tree was unparsable.</returns>
        public virtual Map LoadTree (TagNode tree)
        {
            TagNodeCompound dtree = tree as TagNodeCompound;
            if (dtree == null) {
                return null;
            }

            TagNodeCompound ctree = dtree["data"].ToTagCompound();

            _scale = ctree["scale"].ToTagByte();
            _dimension = ctree["dimension"].ToTagByte();
            _height = ctree["height"].ToTagShort();
            _width = ctree["width"].ToTagShort();
            _x = ctree["xCenter"].ToTagInt();
            _z = ctree["zCenter"].ToTagInt();

            _colors = ctree["colors"].ToTagByteArray();

            _source = ctree.Copy() as TagNodeCompound;

            return this;
        }

        /// <summary>
        /// Attempt to load a Map subtree into the <see cref="Map"/> with validation.
        /// </summary>
        /// <param name="tree">The root node of a Map subtree.</param>
        /// <returns>The <see cref="Map"/> returns itself on success, or null if the tree failed validation.</returns>
        public virtual Map LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            Map map = LoadTree(tree);

            if (map != null) {
                if (map._colors.Length != map._width * map._height) {
                    throw new Exception("Unexpected length of colors byte array in Map"); // TODO: Expception Type
                }
            }

            return map;
        }

        /// <summary>
        /// Builds a Map subtree from the current data.
        /// </summary>
        /// <returns>The root node of a Map subtree representing the current data.</returns>
        public virtual TagNode BuildTree ()
        {
            TagNodeCompound data = new TagNodeCompound();
            data["scale"] = new TagNodeByte(_scale);
            data["dimension"] = new TagNodeByte(_dimension);
            data["height"] = new TagNodeShort(_height);
            data["width"] = new TagNodeShort(_width);
            data["xCenter"] = new TagNodeInt(_x);
            data["zCenter"] = new TagNodeInt(_z);

            data["colors"] = new TagNodeByteArray(_colors);

            if (_source != null) {
                data.MergeFrom(_source);
            }

            TagNodeCompound tree = new TagNodeCompound();
            tree.Add("data", data);

            return tree;
        }

        /// <summary>
        /// Validate a Map subtree against a schema defintion.
        /// </summary>
        /// <param name="tree">The root node of a Map subtree.</param>
        /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
        public virtual bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, _schema).Verify();
        }

        #endregion


        #region ICopyable<Map> Members

        /// <summary>
        /// Creates a deep-copy of the <see cref="Map"/>.
        /// </summary>
        /// <returns>A deep-copy of the <see cref="Map"/>.</returns>
        public virtual Map Copy ()
        {
            return new Map(this);
        }

        #endregion
    }
}
