using System;
using System.Collections;
using System.Collections.Generic;

using Substrate.Utility;

namespace Substrate
{

    public class BlockLight
    {
        private struct LightRecord
        {
            public int x;
            public int y;
            public int z;
            public int str;

            public LightRecord (int _x, int _y, int _z, int s)
            {
                x = _x;
                y = _y;
                z = _z;
                str = s;
            }
        }

        private IBoundedLitBlockCollection _blockset;

        // Maintains internal state of multi-block relighting algorithms
        private BitArray _lightbit;
        private Queue<BlockKey> _update;

        public delegate IBoundedLitBlockCollection NeighborLookupHandler (int relx, int rely, int relz);

        public event NeighborLookupHandler ResolveNeighbor;

        public BlockLight (IBoundedLitBlockCollection blockset)
        {
            _blockset = blockset;

            _lightbit = new BitArray(_blockset.XDim * 3 * _blockset.ZDim * 3 * _blockset.YDim);
            _update = new Queue<BlockKey>();
        }

        public BlockLight (BlockLight bl)
        {
            _blockset = bl._blockset;

            _lightbit = new BitArray(_blockset.XDim * 3 * _blockset.ZDim * 3 * _blockset.YDim);
            _update = new Queue<BlockKey>();
        }

        public void UpdateBlockLight (int lx, int ly, int lz)
        {
            BlockKey primary = new BlockKey(lx, ly, lz);
            _update.Enqueue(primary);

            BlockInfo info = _blockset.GetInfo(lx, ly, lz);
            //if (info.Luminance > BlockInfo.MIN_LUMINANCE || info.TransmitsLight) {
                QueueRelight(new BlockKey(lx - 1, ly, lz));
                QueueRelight(new BlockKey(lx + 1, ly, lz));
                QueueRelight(new BlockKey(lx, ly - 1, lz));
                QueueRelight(new BlockKey(lx, ly + 1, lz));
                QueueRelight(new BlockKey(lx, ly, lz - 1));
                QueueRelight(new BlockKey(lx, ly, lz + 1));
            //}

            UpdateBlockLight();
        }

        public void UpdateBlockSkyLight (int lx, int ly, int lz)
        {
            BlockKey primary = new BlockKey(lx, ly, lz);
            _update.Enqueue(primary);

            UpdateBlockSkyLight();
        }


        public void RebuildBlockLight ()
        {
            IBoundedLitBlockCollection[,] chunkMap = LocalBlockLightMap();

            int xdim = _blockset.XDim;
            int ydim = _blockset.YDim;
            int zdim = _blockset.ZDim;

            for (int x = 0; x < xdim; x++) {
                for (int z = 0; z < zdim; z++) {
                    for (int y = 0; y < ydim; y++) {
                        BlockInfo info = _blockset.GetInfo(x, y, z);
                        if (info.Luminance > 0) {
                            SpreadBlockLight(chunkMap, x, y, z);
                        }
                    }
                }
            }
        }

        public void RebuildBlockSkyLight ()
        {
            IBoundedLitBlockCollection[,] chunkMap = LocalBlockLightMap();
            int[,] heightMap = LocalHeightMap(chunkMap);

            int xdim = _blockset.XDim;
            int ydim = _blockset.YDim;
            int zdim = _blockset.ZDim;

            // Optimization - only need to queue at level of highest neighbor's height
            for (int x = 0; x < xdim; x++) {
                for (int z = 0; z < zdim; z++) {
                    int xi = x + _blockset.XDim;
                    int zi = z + _blockset.ZDim;

                    int h = heightMap[xi, zi];
                    h = Math.Max(h, heightMap[xi, zi - 1]);
                    h = Math.Max(h, heightMap[xi - 1, zi]);
                    h = Math.Max(h, heightMap[xi + 1, zi]);
                    h = Math.Max(h, heightMap[xi, zi + 1]);

                    for (int y = h + 1; y < ydim; y++) {
                        _blockset.SetSkyLight(x, y, z, BlockInfo.MAX_LUMINANCE);
                    }

                    //QueueRelight(new BlockKey(x, h, z));
                    SpreadSkyLight(chunkMap, heightMap, x, h, z);
                }
            }
        }

