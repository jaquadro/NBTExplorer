using System;
using System.Collections.Generic;

using Substrate.Utility;
using Substrate.NBT;

namespace Substrate
{
    public delegate BlockKey BlockCoordinateHandler (int lx, int ly, int lz);

    public class BlockTileEntities
    {
        private XZYByteArray _blocks;
        private TagList _tileEntities;

        private Dictionary<BlockKey, TagCompound> _tileEntityTable;

        public event BlockCoordinateHandler TranslateCoordinates;

        public BlockTileEntities (XZYByteArray blocks, TagList tileEntities)
        {
            _blocks = blocks;
            _tileEntities = tileEntities;

            BuildTileEntityCache();
        }

        public BlockTileEntities (BlockTileEntities bte)
        {
            _blocks = bte._blocks;
            _tileEntities = bte._tileEntities;

            BuildTileEntityCache();
        }

        public TileEntity GetTileEntity (int x, int y, int z)
        {
            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagCompound te;

            if (!_tileEntityTable.TryGetValue(key, out te)) {
                return null;
            }

            return TileEntityFactory.Create(te);
        }

        public void SetTileEntity (int x, int y, int z, TileEntity te)
        {
            BlockInfoEx info = BlockInfo.BlockTable[_blocks[x, y, z]] as BlockInfoEx;
            if (info == null) {
                return;
            }

            if (te.GetType() != TileEntityFactory.Lookup(info.TileEntityName)) {
                return;
            }

            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagCompound oldte;

            if (_tileEntityTable.TryGetValue(key, out oldte)) {
                _tileEntities.Remove(oldte);
            }

            te.X = key.x;
            te.Y = key.y;
            te.Z = key.z;

            TagCompound tree = te.BuildTree() as TagCompound;

            _tileEntities.Add(tree);
            _tileEntityTable[key] = tree;
        }

        public void CreateTileEntity (int x, int y, int z)
        {
            BlockInfoEx info = BlockInfo.BlockTable[_blocks[x, y, z]] as BlockInfoEx;
            if (info == null) {
                return;
            }

            TileEntity te = TileEntityFactory.Create(info.TileEntityName);
            if (te == null) {
                return;
            }

            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagCompound oldte;

            if (_tileEntityTable.TryGetValue(key, out oldte)) {
                _tileEntities.Remove(oldte);
            }

            te.X = key.x;
            te.Y = key.y;
            te.Z = key.z;

            TagCompound tree = te.BuildTree() as TagCompound;

            _tileEntities.Add(tree);
            _tileEntityTable[key] = tree;
        }

        public void ClearTileEntity (int x, int y, int z)
        {
            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagCompound te;

            if (!_tileEntityTable.TryGetValue(key, out te)) {
                return;
            }

            _tileEntities.Remove(te);
            _tileEntityTable.Remove(key);
        }

        private void BuildTileEntityCache ()
        {
            _tileEntityTable = new Dictionary<BlockKey, TagCompound>();

            foreach (TagCompound te in _tileEntities) {
                int tex = te["x"].ToTagInt();
                int tey = te["y"].ToTagInt();
                int tez = te["z"].ToTagInt();

                BlockKey key = new BlockKey(tex, tey, tez);
                _tileEntityTable[key] = te;
            }
        }
    }
}
