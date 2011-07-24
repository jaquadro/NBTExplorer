using System;
using System.Collections.Generic;
using System.IO;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate.ImportExport
{
    public class Schematic
    {
        private static SchemaNodeCompound _schema = new SchemaNodeCompound()
        {
            new SchemaNodeScaler("Width", TagType.TAG_SHORT),
            new SchemaNodeScaler("Length", TagType.TAG_SHORT),
            new SchemaNodeScaler("Height", TagType.TAG_SHORT),
            new SchemaNodeString("Materials", "Alpha"),
            new SchemaNodeArray("Blocks"),
            new SchemaNodeArray("Data"),
            new SchemaNodeList("Entities", TagType.TAG_COMPOUND, Entity.Schema),
            new SchemaNodeList("TileEntities", TagType.TAG_COMPOUND, TileEntity.Schema),
        };

        private XZYByteArray _blocks;
        private XZYNibbleArray _data;
        private XZYNibbleArray _blockLight;
        private XZYNibbleArray _skyLight;
        private ZXByteArray _heightMap;

        private TagNodeList _entities;
        private TagNodeList _tileEntities;

        private AlphaBlockCollection _blockset;
        private EntityCollection _entityset;

        private Schematic ()
        {
        }

        public Schematic (AlphaBlockCollection blocks, EntityCollection entities)
        {
            _blockset = blocks;
            _entityset = entities;
        }

        public Schematic (int xdim, int ydim, int zdim)
        {
            _blocks = new XZYByteArray(xdim, ydim, zdim);
            _data = new XZYNibbleArray(xdim, ydim, zdim);
            _blockLight = new XZYNibbleArray(xdim, ydim, zdim);
            _skyLight = new XZYNibbleArray(xdim, ydim, zdim);
            _heightMap = new ZXByteArray(xdim, zdim);

            _entities = new TagNodeList(TagType.TAG_COMPOUND);
            _tileEntities = new TagNodeList(TagType.TAG_COMPOUND);

            _blockset = new AlphaBlockCollection(_blocks, _data, _blockLight, _skyLight, _heightMap, _tileEntities);
            _entityset = new EntityCollection(_entities);
        }

        #region Properties

        public AlphaBlockCollection Blocks
        {
            get { return _blockset; }
            set { _blockset = value; }
        }

        public EntityCollection Entities
        {
            get { return _entityset; }
            set { _entityset = value; }
        }

        #endregion

        public static Schematic Import (string path)
        {
            NBTFile schematicFile = new NBTFile(path);
            if (!schematicFile.Exists()) {
                return null;
            }

            Stream nbtStream = schematicFile.GetDataInputStream();
            if (nbtStream == null) {
                return null;
            }

            NbtTree tree = new NbtTree(nbtStream);

            NbtVerifier v = new NbtVerifier(tree.Root, _schema);
            if (!v.Verify()) {
                return null;
            }

            //TagNodeCompound schematic = tree.Root["Schematic"] as TagNodeCompound;
            TagNodeCompound schematic = tree.Root;
            int xdim = schematic["Width"].ToTagShort();
            int zdim = schematic["Length"].ToTagShort();
            int ydim = schematic["Height"].ToTagShort();

            Schematic self = new Schematic(xdim, ydim, zdim);

            // Damnit, schematic is YZX ordering.
            YZXByteArray schemaBlocks = new YZXByteArray(xdim, ydim, zdim, schematic["Blocks"].ToTagByteArray());
            YZXByteArray schemaData = new YZXByteArray(xdim, ydim, zdim, schematic["Data"].ToTagByteArray());

            for (int x = 0; x < xdim; x++) {
                for (int y = 0; y < ydim; y++) {
                    for (int z = 0; z < zdim; z++) {
                        self._blocks[x, y, z] = schemaBlocks[x, y, z];
                        self._data[x, y, z] = schemaData[x, y, z];
                    }
                }
            }

            TagNodeList entities = schematic["Entities"] as TagNodeList;
            foreach (TagNode e in entities) {
                self._entities.Add(e);
            }

            TagNodeList tileEntities = schematic["TileEntities"] as TagNodeList;
            foreach (TagNode te in tileEntities) {
                self._tileEntities.Add(te);
            }

            self._blockset.Refresh();

            return self;
        }

        public void Export (string path)
        {
            int xdim = _blockset.XDim;
            int ydim = _blockset.YDim;
            int zdim = _blockset.ZDim;

            byte[] blockData = new byte[xdim * ydim * zdim];
            byte[] dataData = new byte[xdim * ydim * zdim];

            YZXByteArray schemaBlocks = new YZXByteArray(_blockset.XDim, _blockset.YDim, _blockset.ZDim, blockData);
            YZXByteArray schemaData = new YZXByteArray(_blockset.XDim, _blockset.YDim, _blockset.ZDim, dataData);

            TagNodeList entities = new TagNodeList(TagType.TAG_COMPOUND);
            TagNodeList tileEntities = new TagNodeList(TagType.TAG_COMPOUND);

            for (int x = 0; x < xdim; x++) {
                for (int z = 0; z < zdim; z++) {
                    for (int y = 0; y < ydim; y++) {
                        AlphaBlock block = _blockset.GetBlock(x, y, z);
                        schemaBlocks[x, y, z] = (byte)block.ID;
                        schemaData[x, y, z] = (byte)block.Data;

                        TileEntity te = block.GetTileEntity();
                        if (te != null) {
                            te.X = x;
                            te.Y = y;
                            te.Z = z;

                            tileEntities.Add(te.BuildTree());
                        }
                    }
                }
            }

            foreach (EntityTyped e in _entityset) {
                entities.Add(e.BuildTree());
            }

            TagNodeCompound schematic = new TagNodeCompound();
            schematic["Width"] = new TagNodeShort((short)xdim);
            schematic["Length"] = new TagNodeShort((short)zdim);
            schematic["Height"] = new TagNodeShort((short)ydim);

            schematic["Entities"] = entities;
            schematic["TileEntities"] = tileEntities;

            schematic["Materials"] = new TagNodeString("Alpha");

            schematic["Blocks"] = new TagNodeByteArray(blockData);
            schematic["Data"] = new TagNodeByteArray(dataData);

            NBTFile schematicFile = new NBTFile(path);

            Stream nbtStream = schematicFile.GetDataOutputStream();
            if (nbtStream == null) {
                return;
            }

            NbtTree tree = new NbtTree(schematic, "Schematic");
            tree.WriteTo(nbtStream);

            nbtStream.Close();
        }
    }
}
