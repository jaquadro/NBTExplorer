using System;
using System.Collections.Generic;
using Substrate.Nbt;

namespace Substrate.Core
{
    public delegate BlockKey BlockCoordinateHandler (int lx, int ly, int lz);

    public class BlockTileEntities
    {
        private IDataArray3 _blocks;
        private TagNodeList _tileEntities;

        private Dictionary<BlockKey, TagNodeCompound> _tileEntityTable;

        public event BlockCoordinateHandler TranslateCoordinates;

        public BlockTileEntities (IDataArray3 blocks, TagNodeList tileEntities)
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

            TagNodeCompound te;

            if (!_tileEntityTable.TryGetValue(key, out te)) {
                return null;
            }

            return TileEntityFactory.CreateGeneric(te);
        }

        public void SetTileEntity (int x, int y, int z, TileEntity te)
        {
            BlockInfoEx info = BlockInfo.BlockTable[_blocks[x, y, z]] as BlockInfoEx;
            if (info != null) {
                if (te.GetType() != TileEntityFactory.Lookup(info.TileEntityName))
                    throw new ArgumentException("The TileEntity type is not valid for this block.", "te");
            }

            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagNodeCompound oldte;

            if (_tileEntityTable.TryGetValue(key, out oldte)) {
                _tileEntities.Remove(oldte);
            }

            te.X = key.x;
            te.Y = key.y;
            te.Z = key.z;

            TagNodeCompound tree = te.BuildTree() as TagNodeCompound;

            _tileEntities.Add(tree);
            _tileEntityTable[key] = tree;
        }

        public void CreateTileEntity (int x, int y, int z)
        {
            BlockInfoEx info = BlockInfo.BlockTable[_blocks[x, y, z]] as BlockInfoEx;
            if (info == null) {
                throw new InvalidOperationException("The given block is of a type that does not support TileEntities.");
            }

            TileEntity te = TileEntityFactory.Create(info.TileEntityName);
            if (te == null) {
                throw new UnknownTileEntityException("The TileEntity type '" + info.TileEntityName + "' has not been registered with the TileEntityFactory.");
            }

            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagNodeCompound oldte;

            if (_tileEntityTable.TryGetValue(key, out oldte)) {
                _tileEntities.Remove(oldte);
            }

            te.X = key.x;
            te.Y = key.y;
            te.Z = key.z;

            TagNodeCompound tree = te.BuildTree() as TagNodeCompound;

            _tileEntities.Add(tree);
            _tileEntityTable[key] = tree;
        }

        public void ClearTileEntity (int x, int y, int z)
        {
            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagNodeCompound te;

            if (!_tileEntityTable.TryGetValue(key, out te)) {
                return;
            }

            _tileEntities.Remove(te);
            _tileEntityTable.Remove(key);
        }

        private void BuildTileEntityCache ()
        {
            _tileEntityTable = new Dictionary<BlockKey, TagNodeCompound>();

            foreach (TagNodeCompound te in _tileEntities) {
                int tex = te["x"].ToTagInt();
                int tey = te["y"].ToTagInt();
                int tez = te["z"].ToTagInt();

                BlockKey key = new BlockKey(tex, tey, tez);
                _tileEntityTable[key] = te;
            }
        }
    }
}
