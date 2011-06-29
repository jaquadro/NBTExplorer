using System;
using System.Collections;
using System.Collections.Generic;

namespace Substrate.Core
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

        private readonly int _xdim;
        private readonly int _ydim;
        private readonly int _zdim;

        private IBoundedLitBlockCollection _blockset;

        // Maintains internal state of multi-block relighting algorithms
        private BitArray _lightbit;
        private Queue<BlockKey> _update;

        public delegate IBoundedLitBlockCollection NeighborLookupHandler (int relx, int rely, int relz);

        public event NeighborLookupHandler ResolveNeighbor;

        public BlockLight (IBoundedLitBlockCollection blockset)
        {
            _blockset = blockset;

            _xdim = _blockset.XDim;
            _ydim = _blockset.YDim;
            _zdim = _blockset.ZDim;

            _lightbit = new BitArray(_blockset.XDim * 3 * _blockset.ZDim * 3 * _blockset.YDim);
            _update = new Queue<BlockKey>();
        }

        public BlockLight (BlockLight bl)
        {
            _blockset = bl._blockset;

            _xdim = bl._xdim;
            _ydim = bl._ydim;
            _zdim = bl._zdim;

            _lightbit = new BitArray(_blockset.XDim * 3 * _blockset.ZDim * 3 * _blockset.YDim);
            _update = new Queue<BlockKey>();
        }

        public void UpdateBlockLight (int lx, int ly, int lz)
        {
            BlockKey primary = new BlockKey(lx, ly, lz);
            _update.Enqueue(primary);

            //BlockInfo info = _blockset.GetInfo(lx, ly, lz);

            //if (info.Luminance > BlockInfo.MIN_LUMINANCE || info.TransmitsLight) {
            if (ly > 0) {
                QueueRelight(new BlockKey(lx, ly - 1, lz));
            }
            if (ly < _ydim - 1) {
                QueueRelight(new BlockKey(lx, ly + 1, lz));
            }

            QueueRelight(new BlockKey(lx - 1, ly, lz));
            QueueRelight(new BlockKey(lx + 1, ly, lz));
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

        public void UpdateHeightMap (int lx, int ly, int lz)
        {
            BlockInfo info = _blockset.GetInfo(lx, ly, lz);
            int h = Math.Min(ly + 1, _ydim - 1);

            int height = _blockset.GetHeight(lx, lz);
            if (h < height) {
                return;
            }

            if (h == height && !info.ObscuresLight) {
                for (int i = ly - 1; i >= 0; i--) {
                    BlockInfo info2 = _blockset.GetInfo(lx, i, lz);
                    if (info2.ObscuresLight) {
                        _blockset.SetHeight(lx, lz, Math.Min(i + 1, _ydim - 1));
                        break;
                    }
                }
                UpdateBlockSkyLight(lx, h, lz);
            }
            else if (h > height && info.ObscuresLight) {
                _blockset.SetHeight(lx, lz, h);
                UpdateBlockSkyLight(lx, h, lz);
            }
        }


        public void RebuildBlockLight ()
        {
            IBoundedLitBlockCollection[,] chunkMap = LocalBlockLightMap();

            // Because the JIT is less intelligent than I hoped
            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

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

            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

            // Optimization - only need to queue at level of highest neighbor's height
            for (int x = 0; x < xdim; x++) {
                for (int z = 0; z < zdim; z++) {
                    int xi = x + xdim;
                    int zi = z + zdim;

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
            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

            for (int x = 0; x < xdim; x++) {
                for (int z = 0; z < zdim; z++) {
                    for (int y = ydim - 1; y >= 0; y--) {
                        BlockInfo info = _blockset.GetInfo(x, y, z);
                        if (info.ObscuresLight) {
                            _blockset.SetHeight(x, z, Math.Min(y + 1, ydim - 1));
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
            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

            if (chunk.XDim != xdim ||
                chunk.YDim != ydim ||
                chunk.ZDim != zdim) {
                    throw new InvalidOperationException("BlockLight must have same dimensions to be stitched");
            }

            switch (edge) {
                case BlockCollectionEdge.EAST:
                    for (int x = 0; x < xdim; x++) {
                        for (int y = 0; y < ydim; y++) {
                            TestBlockLight(chunk, x, y, 0, x, y, zdim - 1);
                        }
                    }
                    break;

                case BlockCollectionEdge.NORTH:
                    for (int z = 0; z < zdim; z++) {
                        for (int y = 0; y < ydim; y++) {
                            TestBlockLight(chunk, 0, y, z, xdim - 1, y, z);
                        }
                    }
                    break;

                case BlockCollectionEdge.WEST:
                    for (int x = 0; x < xdim; x++) {
                        for (int y = 0; y < ydim; y++) {
                            TestBlockLight(chunk, x, y, zdim - 1, x, y, 0);
                        }
                    }
                    break;

                case BlockCollectionEdge.SOUTH:
                    for (int z = 0; z < zdim; z++) {
                        for (int y = 0; y < ydim; y++) {
                            TestBlockLight(chunk, xdim - 1, y, z, 0, y, z);
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
            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

            if (chunk.XDim != xdim ||
                chunk.YDim != ydim ||
                chunk.ZDim != zdim) {
                throw new InvalidOperationException("BlockLight must have same dimensions to be stitched");
            }

            switch (edge) {
                case BlockCollectionEdge.EAST:
                    for (int x = 0; x < xdim; x++) {
                        for (int y = 0; y < ydim; y++) {
                            TestSkyLight(chunk, x, y, 0, x, y, zdim - 1);
                        }
                    }
                    break;

                case BlockCollectionEdge.NORTH:
                    for (int z = 0; z < zdim; z++) {
                        for (int y = 0; y < ydim; y++) {
                            TestSkyLight(chunk, 0, y, z, xdim - 1, y, z);
                        }
                    }
                    break;

                case BlockCollectionEdge.WEST:
                    for (int x = 0; x < xdim; x++) {
                        for (int y = 0; y < ydim; y++) {
                            TestSkyLight(chunk, x, y, zdim - 1, x, y, 0);
                        }
                    }
                    break;

                case BlockCollectionEdge.SOUTH:
                    for (int z = 0; z < zdim; z++) {
                        for (int y = 0; y < ydim; y++) {
                            TestSkyLight(chunk, xdim - 1, y, z, 0, y, z);
                        }
                    }
                    break;
            }

            UpdateBlockSkyLight();
        }


        private void UpdateBlockLight ()
        {
            IBoundedLitBlockCollection[,] chunkMap = LocalBlockLightMap();

            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

            while (_update.Count > 0) {
                BlockKey k = _update.Dequeue();
                int index = LightBitmapIndex(k);
                _lightbit[index] = false;

                int xi = k.x + xdim;
                int zi = k.z + zdim;

                IBoundedLitBlockCollection cc = chunkMap[xi / xdim, zi / zdim];
                if (cc == null) {
                    continue;
                }

                int lle = NeighborLight(chunkMap, k.x, k.y, k.z - 1);
                int lln = NeighborLight(chunkMap, k.x - 1, k.y, k.z);
                int lls = NeighborLight(chunkMap, k.x, k.y, k.z + 1);
                int llw = NeighborLight(chunkMap, k.x + 1, k.y, k.z);
                int lld = NeighborLight(chunkMap, k.x, k.y - 1, k.z);
                int llu = NeighborLight(chunkMap, k.x, k.y + 1, k.z);

                int x = xi % xdim;
                int y = k.y;
                int z = zi % zdim;

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
                        if (k.y > 0) {
                            QueueRelight(new BlockKey(k.x, k.y - 1, k.z));
                        }
                        if (k.y < ydim - 1) {
                            QueueRelight(new BlockKey(k.x, k.y + 1, k.z));
                        }

                        QueueRelight(new BlockKey(k.x - 1, k.y, k.z));
                        QueueRelight(new BlockKey(k.x + 1, k.y, k.z));
                        QueueRelight(new BlockKey(k.x, k.y, k.z - 1));
                        QueueRelight(new BlockKey(k.x, k.y, k.z + 1));
                    }
                }
            }
        }

        private void UpdateBlockSkyLight ()
        {
            IBoundedLitBlockCollection[,] chunkMap = LocalBlockLightMap();

            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

            while (_update.Count > 0) {
                BlockKey k = _update.Dequeue();
                int index = LightBitmapIndex(k);
                _lightbit[index] = false;

                int xi = k.x + xdim;
                int zi = k.z + zdim;

                IBoundedLitBlockCollection cc = chunkMap[xi / xdim, zi / zdim];
                if (cc == null) {
                    continue;
                }

                int x = xi % xdim;
                int y = k.y;
                int z = zi % zdim;

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
                        if (k.y > 0) {
                            QueueRelight(new BlockKey(k.x, k.y - 1, k.z));
                        }
                        if (k.y < ydim - 1) {
                            QueueRelight(new BlockKey(k.x, k.y + 1, k.z));
                        }

                        QueueRelight(new BlockKey(k.x - 1, k.y, k.z));
                        QueueRelight(new BlockKey(k.x + 1, k.y, k.z));
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

            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

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

            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

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
            int x = key.x + _xdim;
            int y = key.y;
            int z = key.z + _zdim;

            int zstride = _ydim;
            int xstride = _zdim * 3 * zstride;

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
            if (ly < 0 || ly >= _ydim) {
                return null;
            }

            if (lx < 0) {
                if (lz < 0) {
                    return OnResolveNeighbor(-1, 0, -1);
                }
                else if (lz >= _zdim) {
                    return OnResolveNeighbor(-1, 0, 1);
                }
                return OnResolveNeighbor(-1, 0, 0);
            }
            else if (lx >= _xdim) {
                if (lz < 0) {
                    return OnResolveNeighbor(1, 0, -1);
                }
                else if (lz >= _zdim) {
                    return OnResolveNeighbor(1, 0, 1);
                }
                return OnResolveNeighbor(1, 0, 0);
            }
            else {
                if (lz < 0) {
                    return OnResolveNeighbor(0, 0, -1);
                }
                else if (lz >= _zdim) {
                    return OnResolveNeighbor(0, 0, 1);
                }
                return _blockset;
            }
        }

        private int NeighborLight (IBoundedLitBlockCollection[,] chunkMap, int x, int y, int z)
        {
            if (y < 0 || y >= _ydim) {
                return 0;
            }

            int xdim = _xdim;
            int zdim = _zdim;

            int xi = x + xdim;
            int zi = z + zdim;

            IBoundedLitBlockCollection src = chunkMap[xi / xdim, zi / zdim];
            if (src == null) {
                return 0;
            }

            x = xi % xdim;
            z = zi % zdim;

            BlockInfo info = src.GetInfo(x, y, z);
            if (!info.TransmitsLight) {
                return info.Luminance;
            }

            int light = src.GetBlockLight(x, y, z);

            return Math.Max((info.Opacity > 0) ? light : light - 1, info.Luminance - 1);
        }

        private int NeighborSkyLight (IBoundedLitBlockCollection[,] chunkMap, int x, int y, int z)
        {
            if (y < 0 || y >= _ydim) {
                return 0;
            }

            int xdim = _xdim;
            int zdim = _zdim;

            int xi = x + xdim;
            int zi = z + zdim;

            IBoundedLitBlockCollection src = chunkMap[xi / xdim, zi / zdim];
            if (src == null) {
                return 0;
            }

            x = xi % xdim;
            z = zi % zdim;

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
                return _ydim - 1;
            }

            x = (x + _xdim * 2) % _xdim;
            z = (z + _zdim * 2) % _zdim;

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
            int xdim = _xdim;
            int zdim = _zdim;

            int[,] map = new int[3 * xdim, 3 * zdim];

            for (int xi = 0; xi < 3; xi++) {
                int xoff = xi * xdim;
                for (int zi = 0; zi < 3; zi++) {
                    int zoff = zi * zdim;
                    if (chunkMap[xi, zi] == null) {
                        continue;
                    }

                    for (int x = 0; x < xdim; x++) {
                        int xx = xoff + x;
                        for (int z = 0; z < zdim; z++) {
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

                if (n.XDim != _xdim ||
                    n.YDim != _ydim ||
                    n.ZDim != _zdim) {
                        throw new InvalidOperationException("Subscriber returned incompatible ILitBlockCollection");
                }

                return n;
            }

            return null;
        }

        
    }
}
