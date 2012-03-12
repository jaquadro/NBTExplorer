using System;
using System.Collections.Generic;
using Substrate.Nbt;

namespace Substrate.Core
{
    public class BlockTileTicks
    {
        private IDataArray3 _blocks;
        private TagNodeList _tileTicks;

        private Dictionary<BlockKey, TagNodeCompound> _tileTickTable;

        public event BlockCoordinateHandler TranslateCoordinates;

        public BlockTileTicks (IDataArray3 blocks, TagNodeList tileTicks)
        {
            _blocks = blocks;
            _tileTicks = tileTicks;

            BuildTileTickCache();
        }

        public BlockTileTicks (BlockTileTicks bte)
        {
            _blocks = bte._blocks;
            _tileTicks = bte._tileTicks;

            BuildTileTickCache();
        }

        public int GetTileTickValue (int x, int y, int z)
        {
            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagNodeCompound te;

            if (!_tileTickTable.TryGetValue(key, out te)) {
                return 0;
            }

            if (!te.ContainsKey("t"))
                return 0;

            return te["t"].ToTagInt().Data;
        }

        public void SetTileTickValue (int x, int y, int z, int tickValue)
        {
            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagNodeCompound te;

            if (!_tileTickTable.TryGetValue(key, out te)) {
                TileTick tt = new TileTick()
                {
                    ID = _blocks[x, y, z],
                    Ticks = tickValue,
                    X = key.x,
                    Y = key.y,
                    Z = key.z,
                };
                te = tt.BuildTree() as TagNodeCompound;

                _tileTicks.Add(te);
                _tileTickTable[key] = te;
            }
            else {
                te["t"].ToTagInt().Data = tickValue;
            }
        }

        public TileTick GetTileTick (int x, int y, int z)
        {
            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagNodeCompound te;

            if (!_tileTickTable.TryGetValue(key, out te)) {
                return null;
            }

            if (!te.ContainsKey("i")) {
                return null;
            }

            return TileTick.FromTreeSafe(te);
        }

        public void SetTileTick (int x, int y, int z, TileTick te)
        {
            if (te.ID != _blocks[x, y, z]) {
                throw new ArgumentException("The TileTick type is not valid for this block.", "te");
            }

            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagNodeCompound oldte;

            if (_tileTickTable.TryGetValue(key, out oldte)) {
                _tileTicks.Remove(oldte);
            }

            te.X = key.x;
            te.Y = key.y;
            te.Z = key.z;

            TagNodeCompound tree = te.BuildTree() as TagNodeCompound;

            _tileTicks.Add(tree);
            _tileTickTable[key] = tree;
        }

        public void CreateTileTick (int x, int y, int z)
        {
            TileTick te = new TileTick()
            {
                ID = _blocks[x, y, z],
            };

            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagNodeCompound oldte;

            if (_tileTickTable.TryGetValue(key, out oldte)) {
                _tileTicks.Remove(oldte);
            }

            te.X = key.x;
            te.Y = key.y;
            te.Z = key.z;

            TagNodeCompound tree = te.BuildTree() as TagNodeCompound;

            _tileTicks.Add(tree);
            _tileTickTable[key] = tree;
        }

        public void ClearTileTick (int x, int y, int z)
        {
            BlockKey key = (TranslateCoordinates != null)
                ? TranslateCoordinates(x, y, z)
                : new BlockKey(x, y, z);

            TagNodeCompound te;

            if (!_tileTickTable.TryGetValue(key, out te)) {
                return;
            }

            _tileTicks.Remove(te);
            _tileTickTable.Remove(key);
        }

        private void BuildTileTickCache ()
        {
            _tileTickTable = new Dictionary<BlockKey, TagNodeCompound>();

            foreach (TagNodeCompound te in _tileTicks) {
                int tex = te["x"].ToTagInt();
                int tey = te["y"].ToTagInt();
                int tez = te["z"].ToTagInt();

                BlockKey key = new BlockKey(tex, tey, tez);
                _tileTickTable[key] = te;
            }
        }
    }
}
