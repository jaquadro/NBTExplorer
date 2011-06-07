using System;
using System.Collections.Generic;

namespace Substrate
{
    using Utility;

    public class BlockFluid
    {
        private IBoundedDataBlockCollection _blockset;

        private readonly int _xdim;
        private readonly int _ydim;
        private readonly int _zdim;

        private Dictionary<ChunkKey, IBoundedDataBlockCollection> _chunks;

        public delegate IBoundedDataBlockCollection NeighborLookupHandler (int relx, int rely, int relz);

        public event NeighborLookupHandler ResolveNeighbor;

        internal class BlockCoord
        {
            internal IBoundedDataBlockCollection chunk;
            internal int lx;
            internal int ly;
            internal int lz;

            internal BlockCoord (IBoundedDataBlockCollection _chunk, int _lx, int _ly, int _lz)
            {
                chunk = _chunk;
                lx = _lx;
                ly = _ly;
                lz = _lz;
            }
        }

        public BlockFluid (IBoundedDataBlockCollection blockset)
        {
            _blockset = blockset;

            _xdim = _blockset.XDim;
            _ydim = _blockset.YDim;
            _zdim = _blockset.ZDim;

            _chunks = new Dictionary<ChunkKey,IBoundedDataBlockCollection>();
        }

        public BlockFluid (BlockFluid bl)
        {
            _blockset = bl._blockset;

            _xdim = bl._xdim;
            _ydim = bl._ydim;
            _zdim = bl._zdim;

            _chunks = new Dictionary<ChunkKey, IBoundedDataBlockCollection>();
        }

        public void ResetWater ()
        {
            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

            for (int x = 0; x < xdim; x++) {
                for (int z = 0; z < zdim; z++) {
                    for (int y = 0; y < ydim; y++) {
                        IDataBlock block = _blockset.GetBlockRef(x, y, z);
                        if ((block.ID == BlockType.STATIONARY_WATER || block.ID == BlockType.WATER) && block.Data != (int)WaterFlow.FULL) {
                            block.ID = BlockType.AIR;
                            block.Data = 0;
                        }
                        else if (block.ID == BlockType.WATER) {
                            block.ID = BlockType.STATIONARY_WATER;
                        }
                    }
                }
            }
        }

        public void RebuildWater ()
        {
            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

            List<BlockKey> buildQueue = new List<BlockKey>();

            for (int x = 0; x < xdim; x++) {
                for (int z = 0; z < zdim; z++) {
                    for (int y = 0; y < ydim; y++) {
                        BlockInfo info = _blockset.GetInfo(x, y, z);
                        if (info.ID == BlockType.STATIONARY_WATER && _blockset.GetData(x, y, z) == (int)WaterFlow.FULL) {
                            buildQueue.Add(new BlockKey(x, y, z));
                        }
                    }
                }
            }

            foreach (BlockKey key in buildQueue) {
                SpreadWater(key.x, key.y, key.z);
            }
        }