        public void RebuildHeightMap ()
        {
            int xdim = _blockset.XDim;
            int ydim = _blockset.YDim;
            int zdim = _blockset.ZDim;

            for (int x = 0; x < xdim; x++) {
                for (int z = 0; z < zdim; z++) {
                    for (int y = ydim - 1; y >= 0; y--) {
                        BlockInfo info = _blockset.GetInfo(x, y, z);
                        if (info.Opacity > BlockInfo.MIN_OPACITY || !info.TransmitsLight) {
                            _blockset.SetHeight(x, z, y);
                            break;
                        }
                    }
                }
            }
        }


        public void StitchBlockLight ()
        {
            IBoundedLitBlockCollection[,] map = LocalBlockLightMap();

            if (map[1, 0] != null) {
                StitchBlockLight(map[1, 0], BlockCollectionEdge.EAST);
            }
            if (map[0, 1] != null) {
                StitchBlockLight(map[0, 1], BlockCollectionEdge.NORTH);
            }
            if (map[1, 2] != null) {
                StitchBlockLight(map[1, 2], BlockCollectionEdge.WEST);
            }
            if (map[2, 1] != null) {
                StitchBlockLight(map[2, 1], BlockCollectionEdge.SOUTH);
            }
        }

        // TODO: Revise to cache the specified chunk into local map
        public void StitchBlockLight (IBoundedLitBlockCollection chunk, BlockCollectionEdge edge)
        {
            if (chunk.XDim != _blockset.XDim ||
                chunk.YDim != _blockset.YDim ||
                chunk.ZDim != _blockset.ZDim) {
                    throw new InvalidOperationException("BlockLight must have same dimensions to be stitched");
            }

            switch (edge) {
                case BlockCollectionEdge.EAST:
                    for (int x = 0; x < _blockset.XDim; x++) {
                        for (int y = 0; y < _blockset.YDim; y++) {
                            TestBlockLight(chunk, x, y, 0, x, y, _blockset.ZDim - 1);
                        }
                    }
                    break;

                case BlockCollectionEdge.NORTH:
                    for (int z = 0; z < _blockset.ZDim; z++) {
                        for (int y = 0; y < _blockset.YDim; y++) {
                            TestBlockLight(chunk, 0, y, z, _blockset.XDim - 1, y, z);
                        }
                    }
                    break;

                case BlockCollectionEdge.WEST:
                    for (int x = 0; x < _blockset.XDim; x++) {
                        for (int y = 0; y < _blockset.YDim; y++) {
                            TestBlockLight(chunk, x, y, _blockset.ZDim - 1, x, y, 0);
                        }
                    }
                    break;

                case BlockCollectionEdge.SOUTH:
                    for (int z = 0; z < _blockset.ZDim; z++) {
                        for (int y = 0; y < _blockset.YDim; y++) {
                            TestBlockLight(chunk, _blockset.XDim - 1, y, z, 0, y, z);
                        }
                    }
                    break;
            }

            UpdateBlockLight();
        }

        public void StitchBlockSkyLight ()
        {
            IBoundedLitBlockCollection[,] map = LocalBlockLightMap();

            if (map[1, 0] != null) {
                StitchBlockSkyLight(map[1, 0], BlockCollectionEdge.EAST);
            }
            if (map[0, 1] != null) {
                StitchBlockSkyLight(map[0, 1], BlockCollectionEdge.NORTH);
            }
            if (map[1, 2] != null) {
                StitchBlockSkyLight(map[1, 2], BlockCollectionEdge.WEST);
            }
            if (map[2, 1] != null) {
                StitchBlockSkyLight(map[2, 1], BlockCollectionEdge.SOUTH);
            }
        }

