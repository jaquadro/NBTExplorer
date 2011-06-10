using System;
using System.Collections.Generic;

namespace Substrate
{
    using Utility;

    // Rules:
    // - Water must be calculated in steps breadth-first
    // - If there are any "holes" within 5 steps (manhattan distance) of a water tile, only the edges
    //   that can be part of a shortest path to the closest hole(s) are part of the outflow.
    // - Any blocks in the water tile's outflow are added to the queue
    // - A water source's strength is calculated as strongest inflow - 1.

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

        public void ResetWater (ByteArray blocks, NibbleArray data)
        {
            for (int i = 0; i < blocks.Length; i++) {
                if ((blocks[i] == BlockType.STATIONARY_WATER || blocks[i] == BlockType.WATER) && data[i] != 0) {
                    blocks[i] = BlockType.AIR;
                    data[i] = 0;
                }
                else if (blocks[i] == BlockType.WATER) {
                    blocks[i] = BlockType.STATIONARY_WATER;
                }
            }
        }

        public void ResetLava (ByteArray blocks, NibbleArray data)
        {
            for (int i = 0; i < blocks.Length; i++) {
                if ((blocks[i] == BlockType.STATIONARY_LAVA || blocks[i] == BlockType.LAVA) && data[i] != 0) {
                    blocks[i] = BlockType.AIR;
                    data[i] = 0;
                }
                else if (blocks[i] == BlockType.LAVA) {
                    blocks[i] = BlockType.STATIONARY_LAVA;
                }
            }
        }

        public void UpdateWater (int x, int y, int z)
        {
            DoWater(x, y, z);
            _chunks.Clear();
        }