        private void SpreadWater (int x, int y, int z)
        {
            Queue<KeyValuePair<BlockKey, int>> targets = new Queue<KeyValuePair<BlockKey, int>>();
            targets.Enqueue(new KeyValuePair<BlockKey, int>(new BlockKey(x, y, z), 0));

            while (targets.Count > 0) {
                KeyValuePair<BlockKey, int> skey = targets.Dequeue();

                BlockKey source = skey.Key;
                int sourceDist = skey.Value;
 
                int nearestDrop = 7;
                bool foundDrop = false;

                Queue<BlockKey> searchQueue = new Queue<BlockKey>();
                Queue<KeyValuePair<BlockKey, int>> sinkQueue = new Queue<KeyValuePair<BlockKey, int>>();
                Dictionary<BlockKey, int> marked = new Dictionary<BlockKey, int>();

                searchQueue.Enqueue(source);
                marked.Add(source, sourceDist);

                // Identify sinks
                while (searchQueue.Count > 0) {
                    BlockKey branch = searchQueue.Dequeue();
                    int distance = marked[branch] & ~(int)LiquidState.FALLING;
                    bool falling = (marked[branch] & (int)LiquidState.FALLING) > 0;

                    // Ignore blocks out of range
                    if (distance > nearestDrop) {
                        continue;
                    }

                    // Ignore invalid blocks
                    BlockCoord branchHigh = TranslateCoord(branch.x, branch.y, branch.z);
                    if (branchHigh.chunk == null || branch.y == 0) {
                        marked.Remove(branch);
                        continue;
                    }

                    // If we're not the magical source block...
                    if (distance > 0 && !falling) {
                        // Ignore blocks that block fluid (and thus could not become a fluid)
                        BlockInfo branchHighInfo = branchHigh.chunk.GetInfo(branchHigh.lx, branchHigh.ly, branchHigh.lz);
                        if (branchHighInfo.BlocksFluid) {
                            marked.Remove(branch);
                            continue;
                        }

                        // Ignore blocks that have a "fuller" fluid level
                        if (branchHighInfo.ID == BlockType.STATIONARY_WATER) {
                            int cmp = branchHigh.chunk.GetData(branchHigh.lx, branchHigh.ly, branchHigh.lz);
                            if (cmp <= distance || (cmp & (int)LiquidState.FALLING) > 0) {
                                marked.Remove(branch);
                                continue;
                            }
                        }
                    }

                    // If we found a hole, add as a sink, mark the sink distance.
                    BlockCoord branchLow = TranslateCoord(branch.x, branch.y - 1, branch.z);
                    BlockInfo branchLowInfo = branchLow.chunk.GetInfo(branchLow.lx, branchLow.ly, branchLow.lz);
                    if (!branchLowInfo.BlocksFluid) {
                        foundDrop = true;
                        nearestDrop = falling ? 0 : distance;
                        sinkQueue.Enqueue(new KeyValuePair<BlockKey, int>(branch, falling ? distance | (int)LiquidState.FALLING : distance));
                        continue;
                    }

                    // Didn't find a hole, hit the bottom, reset distance
                    if (falling) {
                        distance = 0;
                    }

                    // Expand to neighbors
                    if (distance < nearestDrop) {
                        BlockKey[] keys = {
                            new BlockKey(branch.x - 1, branch.y, branch.z),
                            new BlockKey(branch.x + 1, branch.y, branch.z),
                            new BlockKey(branch.x, branch.y, branch.z - 1),
                            new BlockKey(branch.x, branch.y, branch.z + 1),
                        };
                        
                        for (int i = 0; i < 4; i++) {
                            if (!marked.ContainsKey(keys[i])) {
                                searchQueue.Enqueue(keys[i]);
                                marked.Add(keys[i], distance + 1);
                            }
                        }
                    }
                }

                // If we didn't find any drops, fill out the mark table
                if (!foundDrop) {
                    foreach (KeyValuePair<BlockKey, int> mark in marked) {
                        BlockKey target = mark.Key;

                        BlockCoord targetHigh = TranslateCoord(target.x, target.y, target.z);
                        targetHigh.chunk.SetID(targetHigh.lx, targetHigh.ly, targetHigh.lz, BlockType.STATIONARY_WATER);
                        targetHigh.chunk.SetData(targetHigh.lx, targetHigh.ly, targetHigh.lz, mark.Value);
                    }

                    // Skip sinks and tracing
                    continue;
                }

                // Create new target for each sink
                Queue<KeyValuePair<BlockKey, int>> traceQueue = new Queue<KeyValuePair<BlockKey, int>>();
                while (sinkQueue.Count > 0) {
                    KeyValuePair<BlockKey, int> tkey = sinkQueue.Dequeue();
                    BlockKey target = tkey.Key;
                    int distance = tkey.Value;

                    //int distance = marked[target];
                    marked.Remove(target);

                    targets.Enqueue(new KeyValuePair<BlockKey, int>(new BlockKey(target.x, target.y - 1, target.z), distance | (int)LiquidState.FALLING));
                    traceQueue.Enqueue(new KeyValuePair<BlockKey, int>(target, distance));

                    /*BlockCoord targetLow = TranslateCoord(target.x, target.y - 1, target.z);
                    targetLow.chunk.SetID(targetLow.lx, targetLow.ly, targetHigh.lz, BlockType.STATIONARY_WATER);
                    targetLow.chunk.SetData(targetLow.lx, targetLow.ly, targetHigh.lz, tkey.Value);*/
                }

                // Trace back from sinks
                while (traceQueue.Count > 0) {
                    KeyValuePair<BlockKey, int> tkey = traceQueue.Dequeue();

                    BlockKey target = tkey.Key;
                    int distance = tkey.Value & ~(int)LiquidState.FALLING;
                    bool falling = (tkey.Value & (int)LiquidState.FALLING) > 0;

                    //if (distance == 0) {
                    //   distance |= (int)LiquidState.FALLING;
                    //}

                    BlockCoord targetHigh = TranslateCoord(target.x, target.y, target.z);
                    targetHigh.chunk.SetID(targetHigh.lx, targetHigh.ly, targetHigh.lz, BlockType.STATIONARY_WATER);
                    targetHigh.chunk.SetData(targetHigh.lx, targetHigh.ly, targetHigh.lz, tkey.Value);

                    if (falling) {
                        continue;
                    }

                    BlockKey[] keys = {
                        new BlockKey(target.x - 1, target.y, target.z),
                        new BlockKey(target.x + 1, target.y, target.z),
                        new BlockKey(target.x, target.y, target.z - 1),
                        new BlockKey(target.x, target.y, target.z + 1),
                    };

                    for (int i = 0; i < 4; i++) {
                        int nval;
                        if (!marked.TryGetValue(keys[i], out nval)) {
                            continue;
                        }

                        int ndist = nval & ~(int)LiquidState.FALLING;
                        bool nfall = (nval & (int)LiquidState.FALLING) > 0;

                        marked.Remove(keys[i]);
                        if (ndist < distance || nfall) {
                            traceQueue.Enqueue(new KeyValuePair<BlockKey, int>(keys[i], nval));
                        }
                    }
                }
            }
        }

        private BlockCoord TranslateCoord (int x, int y, int z)
        {
            IBoundedDataBlockCollection chunk = GetChunk(x, z);

            int lx = ((x % _xdim) + _xdim) % _xdim;
            int lz = ((z % _zdim) + _zdim) % _zdim;

            return new BlockCoord(chunk, lx, y, lz);
        }

        private IBoundedDataBlockCollection GetChunk (int x, int z)
        {
            int cx = x / _xdim + (x >> 31);
            int cz = z / _zdim + (z >> 31);

            ChunkKey key = new ChunkKey(cx, cz);

            IBoundedDataBlockCollection chunk;
            if (!_chunks.TryGetValue(key, out chunk)) {
                chunk = OnResolveNeighbor(cx, 0, cz);
                _chunks[key] = chunk;
            }

            return chunk;
        }

        private IBoundedDataBlockCollection OnResolveNeighbor (int relX, int relY, int relZ)
        {
            if (ResolveNeighbor != null) {
                IBoundedDataBlockCollection n = ResolveNeighbor(relX, relY, relZ);

                if (n == null) {
                    return null;
                }

                if (n.XDim != _xdim ||
                    n.YDim != _ydim ||
                    n.ZDim != _zdim) {
                        throw new InvalidOperationException("Subscriber returned incompatible IDataBlockCollection");
                }

                return n;
            }

            return null;
        }
    }
}