        public void StitchBlockSkyLight (IBoundedLitBlockCollection chunk, BlockCollectionEdge edge)
        {
            if (chunk.XDim != _blockset.XDim ||
                chunk.YDim != _blockset.YDim ||
                chunk.ZDim != _blockset.ZDim) {
                throw new InvalidOperationException("BlockLight must have same dimensions to be stitched");
            }

            switch (edge) {
                case BlockCollectionEdge.EAST:
                    for (int x = 0; x < _blockset.XDim; x++) {
                        for (int y = 0; y < _blockset.YDim; y++) {
                            TestSkyLight(chunk, x, y, 0, x, y, _blockset.ZDim - 1);
                        }
                    }
                    break;

                case BlockCollectionEdge.NORTH:
                    for (int z = 0; z < _blockset.ZDim; z++) {
                        for (int y = 0; y < _blockset.YDim; y++) {
                            TestSkyLight(chunk, 0, y, z, _blockset.XDim - 1, y, z);
                        }
                    }
                    break;

                case BlockCollectionEdge.WEST:
                    for (int x = 0; x < _blockset.XDim; x++) {
                        for (int y = 0; y < _blockset.YDim; y++) {
                            TestSkyLight(chunk, x, y, _blockset.ZDim - 1, x, y, 0);
                        }
                    }
                    break;

                case BlockCollectionEdge.SOUTH:
                    for (int z = 0; z < _blockset.ZDim; z++) {
                        for (int y = 0; y < _blockset.YDim; y++) {
                            TestSkyLight(chunk, _blockset.XDim - 1, y, z, 0, y, z);
                        }
                    }
                    break;
            }

            UpdateBlockSkyLight();
        }


        private void UpdateBlockLight ()
        {
            IBoundedLitBlockCollection[,] chunkMap = LocalBlockLightMap();

            while (_update.Count > 0) {
                BlockKey k = _update.Dequeue();
                int index = LightBitmapIndex(k);
                _lightbit[index] = false;

                int xi = k.x + _blockset.XDim;
                int zi = k.z + _blockset.ZDim;

                IBoundedLitBlockCollection cc = chunkMap[xi / _blockset.XDim, zi / _blockset.ZDim];
                if (cc == null) {
                    continue;
                }

                int lle = NeighborLight(chunkMap, k.x, k.y, k.z - 1);
                int lln = NeighborLight(chunkMap, k.x - 1, k.y, k.z);
                int lls = NeighborLight(chunkMap, k.x, k.y, k.z + 1);
                int llw = NeighborLight(chunkMap, k.x + 1, k.y, k.z);
                int lld = NeighborLight(chunkMap, k.x, k.y - 1, k.z);
                int llu = NeighborLight(chunkMap, k.x, k.y + 1, k.z);

                int x = xi % _blockset.XDim;
                int y = k.y;
                int z = zi % _blockset.ZDim;

                int lightval = cc.GetBlockLight(x, y, z);
                BlockInfo info = cc.GetInfo(x, y, z);

                int light = Math.Max(info.Luminance, 0);
                light = Math.Max(light, lle);
                light = Math.Max(light, lln);
                light = Math.Max(light, lls);
                light = Math.Max(light, llw);
                light = Math.Max(light, lld);
                light = Math.Max(light, llu);

                light = Math.Max(light - info.Opacity, 0);

                if (light != lightval) {
                    //Console.WriteLine("Block Light: ({0},{1},{2}) " + lightval + " -> " + light, k.x, k.y, k.z);

                    cc.SetBlockLight(x, y, z, light);

                    if (info.TransmitsLight) {
                        QueueRelight(new BlockKey(k.x - 1, k.y, k.z));
                        QueueRelight(new BlockKey(k.x + 1, k.y, k.z));
                        QueueRelight(new BlockKey(k.x, k.y - 1, k.z));
                        QueueRelight(new BlockKey(k.x, k.y + 1, k.z));
                        QueueRelight(new BlockKey(k.x, k.y, k.z - 1));
                        QueueRelight(new BlockKey(k.x, k.y, k.z + 1));
                    }
                }
            }
        }

