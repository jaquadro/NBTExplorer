using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NBT
{
    class TagLookup
    {
        public NBT_Tag level;
        public NBT_Tag blocks;
        public NBT_Tag data;
        public NBT_Tag blocklight;
        public NBT_Tag skylight;
        public NBT_Tag heightmap;

        public NBT_ByteArray blocksArray;
        public NBT_ByteArray dataArray;
        public NBT_ByteArray blocklightArray;
        public NBT_ByteArray skylightArray;
    }

    public class NBTChunk : MC.Chunk
    {
        NBT_File file;
        TagLookup table;

        String dirPath;
        String chunkName;

        private int x;
        private int z;

        bool dirty;

        public NBTChunk (String path, String name)
        {
            dirPath = path;
            chunkName = name;

            Match match = Regex.Match(name, @"^c\.(-?[0-9a-z]+)\.(-?[0-9a-z]+)\.dat$");

            x = Util.Base36.ToInteger(match.Groups[1].Value);
            z = Util.Base36.ToInteger(match.Groups[2].Value);
        }

        public override int GetX ()
        {
            return x;
        }

        public override int GetZ ()
        {
            return z;
        }

        public void LoadChunk ()
        {
            file = new NBT_File(dirPath + "\\" + chunkName);
        }

        public void ActivateChunk ()
        {
            file.activate();

            table = new TagLookup();
            table.level = file.getRoot().findTagByName("Level");

            if (table.level != null) {
                table.blocks = table.level.findTagByName("Blocks");
                table.data = table.level.findTagByName("Data");
                table.blocklight = table.level.findTagByName("BlockLight");
                table.skylight = table.level.findTagByName("SkyLight");
                table.heightmap = table.level.findTagByName("HeightMap");

                if (table.blocks != null) {
                    table.blocksArray = table.blocks.value.toByteArray();
                }

                if (table.data != null) {
                    table.dataArray = table.data.value.toByteArray();
                }

                if (table.blocklight != null) {
                    table.blocklightArray = table.blocklight.value.toByteArray();
                }

                if (table.skylight != null) {
                    table.skylightArray = table.skylight.value.toByteArray();
                }
            }
        }

        public void VerifyChunk ()
        {
            if (file.getRoot() != null) {
                // Blocks
                if (table.blocks == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [Blocks]");
                }
                else if (table.blocks.type != NBT_Type.TAG_BYTE_ARRAY) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [Blocks]");
                }
                else if (table.blocks.value.toByteArray().length != (16 * 16 * 128)) {
                    throw new NBTChunkException(NBTChunkException.MSG_MALFORMED_TAG + " [Blocks]");
                }

                // Data
                if (table.data == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [Data]");
                }
                else if (table.data.type != NBT_Type.TAG_BYTE_ARRAY) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [Data]");
                }
                else if (table.data.value.toByteArray().length != (16 * 16 * 128 / 2)) {
                    throw new NBTChunkException(NBTChunkException.MSG_MALFORMED_TAG + " [Data]");
                }

                // BlockLight
                if (table.blocklight == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [BlockLight]");
                }
                else if (table.blocklight.type != NBT_Type.TAG_BYTE_ARRAY) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [BlockLight]");
                }
                else if (table.blocklight.value.toByteArray().length != (16 * 16 * 128 / 2)) {
                    throw new NBTChunkException(NBTChunkException.MSG_MALFORMED_TAG + " [BlockLight]");
                }

                // SkyLight
                if (table.skylight == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [SkyLight]");
                }
                else if (table.skylight.type != NBT_Type.TAG_BYTE_ARRAY) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [SkyLight]");
                }
                else if (table.skylight.value.toByteArray().length != (16 * 16 * 128 / 2)) {
                    throw new NBTChunkException(NBTChunkException.MSG_MALFORMED_TAG + " [SkyLight]");
                }

                // HeightMap
                if (table.heightmap == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [HeightMap]");
                }
                else if (table.heightmap.type != NBT_Type.TAG_BYTE_ARRAY) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [HeightMap]");
                }
                else if (table.heightmap.value.toByteArray().length != (16 * 16)) {
                    throw new NBTChunkException(NBTChunkException.MSG_MALFORMED_TAG + " [HeightMap]");
                }

                // xPos
                NBT_Tag xPos = table.level.findTagByName("xPos");
                if (xPos == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [xPos]");
                }
                else if (xPos.type != NBT_Type.TAG_INT) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [xPos]");
                }

                // zPos
                NBT_Tag zPos = table.level.findTagByName("zPos");
                if (zPos == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [zPos]");
                }
                else if (zPos.type != NBT_Type.TAG_INT) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [zPos]");
                }

                // LastUpdate
                NBT_Tag lastUpdate = table.level.findTagByName("LastUpdate");
                if (lastUpdate == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [LastUpdate]");
                }
                else if (lastUpdate.type != NBT_Type.TAG_LONG) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [LastUpdate]");
                }

                // TerrainPopulated
                NBT_Tag tp = table.level.findTagByName("TerrainPopulated");
                if (tp == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [TerrainPopulated]");
                }
                else if (tp.type != NBT_Type.TAG_BYTE) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [TerrainPopulated]");
                }
                else if (tp.value.toByte().data != 0 && tp.value.toByte().data != 1) {
                    throw new NBTChunkException(NBTChunkException.MSG_MALFORMED_TAG + " [TerrainPopulated]");
                }

                // Entities
                NBT_Tag ent = table.level.findTagByName("Entities");
                if (ent == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [Entities]");
                }
                else if (ent.type != NBT_Type.TAG_LIST) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [Entities]");
                }
                else if (ent.value.toList().length > 0 && ent.value.toList().type != NBT_Type.TAG_COMPOUND) {
                    throw new NBTChunkException(NBTChunkException.MSG_MALFORMED_TAG + " [Entities]");
                }

                VerifyEntities(ent);

                // TileEntities
                NBT_Tag tent = table.level.findTagByName("TileEntities");
                if (tent == null) {
                    throw new NBTChunkException(NBTChunkException.MSG_MISSING_TAG + " [TileEntities]");
                }
                else if (tent.type != NBT_Type.TAG_LIST) {
                    throw new NBTChunkException(NBTChunkException.MSG_TAG_TYPE + " [TileEntities]");
                }
                else if (tent.value.toList().length > 0 && tent.value.toList().type != NBT_Type.TAG_COMPOUND) {
                    throw new NBTChunkException(NBTChunkException.MSG_MALFORMED_TAG + " [TileEntities]");
                }

                VerifyTileEntities(tent);
            }
        }

        private void VerifyEntities (NBT_Tag entities) {

        }

        private void VerifyTileEntities (NBT_Tag entities)
        {

        }

        private int IndexAt (int x, int y, int z)
        {
            return y + ((z * 128) + (x * 128 * 16));
        }

        override public int GetBlockId (int x, int y, int z)
        {
            int index = IndexAt(x, y, z);
            return table.blocksArray.data[index];
        }

        override public void SetBlockId (int x, int y, int z, int id)
        {
            byte byteId = (byte)(id % 256);

            if (x >= 0 && x < 16 && y >= 0 && y < 128 && z >= 0 && z < 16) {
                int index = IndexAt(x, y, z);
                if (table.blocksArray.data[index] != id) {
                    dirty = true;
                }

                table.blocksArray.data[index] = byteId;
            }
        }

        override public int GetBlockData (int x, int y, int z)
        {
            int index = IndexAt(x, y, z);
            if (index % 2 == 0) {
                return table.blocksArray.data[index / 2] & 0x0F;
            }
            else {
                return (table.blocksArray.data[index / 2] & 0xF0) >> 4;
            }
        }

        override public void SetBlockData (int x, int y, int z, int data)
        {
            byte byteData = (byte)(data % 256);

            if (x >= 0 && x < 16 && y >= 0 && y < 128 && z >= 0 && z < 16) {
                int index = IndexAt(x, y, z);
                if (index % 2 == 0) {
                    byte old = (byte)(table.blocksArray.data[index / 2] & 0x0F);
                    if (old != data) {
                        dirty = true;
                    }
                    table.blocksArray.data[index / 2] = (byte)((table.blocksArray.data[index / 2] & 0xF0) | (byteData & 0x0F));
                }
                else {
                    byte old = (byte)((table.blocksArray.data[index / 2] & 0xF0) >> 4);
                    if (old != data) {
                        dirty = true;
                    }
                    table.blocksArray.data[index / 2] = (byte)((table.blocksArray.data[index / 2] & 0x0F) | ((byteData & 0x0F) << 4));
                }
            }
        }

        override public int GetBlockLight (int x, int y, int z)
        {
            int index = IndexAt(x, y, z);
            if (index % 2 == 0) {
                return table.blocklightArray.data[index / 2] & 0x0F;
            }
            else {
                return (table.blocklightArray.data[index / 2] & 0xF0) >> 4;
            }
        }

        override public void SetBlockLight (int x, int y, int z, int data)
        {
            byte byteData = (byte)(data % 256);

            if (x >= 0 && x < 16 && y >= 0 && y < 128 && z >= 0 && z < 16) {
                int index = IndexAt(x, y, z);
                if (index % 2 == 0) {
                    byte old = (byte)(table.blocklightArray.data[index / 2] & 0x0F);
                    if (old != data) {
                        dirty = true;
                    }
                    table.blocklightArray.data[index / 2] = (byte)((table.blocklightArray.data[index / 2] & 0xF0) | (byteData & 0x0F));
                }
                else {
                    byte old = (byte)((table.blocklightArray.data[index / 2] & 0xF0) >> 4);
                    if (old != data) {
                        dirty = true;
                    }
                    table.blocklightArray.data[index / 2] = (byte)((table.blocklightArray.data[index / 2] & 0x0F) | ((byteData & 0x0F) << 4));
                }
            }
        }
    }

    class NBTChunkException : Exception
    {
        public const String MSG_MISSING_TAG = "Chunk Error: Missing required tag";
        public const String MSG_TAG_TYPE = "Chunk Error: Tag not expected type";
        public const String MSG_MALFORMED_TAG = "Chunk Error: Malformed tag";

        public NBTChunkException () { }

        public NBTChunkException (String msg) : base(msg) { }

        public NBTChunkException (String msg, Exception innerException) : base(msg, innerException) { }
    }
}