        public void UpdateLava (int x, int y, int z)
        {
            DoLava(x, y, z);
            _chunks.Clear();
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
                        if (info.ID == BlockType.STATIONARY_WATER && _blockset.GetData(x, y, z) == 0) {
                            buildQueue.Add(new BlockKey(x, y, z));
                        }
                    }
                }
            }

            foreach (BlockKey key in buildQueue) {
                DoWater(key.x, key.y, key.z);
            }

            _chunks.Clear();
        }

        public void RebuildLava ()
        {
            int xdim = _xdim;
            int ydim = _ydim;
            int zdim = _zdim;

            List<BlockKey> buildQueue = new List<BlockKey>();

            for (int x = 0; x < xdim; x++) {
                for (int z = 0; z < zdim; z++) {
                    for (int y = 0; y < ydim; y++) {
                        BlockInfo info = _blockset.GetInfo(x, y, z);
                        if (info.ID == BlockType.STATIONARY_LAVA && _blockset.GetData(x, y, z) == 0) {
                            buildQueue.Add(new BlockKey(x, y, z));
                        }
                    }
                }
            }

            foreach (BlockKey key in buildQueue) {
                DoLava(key.x, key.y, key.z);
            }

            _chunks.Clear();
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

        // -----

        private List<BlockKey> TileOutflow (BlockKey key, int reach = 5)
        {
            Queue<BlockKey> searchQueue = new Queue<BlockKey>();
            Queue<KeyValuePair<BlockKey, int>> traceQueue = new Queue<KeyValuePair<BlockKey, int>>();
            Dictionary<BlockKey, int> markTable = new Dictionary<BlockKey,int>();

            searchQueue.Enqueue(key);
            markTable.Add(key, 0);

            // Identify sinks
            while (searchQueue.Count > 0) {
                BlockKey branch = searchQueue.Dequeue();
                int distance = markTable[branch];

                // Ignore blocks out of range
                if (distance > reach) {
                    continue;
                }

                // Ignore invalid blocks
                BlockCoord branchHigh = TranslateCoord(branch.x, branch.y, branch.z);
                if (branchHigh.chunk == null || branch.y == 0) {
                    markTable.Remove(branch);
                    continue;
                }

                // If we're not the magical source block...
                if (distance > 0) {
                    // Ignore blocks that block fluid (and thus could not become a fluid)
                    BlockInfo branchHighInfo = branchHigh.chunk.GetInfo(branchHigh.lx, branchHigh.ly, branchHigh.lz);
                    if (branchHighInfo.BlocksFluid) {
                        markTable.Remove(branch);
                        continue;
                    }
                }

                // If we found a hole, add as a sink, mark the sink distance.
                BlockCoord branchLow = TranslateCoord(branch.x, branch.y - 1, branch.z);
                BlockInfo branchLowInfo = branchLow.chunk.GetInfo(branchLow.lx, branchLow.ly, branchLow.lz);
                if (!branchLowInfo.BlocksFluid) {
                    // If we are our own sink, return the only legal outflow
                    if (key == branch) {
                        List<BlockKey> ret = new List<BlockKey>();
                        ret.Add(new BlockKey(branch.x, branch.y - 1, branch.z));
                        return ret;
                    }

                    reach = distance;
                    traceQueue.Enqueue(new KeyValuePair<BlockKey, int>(branch, distance));
                    continue;
                }

                // Expand to neighbors
                if (distance < reach) {
                    BlockKey[] keys = {
                        new BlockKey(branch.x - 1, branch.y, branch.z),
                        new BlockKey(branch.x + 1, branch.y, branch.z),
                        new BlockKey(branch.x, branch.y, branch.z - 1),
                        new BlockKey(branch.x, branch.y, branch.z + 1),
                    };

                    for (int i = 0; i < 4; i++) {
                        if (!markTable.ContainsKey(keys[i])) {
                            searchQueue.Enqueue(keys[i]);
                            markTable.Add(keys[i], distance + 1);
                        }
                    }
                }
            }

            // Candidate outflows are marked
            BlockKey[] neighbors = {
                new BlockKey(key.x - 1, key.y, key.z),
                new BlockKey(key.x + 1, key.y, key.z),
                new BlockKey(key.x, key.y, key.z - 1),
                new BlockKey(key.x, key.y, key.z + 1),
            };

            List<BlockKey> outflow = new List<BlockKey>();
            foreach (BlockKey n in neighbors) {
                if (markTable.ContainsKey(n)) {
                    outflow.Add(n);
                }
            }

            // If there's no sinks, all neighbors are valid outflows
            if (traceQueue.Count == 0) {
                return outflow;
            }

            // Trace back from each sink eliminating shortest path marks
            while (traceQueue.Count > 0) {

                KeyValuePair<BlockKey, int> tilekv = traceQueue.Dequeue();
                BlockKey tile = tilekv.Key;

                int distance = tilekv.Value;
                markTable.Remove(tile);

                BlockKey[] keys = {
                    new BlockKey(tile.x - 1, tile.y, tile.z),
                    new BlockKey(tile.x + 1, tile.y, tile.z),
                    new BlockKey(tile.x, tile.y, tile.z - 1),
                    new BlockKey(tile.x, tile.y, tile.z + 1),
                };

                for (int i = 0; i < 4; i++) {
                    int nval;
                    if (!markTable.TryGetValue(keys[i], out nval)) {
                        continue;
                    }

                    if (nval < distance) {
                        markTable.Remove(keys[i]);
                        traceQueue.Enqueue(new KeyValuePair<BlockKey, int>(keys[i], nval));
                    }
                }
            }

            // Remove any candidates that are still marked
            foreach (BlockKey n in neighbors) {
                if (markTable.ContainsKey(n)) {
                    outflow.Remove(n);
                }
            }

            return outflow;
        }

        private int TileInflow (BlockKey key)
        {
            // Check if water is falling on us
            if (key.y < _ydim - 1) {
                BlockCoord up = TranslateCoord(key.x, key.y + 1, key.z);
                BlockInfo upInfo = up.chunk.GetInfo(up.lx, up.ly, up.lz);

                if (upInfo.State == BlockState.FLUID) {
                    return up.chunk.GetData(up.lx, up.ly, up.lz) | (int)LiquidState.FALLING;
                }
            }

            // Otherwise return the min inflow of our neighbors + step
            BlockKey[] keys = {
                new BlockKey(key.x - 1, key.y, key.z),
                new BlockKey(key.x + 1, key.y, key.z),
                new BlockKey(key.x, key.y, key.z - 1),
                new BlockKey(key.x, key.y, key.z + 1),
            };

            int minFlow = 16;

            // XXX: Might have different neighboring fluids
            for (int i = 0; i < 4; i++) {
                BlockCoord neighbor = TranslateCoord(keys[i].x, keys[i].y, keys[i].z);
                if (neighbor.chunk == null) {
                    continue;
                }

                BlockInfo neighborInfo = neighbor.chunk.GetInfo(neighbor.lx, neighbor.ly, neighbor.lz);

                if (neighborInfo.State == BlockState.FLUID) {
                    int flow = neighbor.chunk.GetData(neighbor.lx, neighbor.ly, neighbor.lz);
                    bool flowFall = (flow & (int)LiquidState.FALLING) != 0;

                    if (flowFall) {
                        if (keys[i].y == 0) {
                            continue;
                        }

                        BlockCoord low = TranslateCoord(keys[i].x, keys[i].y - 1, keys[i].z);
                        BlockInfo lowinfo = low.chunk.GetInfo(low.lx, low.ly, low.lz);

                        if (lowinfo.BlocksFluid) {
                            return 0;
                        }
                        continue;
                    }
                    if (flow < minFlow) {
                        minFlow = flow;
                    }
                }
            }

            return minFlow;
        }

        private void DoWater (int x, int y, int z)
        {
            Queue<BlockKey> flowQueue = new Queue<BlockKey>();

            BlockKey prikey = new BlockKey(x, y, z);
            flowQueue.Enqueue(prikey);

            List<BlockKey> outflow = TileOutflow(prikey);
            foreach (BlockKey outkey in outflow) {
                flowQueue.Enqueue(outkey);
            }

            while (flowQueue.Count > 0) {
                BlockKey key = flowQueue.Dequeue();

                int curflow = 16;
                int inflow = TileInflow(key);

                BlockCoord tile = TranslateCoord(key.x, key.y, key.z);
                BlockInfo tileInfo = tile.chunk.GetInfo(tile.lx, tile.ly, tile.lz);
                if (tileInfo.ID == BlockType.STATIONARY_WATER || tileInfo.ID == BlockType.WATER) {
                    curflow = tile.chunk.GetData(tile.lx, tile.ly, tile.lz);
                }
                else if (tileInfo.BlocksFluid) {
                    continue;
                }

                bool curFall = (curflow & (int)LiquidState.FALLING) != 0;
                bool inFall = (inflow & (int)LiquidState.FALLING) != 0;

                // We won't update from the following states
                if (curflow == 0 || curflow == inflow || curFall) {
                    continue;
                }

                int newflow = curflow;

                // Update from inflow if necessary
                if (inFall) {
                    newflow = inflow;
                }
                else if (inflow >= 7) {
                    newflow = 16;
                }
                else {
                    newflow = inflow + 1;
                }

                // If we haven't changed the flow, don't propagate
                if (newflow == curflow) {
                    continue;
                }

                // Update flow, add or remove water tile as necessary
                if (newflow < 16 && curflow == 16) {
                    // If we're overwriting lava, replace with appropriate stone type and abort propagation
                    if (tileInfo.ID == BlockType.STATIONARY_LAVA || tileInfo.ID == BlockType.LAVA) {
                        if ((newflow & (int)LiquidState.FALLING) != 0) {
                            int odata = tile.chunk.GetData(tile.lx, tile.ly, tile.lz);
                            if (odata == 0) {
                                tile.chunk.SetID(tile.lx, tile.ly, tile.lz, BlockType.OBSIDIAN);
                            }
                            else {
                                tile.chunk.SetID(tile.lx, tile.ly, tile.lz, BlockType.COBBLESTONE);
                            }
                            tile.chunk.SetData(tile.lx, tile.ly, tile.lz, 0);
                            continue;
                        }
                    }

                    // Otherwise replace the tile with our water flow
                    tile.chunk.SetID(tile.lx, tile.ly, tile.lz, BlockType.STATIONARY_WATER);
                    tile.chunk.SetData(tile.lx, tile.ly, tile.lz, newflow);
                }
                else if (newflow == 16) {
                    tile.chunk.SetID(tile.lx, tile.ly, tile.lz, BlockType.AIR);
                    tile.chunk.SetData(tile.lx, tile.ly, tile.lz, 0);
                }
                else {
                    tile.chunk.SetData(tile.lx, tile.ly, tile.lz, newflow);
                }

                // Process outflows
                outflow = TileOutflow(key);

                foreach (BlockKey nkey in outflow) {
                    flowQueue.Enqueue(nkey);
                }
            }
        }

        private void DoLava (int x, int y, int z)
        {
            Queue<BlockKey> flowQueue = new Queue<BlockKey>();

            BlockKey prikey = new BlockKey(x, y, z);
            flowQueue.Enqueue(prikey);

            List<BlockKey> outflow = TileOutflow(prikey);
            foreach (BlockKey outkey in outflow) {
                flowQueue.Enqueue(outkey);
            }

            while (flowQueue.Count > 0) {
                BlockKey key = flowQueue.Dequeue();

                int curflow = 16;
                int inflow = TileInflow(key);

                BlockCoord tile = TranslateCoord(key.x, key.y, key.z);
                BlockInfo tileInfo = tile.chunk.GetInfo(tile.lx, tile.ly, tile.lz);
                if (tileInfo.ID == BlockType.STATIONARY_LAVA || tileInfo.ID == BlockType.LAVA) {
                    curflow = tile.chunk.GetData(tile.lx, tile.ly, tile.lz);
                }
                else if (tileInfo.BlocksFluid) {
                    continue;
                }

                bool curFall = (curflow & (int)LiquidState.FALLING) != 0;
                bool inFall = (inflow & (int)LiquidState.FALLING) != 0;

                // We won't update from the following states
                if (curflow == 0 || curflow == inflow || curFall) {
                    continue;
                }

                int newflow = curflow;

                // Update from inflow if necessary
                if (inFall) {
                    newflow = inflow;
                }
                else if (inflow >= 6) {
                    newflow = 16;
                }
                else {
                    newflow = inflow + 2;
                }

                // If we haven't changed the flow, don't propagate
                if (newflow == curflow) {
                    continue;
                }

                // Update flow, add or remove lava tile as necessary
                if (newflow < 16 && curflow == 16) {
                    // If we're overwriting water, replace with appropriate stone type and abort propagation
                    if (tileInfo.ID == BlockType.STATIONARY_WATER || tileInfo.ID == BlockType.WATER) {
                        if ((newflow & (int)LiquidState.FALLING) == 0) {
                            tile.chunk.SetID(tile.lx, tile.ly, tile.lz, BlockType.COBBLESTONE);
                            tile.chunk.SetData(tile.lx, tile.ly, tile.lz, 0);
                            continue;
                        }
                    }

                    tile.chunk.SetID(tile.lx, tile.ly, tile.lz, BlockType.STATIONARY_LAVA);
                    tile.chunk.SetData(tile.lx, tile.ly, tile.lz, newflow);
                }
                else if (newflow == 16) {
                    tile.chunk.SetID(tile.lx, tile.ly, tile.lz, BlockType.AIR);
                    tile.chunk.SetData(tile.lx, tile.ly, tile.lz, 0);
                }
                else {
                    tile.chunk.SetData(tile.lx, tile.ly, tile.lz, newflow);
                }

                // Process outflows
                outflow = TileOutflow(key);

                foreach (BlockKey nkey in outflow) {
                    flowQueue.Enqueue(nkey);
                }
            }
        }
    }
}