        private void UpdateBlockSkyLight ()
        {
            IBoundedLitBlockCollection[,] chunkMap = LocalBlockLightMap();

            while (_update.Count > 0) {
                BlockKey k = _update.Dequeue();
                int index = LightBitmapIndex(k);
                _lightbit[index] = false;

                int xi = k.x + _blockset.XDim;
                int zi = k.z + _blockset.ZDim;

                IBoundedLitBlockCollection cc = chunkMap[xi / _blockset.XDim, zi / _blockset.ZDim];
                if (cc == null) {
                    continue;
                }

                int x = xi % _blockset.XDim;
                int y = k.y;
                int z = zi % _blockset.ZDim;

                int lightval = cc.GetSkyLight(x, y, z);
                BlockInfo info = cc.GetInfo(x, y, z);

                int light = BlockInfo.MIN_LUMINANCE;

                if (cc.GetHeight(x, z) <= y) {
                    light = BlockInfo.MAX_LUMINANCE;
                }
                else {
                    int lle = NeighborSkyLight(chunkMap, k.x, k.y, k.z - 1);
                    int lln = NeighborSkyLight(chunkMap, k.x - 1, k.y, k.z);
                    int lls = NeighborSkyLight(chunkMap, k.x, k.y, k.z + 1);
                    int llw = NeighborSkyLight(chunkMap, k.x + 1, k.y, k.z);
                    int lld = NeighborSkyLight(chunkMap, k.x, k.y - 1, k.z);
                    int llu = NeighborSkyLight(chunkMap, k.x, k.y + 1, k.z);

                    light = Math.Max(light, lle);
                    light = Math.Max(light, lln);
                    light = Math.Max(light, lls);
                    light = Math.Max(light, llw);
                    light = Math.Max(light, lld);
                    light = Math.Max(light, llu);
                }

                light = Math.Max(light - info.Opacity, 0);

                if (light != lightval) {
                    //Console.WriteLine("Block SkyLight: ({0},{1},{2}) " + lightval + " -> " + light, k.x, k.y, k.z);

                    cc.SetSkyLight(x, y, z, light);

                    if (info.TransmitsLight) {
                        QueueRelight(new BlockKey(k.x - 1, k.y, k.z));
                        QueueRelight(new BlockKey(k.x + 1, k.y, k.z));
                        QueueRelight(new BlockKey(k.x, k.y - 1, k.z));
                        QueueRelight(new BlockKey(k.x, k.y + 1, k.z));
                        QueueRelight(new BlockKey(k.x, k.y, k.z - 1));
                        QueueRelight(new BlockKey(k.x, k.y, k.z + 1));
                    }
                }
            }
        }

        private void SpreadBlockLight (IBoundedLitBlockCollection[,] chunkMap, int lx, int ly, int lz)
        {
            BlockInfo primary = _blockset.GetInfo(lx, ly, lz);
            int primaryLight = _blockset.GetBlockLight(lx, ly, lz);
            int priLum = Math.Max(primary.Luminance - primary.Opacity, 0);

            if (primaryLight < priLum) {
               _blockset.SetBlockLight(lx, ly, lz, priLum);
            }

            if (primaryLight > primary.Luminance - 1 && !primary.TransmitsLight) {
                return;
            }

            int xdim = _blockset.XDim;
            int ydim = _blockset.YDim;
            int zdim = _blockset.ZDim;

            Queue<LightRecord> spread = new Queue<LightRecord>();
            if (ly > 0) {
                spread.Enqueue(new LightRecord(lx, ly - 1, lz, primary.Luminance - 1));
            }
            if (ly < ydim - 1) {
                spread.Enqueue(new LightRecord(lx, ly + 1, lz, primary.Luminance - 1));
            }

            spread.Enqueue(new LightRecord(lx - 1, ly, lz, primary.Luminance - 1));
            spread.Enqueue(new LightRecord(lx + 1, ly, lz, primary.Luminance - 1));
            spread.Enqueue(new LightRecord(lx, ly, lz - 1, primary.Luminance - 1));
            spread.Enqueue(new LightRecord(lx, ly, lz + 1, primary.Luminance - 1));

            while (spread.Count > 0) {
                LightRecord rec = spread.Dequeue();

                int xi = rec.x + xdim;
                int zi = rec.z + zdim;

                IBoundedLitBlockCollection cc = chunkMap[xi / xdim, zi / zdim];
                if (cc == null) {
                    continue;
                }

                int x = xi % xdim;
                int y = rec.y;
                int z = zi % zdim;

                BlockInfo info = cc.GetInfo(x, y, z);
                int light = cc.GetBlockLight(x, y, z);

                int dimStr = Math.Max(rec.str - info.Opacity, 0);

                if (dimStr > light) {
                    cc.SetBlockLight(x, y, z, dimStr);

                    if (info.TransmitsLight) {
                        int strength = (info.Opacity > 0) ? dimStr : dimStr - 1;

                        if (rec.y > 0) {
                            spread.Enqueue(new LightRecord(rec.x, rec.y - 1, rec.z, strength));
                        }
                        if (rec.y < ydim - 1) {
                            spread.Enqueue(new LightRecord(rec.x, rec.y + 1, rec.z, strength));
                        }

                        spread.Enqueue(new LightRecord(rec.x - 1, rec.y, rec.z, strength));
                        spread.Enqueue(new LightRecord(rec.x + 1, rec.y, rec.z, strength));
                        spread.Enqueue(new LightRecord(rec.x, rec.y, rec.z - 1, strength));
                        spread.Enqueue(new LightRecord(rec.x, rec.y, rec.z + 1, strength));
                    }
                }
            }
        }

        private void SpreadSkyLight (IBoundedLitBlockCollection[,] chunkMap, int[,] heightMap, int lx, int ly, int lz)
        {
            BlockInfo primary = _blockset.GetInfo(lx, ly, lz);
            int primaryLight = _blockset.GetSkyLight(lx, ly, lz);
            int priLum = Math.Max(BlockInfo.MAX_LUMINANCE - primary.Opacity, 0);

            if (primaryLight < priLum) {
                _blockset.SetSkyLight(lx, ly, lz, priLum);
            }

            if (primaryLight > BlockInfo.MAX_LUMINANCE - 1 || !primary.TransmitsLight) {
                return;
            }

            Queue<LightRecord> spread = new Queue<LightRecord>();

            int xdim = _blockset.XDim;
            int ydim = _blockset.YDim;
            int zdim = _blockset.ZDim;

            int lxi = lx + xdim;
            int lzi = lz + zdim;

            int strength = (primary.Opacity > 0) ? priLum : priLum - 1;

            if (ly > 0) {
                if (heightMap[lxi, lzi] > ly - 1) {
                    spread.Enqueue(new LightRecord(lx, ly - 1, lz, strength));
                }
                else {
                    spread.Enqueue(new LightRecord(lx, ly - 1, lz, priLum));
                }
            }

            if (ly < ydim - 1) {
                if (heightMap[lxi, lzi] > ly + 1) {
                    spread.Enqueue(new LightRecord(lx, ly + 1, lz, strength));
                }
            }

            if (heightMap[lxi - 1, lzi] > ly) {
                spread.Enqueue(new LightRecord(lx - 1, ly, lz, strength));
            }
            if (heightMap[lxi + 1, lzi] > ly) {
                spread.Enqueue(new LightRecord(lx + 1, ly, lz, strength));
            }
            if (heightMap[lxi, lzi - 1] > ly) {
                spread.Enqueue(new LightRecord(lx, ly, lz - 1, strength));
            }
            if (heightMap[lxi, lzi + 1] > ly) {
                spread.Enqueue(new LightRecord(lx, ly, lz + 1, strength));
            }

            while (spread.Count > 0) {
                LightRecord rec = spread.Dequeue();

                int xi = rec.x + xdim;
                int zi = rec.z + zdim;

                IBoundedLitBlockCollection cc = chunkMap[xi / xdim, zi / zdim];
                if (cc == null) {
                    continue;
                }

                int x = xi % xdim;
                int y = rec.y;
                int z = zi % zdim;

                BlockInfo info = cc.GetInfo(x, y, z);
                int light = cc.GetSkyLight(x, y, z);

                int dimStr = Math.Max(rec.str - info.Opacity, 0);

                if (dimStr > light) {
                    cc.SetSkyLight(x, y, z, dimStr);

                    if (info.TransmitsLight) {
                        strength = (info.Opacity > 0) ? dimStr : dimStr - 1;

                        if (rec.y > 0) {
                            if (heightMap[xi, zi] > rec.y - 1) {
                                spread.Enqueue(new LightRecord(rec.x, rec.y - 1, rec.z, strength));
                            }
                            else {
                                spread.Enqueue(new LightRecord(rec.x, rec.y - 1, rec.z, dimStr));
                            }
                        }

                        if (rec.y < ydim - 1) {
                            if (heightMap[xi, zi] > rec.y + 1) {
                                spread.Enqueue(new LightRecord(rec.x, rec.y + 1, rec.z, strength));
                            }
                        }

                        if (heightMap[xi - 1, zi] > rec.y) {
                            spread.Enqueue(new LightRecord(rec.x - 1, rec.y, rec.z, strength));
                        }
                        if (heightMap[xi + 1, zi] > rec.y) {
                            spread.Enqueue(new LightRecord(rec.x + 1, rec.y, rec.z, strength));
                        }
                        if (heightMap[xi, zi - 1] > rec.y) {
                            spread.Enqueue(new LightRecord(rec.x, rec.y, rec.z - 1, strength));
                        }
                        if (heightMap[xi, zi + 1] > rec.y) {
                            spread.Enqueue(new LightRecord(rec.x, rec.y, rec.z + 1, strength));
                        }
                    }
                }
            }
        }


        private int LightBitmapIndex (BlockKey key)
        {
            int x = key.x + _blockset.XDim;
            int y = key.y;
            int z = key.z + _blockset.ZDim;

            int zstride = _blockset.YDim;
            int xstride = _blockset.ZDim * 3 * zstride;

            return (x * xstride) + (z * zstride) + y;
        }

        private void QueueRelight (BlockKey key)
        {
            if (key.x < -15 || key.x >= 31 || key.z < -15 || key.z >= 31) {
                return;
            }

            int index = LightBitmapIndex(key);

            if (!_lightbit[index]) {
                _lightbit[index] = true;
                _update.Enqueue(key);
            }
        }


        private IBoundedLitBlockCollection LocalChunk (int lx, int ly, int lz)
        {
            if (ly < 0 || ly >= _blockset.YDim) {
                return null;
            }

            if (lx < 0) {
                if (lz < 0) {
                    return OnResolveNeighbor(-1, 0, -1);
                }
                else if (lz >= _blockset.ZDim) {
                    return OnResolveNeighbor(-1, 0, 1);
                }
                return OnResolveNeighbor(-1, 0, 0);
            }
            else if (lx >= _blockset.XDim) {
                if (lz < 0) {
                    return OnResolveNeighbor(1, 0, -1);
                }
                else if (lz >= _blockset.ZDim) {
                    return OnResolveNeighbor(1, 0, 1);
                }
                return OnResolveNeighbor(1, 0, 0);
            }
            else {
                if (lz < 0) {
                    return OnResolveNeighbor(0, 0, -1);
                }
                else if (lz >= _blockset.ZDim) {
                    return OnResolveNeighbor(0, 0, 1);
                }
                return _blockset;
            }
        }

        private int NeighborLight (IBoundedLitBlockCollection[,] chunkMap, int x, int y, int z)
        {
            if (y < 0 || y >= _blockset.YDim) {
                return 0;
            }

            int xi = x + _blockset.XDim;
            int zi = z + _blockset.ZDim;

            IBoundedLitBlockCollection src = chunkMap[xi / _blockset.XDim, zi / _blockset.ZDim];
            if (src == null) {
                return 0;
            }

            x = xi % _blockset.XDim;
            z = zi % _blockset.ZDim;

            BlockInfo info = src.GetInfo(x, y, z);
            if (!info.TransmitsLight) {
                return info.Luminance;
            }

            int light = src.GetBlockLight(x, y, z);

            return Math.Max((info.Opacity > 0) ? light : light - 1, info.Luminance - 1);
        }

        private int NeighborSkyLight (IBoundedLitBlockCollection[,] chunkMap, int x, int y, int z)
        {
            if (y < 0 || y >= _blockset.YDim) {
                return 0;
            }

            int xi = x + _blockset.XDim;
            int zi = z + _blockset.ZDim;

            IBoundedLitBlockCollection src = chunkMap[xi / _blockset.XDim, zi / _blockset.ZDim];
            if (src == null) {
                return 0;
            }

            x = xi % _blockset.XDim;
            z = zi % _blockset.ZDim;

            BlockInfo info = src.GetInfo(x, y, z);
            if (!info.TransmitsLight) {
                return BlockInfo.MIN_LUMINANCE;
            }

            int light = src.GetSkyLight(x, y, z);

            return (info.Opacity > 0) ? light : light - 1;
        }

        private int NeighborHeight (int x, int z)
        {
            IBoundedLitBlockCollection src = LocalChunk(x, 0, z);
            if (src == null) {
                return _blockset.YDim - 1;
            }

            x = (x + _blockset.XDim * 2) % _blockset.XDim;
            z = (z + _blockset.ZDim * 2) % _blockset.ZDim;

            return src.GetHeight(x, z);
        }


        private void TestBlockLight (IBoundedLitBlockCollection chunk, int x1, int y1, int z1, int x2, int y2, int z2)
        {
            int light1 = _blockset.GetBlockLight(x1, y1, z1);
            int light2 = chunk.GetBlockLight(x2, y2, z2);
            int lum1 = _blockset.GetInfo(x1, y1, z1).Luminance;
            int lum2 = chunk.GetInfo(x2, y2, z2).Luminance;

            int v1 = Math.Max(light1, lum1);
            int v2 = Math.Max(light2, lum2);
            if (Math.Abs(v1 - v2) > 1) {
                QueueRelight(new BlockKey(x1, y1, z1));
            }
        }

        private void TestSkyLight (IBoundedLitBlockCollection chunk, int x1, int y1, int z1, int x2, int y2, int z2)
        {
            int light1 = _blockset.GetSkyLight(x1, y1, z1);
            int light2 = chunk.GetSkyLight(x2, y2, z2);

            if (Math.Abs(light1 - light2) > 1) {
                QueueRelight(new BlockKey(x1, y1, z1));
            }
        }


        private IBoundedLitBlockCollection[,] LocalBlockLightMap ()
        {
            IBoundedLitBlockCollection[,] map = new IBoundedLitBlockCollection[3, 3];

            map[0, 0] = OnResolveNeighbor(-1, 0, -1);
            map[0, 1] = OnResolveNeighbor(-1, 0, 0);
            map[0, 2] = OnResolveNeighbor(-1, 0, 1);
            map[1, 0] = OnResolveNeighbor(0, 0, -1);
            map[1, 1] = _blockset;
            map[1, 2] = OnResolveNeighbor(0, 0, 1);
            map[2, 0] = OnResolveNeighbor(1, 0, -1);
            map[2, 1] = OnResolveNeighbor(1, 0, 0);
            map[2, 2] = OnResolveNeighbor(1, 0, 1);

            return map;
        }

        private int[,] LocalHeightMap (IBoundedLitBlockCollection[,] chunkMap)
        {
            int[,] map = new int[3 * _blockset.XDim, 3 * _blockset.ZDim];

            for (int xi = 0; xi < 3; xi++) {
                int xoff = xi * _blockset.XDim;
                for (int zi = 0; zi < 3; zi++) {
                    int zoff = zi * _blockset.ZDim;
                    if (chunkMap[xi, zi] == null) {
                        continue;
                    }

                    for (int x = 0; x < _blockset.XDim; x++) {
                        int xx = xoff + x;
                        for (int z = 0; z < _blockset.ZDim; z++) {
                            int zz = zoff + z;
                            map[xx, zz] = chunkMap[xi, zi].GetHeight(x, z);
                        }
                    }
                }
            }

            return map;
        }


        private IBoundedLitBlockCollection OnResolveNeighbor (int relX, int relY, int relZ)
        {
            if (ResolveNeighbor != null) {
                IBoundedLitBlockCollection n = ResolveNeighbor(relX, relY, relZ);

                if (n == null) {
                    return null;
                }

                if (n.XDim != _blockset.XDim ||
                    n.YDim != _blockset.YDim ||
                    n.ZDim != _blockset.ZDim) {
                        throw new InvalidOperationException("Subscriber returned incompatible ILitBlockCollection");
                }

                return n;
            }

            return null;
        }

        
    }
}
