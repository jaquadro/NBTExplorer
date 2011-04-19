using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Substrate
{
    using NBT;
    using System.Collections;

    public class ChunkRef : IChunk
    {
        private IChunkContainer _container;
        private IChunkCache _cache;
        private Chunk _chunk;

        private int _cx;
        private int _cz;

        private bool _dirty;

        private bool _autoLight = true;

        public int X
        {
            get { return _container.ChunkGlobalX(_cx); }
        }

        public int Z
        {
            get { return _container.ChunkGlobalZ(_cz); }
        }

        public int LocalX
        {
            get { return _container.ChunkLocalX(_cx); }
        }

        public int LocalZ
        {
            get { return _container.ChunkLocalZ(_cz); }
        }

        public bool AutoRecalcLight
        {
            get { return _autoLight; }
            set { _autoLight = value; }
        }

        public bool IsDirty
        {
            get { return _dirty; }
        }

        private ChunkRef ()
        {
        }

        public static ChunkRef Create (IChunkContainer container, IChunkCache cache, int cx, int cz)
        {
            if (!container.ChunkExists(cx, cz)) {
                return null;
            }

            ChunkRef c = new ChunkRef();

            c._container = container;
            c._cache = cache;
            c._cx = cx;
            c._cz = cz;

            return c;
        }

        public int BlockGlobalX (int x)
        {
            return _container.ChunkGlobalX(_cx) * XDim + x;
        }

        public int BlockGlobalY (int y)
        {
            return y;
        }

        public int BlockGlobalZ (int z)
        {
            return _container.ChunkGlobalZ(_cz) * ZDim + z;
        }

        public int BlockLocalX (int x)
        {
            return x;
        }

        public int BlockLocalY (int y)
        {
            return y;
        }

        public int BlockLocalZ (int z)
        {
            return z;
        }

        private Chunk GetChunk ()
        {
            if (_chunk == null) {
                _chunk = _container.GetChunk(_cx, _cz);

                _lightbit = new BitArray(XDim * 3 * ZDim * 3 * YDim);
                _update = new Queue<BlockKey>();
            }
            return _chunk;
        }

        private bool MarkDirty ()
        {
            if (_dirty) {
                return false;
            }

            _cache.MarkChunkDirty(this);

            _dirty = true;
            return true;
        }

        public ChunkRef GetNorthNeighbor ()
        {
            return _container.GetChunkRef(_cx - 1, _cz);
        }

        public ChunkRef GetSouthNeighbor ()
        {
            return _container.GetChunkRef(_cx + 1, _cz);
        }

        public ChunkRef GetEastNeighbor ()
        {
            return _container.GetChunkRef(_cx, _cz - 1);
        }

        public ChunkRef GetWestNeighbor ()
        {
            return _container.GetChunkRef(_cx, _cz + 1);
        }

        public Chunk GetChunkCopy ()
        {
            return GetChunk().Copy();
        }

        public Chunk GetChunkRef ()
        {
            Chunk chunk = GetChunk();
            _chunk = null;

            return chunk;
        }

        public void SetChunkRef (Chunk chunk)
        {
            _chunk = chunk;
            _chunk.SetLocation(_cx, _cz);
            MarkDirty();
        }

        #region IChunk Members

        public bool IsTerrainPopulated
        {
	        get { return GetChunk().IsTerrainPopulated; }
	        set 
            {
                if (GetChunk().IsTerrainPopulated != value) {
                    GetChunk().IsTerrainPopulated = value; 
                    MarkDirty();
                }
            }
        }

        public bool Save (Stream outStream)
        {
            if (_dirty) {
                if (GetChunk().Save(outStream)) {
                    _dirty = false;
                    return true;
                }
                return false;
            }
            return true;
        }

        public Block GetBlock (int lx, int ly, int lz)
        {
 	        return new Block(this, lx, ly, lz);
        }

        public BlockRef GetBlockRef (int lx, int ly, int lz)
        {
 	        return new BlockRef(this, lx, ly, lz);
        }

        public BlockInfo GetBlockInfo (int lx, int ly, int lz)
        {
 	        return GetChunk().GetBlockInfo(lx, ly, lz);
        }

        public void SetBlock (int lx, int ly, int lz, Block block)
        {
            GetChunk().SetBlock(lx, ly, lz, block);
        }

        public int CountBlockID (int id)
        {
 	        return GetChunk().CountBlockID(id);
        }

        public int CountBlockData (int id, int data)
        {
 	        return GetChunk().CountBlockData(id, data);
        }

        public int CountEntities ()
        {
            return GetChunk().CountEntities();
        }

        public int GetHeight (int lx, int lz)
        {
 	        return GetChunk().GetHeight(lx, lz);
        }

        #endregion


        #region IBoundedBlockContainer Members

        public int XDim
        {
            get { return 16; }
        }

        public int YDim
        {
            get { return 128; }
        }

        public int ZDim
        {
            get { return 16; }
        }

        #endregion


        #region IBlockContainer Members

        IBlock IBlockContainer.GetBlock (int lx, int ly, int lz)
        {
            return new Block(this, lx, ly, lz);
        }

        IBlock IBlockContainer.GetBlockRef (int lx, int ly, int lz)
        {
            return new BlockRef(this, lx, ly, lz);
        }

        public void SetBlock (int lx, int ly, int lz, IBlock block)
        {
            GetChunk().SetBlock(lx, ly, lz, block);
        }

        public int GetBlockID (int lx, int ly, int lz)
        {
            return GetChunk().GetBlockID(lx, ly, lz);
        }

        public int GetBlockData (int lx, int ly, int lz)
        {
            return GetChunk().GetBlockData(lx, ly, lz);
        }

        public bool SetBlockID (int lx, int ly, int lz, int id)
        {
            BlockInfo info1 = GetChunk().GetBlockInfo(lx, ly, lz);
            if (GetChunk().SetBlockID(lx, ly, lz, id)) {
                MarkDirty();

                if (_autoLight) {
                    BlockInfo info2 = GetChunk().GetBlockInfo(lx, ly, lz);
                    if (info1.Luminance != info2.Luminance || info1.Opacity != info2.Opacity) {
                        UpdateBlockLight(lx, ly, lz);
                    }

                    if (info1.Opacity != info2.Opacity) {
                        _update.Enqueue(new BlockKey(lx, ly, lz));
                        UpdateSkyLight();
                    }
                }

                return true;
            }
            return false;
        }

        public bool SetBlockData (int lx, int ly, int lz, int data)
        {
            if (GetChunk().SetBlockData(lx, ly, lz, data)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        #endregion


        #region ILitBlockContainer Members

        ILitBlock ILitBlockContainer.GetBlock (int lx, int ly, int lz)
        {
            throw new NotImplementedException();
        }

        ILitBlock ILitBlockContainer.GetBlockRef (int lx, int ly, int lz)
        {
            return new BlockRef(this, lx, ly, lz);
        }

        public void SetBlock (int lx, int ly, int lz, ILitBlock block)
        {
            GetChunk().SetBlock(lx, ly, lz, block);
        }

        public int GetBlockLight (int lx, int ly, int lz)
        {
            return GetChunk().GetBlockLight(lx, ly, lz);
        }

        public int GetBlockSkyLight (int lx, int ly, int lz)
        {
            return GetChunk().GetBlockSkyLight(lx, ly, lz);
        }

        public bool SetBlockLight (int lx, int ly, int lz, int light)
        {
            if (GetChunk().SetBlockLight(lx, ly, lz, light)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        public bool SetBlockSkyLight (int lx, int ly, int lz, int light)
        {
            if (GetChunk().SetBlockSkyLight(lx, ly, lz, light)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        #endregion


        #region IPropertyBlockContainer Members

        IPropertyBlock IPropertyBlockContainer.GetBlock (int lx, int ly, int lz)
        {
            return new Block(this, lx, ly, lz);
        }

        IPropertyBlock IPropertyBlockContainer.GetBlockRef (int lx, int ly, int lz)
        {
            return new BlockRef(this, lx, ly, lz);
        }

        public void SetBlock (int lx, int ly, int lz, IPropertyBlock block)
        {
            GetChunk().SetBlock(lx, ly, lz, block);
        }

        public TileEntity GetTileEntity (int lx, int ly, int lz)
        {
            return GetChunk().GetTileEntity(lx, ly, lz);
        }

        public bool SetTileEntity (int lx, int ly, int lz, TileEntity te)
        {
            if (GetChunk().SetTileEntity(lx, ly, lz, te)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        public bool ClearTileEntity (int lx, int ly, int lz)
        {
            if (GetChunk().ClearTileEntity(lx, ly, lz)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        #endregion


        #region IEntityContainer Members

        public List<Entity> FindEntities (string id)
        {
            return GetChunk().FindEntities(id);
        }

        public List<Entity> FindEntities (Predicate<Entity> match)
        {
            return GetChunk().FindEntities(match);
        }

        public bool AddEntity (Entity ent)
        {
            if (GetChunk().AddEntity(ent)) {
                MarkDirty();
                return true;
            }
            return false;
        }

        public int RemoveEntities (string id)
        {
            int ret = GetChunk().RemoveEntities(id);
            if (ret > 0) {
                MarkDirty();
            }
            return ret;
        }

        public int RemoveEntities (Predicate<Entity> match)
        {
            int ret = GetChunk().RemoveEntities(match);
            if (ret > 0) {
                MarkDirty();
            }
            return ret;
        }

        #endregion

        private BitArray _lightbit;
        private Queue<BlockKey> _update;

        public void ResetBlockLight ()
        {
            GetChunk().ResetBlockLight();
            MarkDirty();
        }

        public void ResetSkyLight ()
        {
            GetChunk().ResetSkyLight();
            MarkDirty();
        }

        public void RebuildBlockLight ()
        {
            GetChunk();

            for (int x = 0; x < XDim; x++) {
                for (int z = 0; z < ZDim; z++) {
                    for (int y = 0; y < YDim; y++) {
                        BlockInfo info = GetBlockInfo(x, y, z);
                        if (info.Luminance > 0) {
                            SpreadBlockLight(x, y, z);
                        }
                    }
                }
            }
        }

        public void RebuildSkyLight ()
        {
            GetChunk();

            // Optimization - only need to queue at level of highest neighbor's height
            for (int x = 0; x < XDim; x++) {
                for (int z = 0; z < ZDim; z++) {
                    ChunkRef ce = LocalChunk(x, 0, z - 1);
                    ChunkRef cn = LocalChunk(x - 1, 0, z);
                    ChunkRef cs = LocalChunk(x + 1, 0, z);
                    ChunkRef cw = LocalChunk(x, 0, z + 1);

                    int h = GetHeight(x, z);
                    if (ce != null) {
                        h = Math.Max(h, ce.GetHeight(x, ZDim - 1));
                    }
                    if (cn != null) {
                        h = Math.Max(h, cn.GetHeight(XDim - 1, z));
                    }
                    if (cs != null) {
                        h = Math.Max(h, cs.GetHeight(0, z));
                    }
                    if (cw != null) {
                        h = Math.Max(h, cw.GetHeight(x, 0));
                    }

                    for (int y = h; y < YDim; y++) {
                        SetBlockSkyLight(x, y, z, BlockInfo.MAX_LUMINANCE);
                    }

                    QueueRelight(new BlockKey(x, h, z));
                }
            }

            MarkDirty();
            UpdateSkyLight();
        }

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

        private void SpreadBlockLight (int lx, int ly, int lz)
        {
            BlockInfo primary = GetBlockInfo(lx, ly, lz);
            int primaryLight = GetBlockLight(lx, ly, lz);
            int priLum = Math.Max(primary.Luminance - primary.Opacity, 0);

            if (primaryLight < priLum) {
                SetBlockLight(lx, ly, lz, priLum);
            }

            if (primaryLight > primary.Luminance - 1) {
                return;
            }

            Queue<LightRecord> spread = new Queue<LightRecord>();
            spread.Enqueue(new LightRecord(lx - 1, ly, lz, primary.Luminance - 1));
            spread.Enqueue(new LightRecord(lx + 1, ly, lz, primary.Luminance - 1));
            spread.Enqueue(new LightRecord(lx, ly - 1, lz, primary.Luminance - 1));
            spread.Enqueue(new LightRecord(lx, ly + 1, lz, primary.Luminance - 1));
            spread.Enqueue(new LightRecord(lx, ly, lz - 1, primary.Luminance - 1));
            spread.Enqueue(new LightRecord(lx, ly, lz + 1, primary.Luminance - 1));

            while (spread.Count > 0) {
                LightRecord rec = spread.Dequeue();

                ChunkRef cc = LocalChunk(rec.x, rec.y, rec.z);
                if (cc == null) {
                    continue;
                }

                int x = (rec.x + XDim) % XDim;
                int y = rec.y;
                int z = (rec.z + ZDim) % ZDim;

                BlockInfo info = cc.GetBlockInfo(x, y, z);
                int light = cc.GetBlockLight(x, y, z);

                int dimStr = Math.Max(rec.str - info.Opacity, 0);

                if (dimStr > light) {
                    cc.SetBlockLight(x, y, z, dimStr);

                    spread.Enqueue(new LightRecord(rec.x - 1, rec.y, rec.z, dimStr - 1));
                    spread.Enqueue(new LightRecord(rec.x + 1, rec.y, rec.z, dimStr - 1));
                    spread.Enqueue(new LightRecord(rec.x, rec.y - 1, rec.z, dimStr - 1));
                    spread.Enqueue(new LightRecord(rec.x, rec.y + 1, rec.z, dimStr - 1));
                    spread.Enqueue(new LightRecord(rec.x, rec.y, rec.z - 1, dimStr - 1));
                    spread.Enqueue(new LightRecord(rec.x, rec.y, rec.z + 1, dimStr - 1));
                }
            }
        }

        public void UpdateBlockLight (int lx, int ly, int lz)
        {
            BlockKey primary = new BlockKey(lx, ly, lz);
            _update.Enqueue(primary);

            QueueRelight(new BlockKey(lx - 1, ly, lz));
            QueueRelight(new BlockKey(lx + 1, ly, lz));
            QueueRelight(new BlockKey(lx, ly - 1, lz));
            QueueRelight(new BlockKey(lx, ly + 1, lz));
            QueueRelight(new BlockKey(lx, ly, lz - 1));
            QueueRelight(new BlockKey(lx, ly, lz + 1));

            UpdateBlockLight();
        }

        private void UpdateBlockLight ()
        {
            while (_update.Count > 0) {
                BlockKey k = _update.Dequeue();
                int index = LightBitmapIndex(k);
                _lightbit[index] = false;

                ChunkRef cc = LocalChunk(k.x, k.y, k.z);
                if (cc == null) {
                    continue;
                }

                int lle = NeighborLight(k.x, k.y, k.z - 1);
                int lln = NeighborLight(k.x - 1, k.y, k.z);
                int lls = NeighborLight(k.x, k.y, k.z + 1);
                int llw = NeighborLight(k.x + 1, k.y, k.z);
                int lld = NeighborLight(k.x, k.y - 1, k.z);
                int llu = NeighborLight(k.x, k.y + 1, k.z);

                int x = (k.x + XDim * 2) % XDim;
                int y = k.y;
                int z = (k.z + ZDim * 2) % ZDim;

                int lightval = cc.GetBlockLight(x, y, z);
                BlockInfo info = cc.GetBlockInfo(x, y, z);

                int light = Math.Max(info.Luminance, 0);
                light = Math.Max(light, lle - 1);
                light = Math.Max(light, lln - 1);
                light = Math.Max(light, lls - 1);
                light = Math.Max(light, llw - 1);
                light = Math.Max(light, lld - 1);
                light = Math.Max(light, llu - 1);

                light = Math.Max(light - info.Opacity, 0);

                if (light != lightval) {
                    //Console.WriteLine("Block Light: ({0},{1},{2}) " + lightval + " -> " + light, k.x, k.y, k.z);

                    cc.SetBlockLight(x, y, z, light);
                    cc.MarkDirty();

                    QueueRelight(new BlockKey(k.x - 1, k.y, k.z));
                    QueueRelight(new BlockKey(k.x + 1, k.y, k.z));
                    QueueRelight(new BlockKey(k.x, k.y - 1, k.z));
                    QueueRelight(new BlockKey(k.x, k.y + 1, k.z));
                    QueueRelight(new BlockKey(k.x, k.y, k.z - 1));
                    QueueRelight(new BlockKey(k.x, k.y, k.z + 1));
                }
            }
        }

        private void UpdateSkyLight ()
        {
            while (_update.Count > 0) {
                BlockKey k = _update.Dequeue();
                int index = LightBitmapIndex(k);
                _lightbit[index] = false;

                ChunkRef cc = LocalChunk(k.x, k.y, k.z);
                if (cc == null) {
                    continue;
                }

                int x = (k.x + XDim) % XDim;
                int y = k.y;
                int z = (k.z + ZDim) % ZDim;

                int lightval = cc.GetBlockSkyLight(x, y, z);
                BlockInfo info = cc.GetBlockInfo(x, y, z);

                int light = BlockInfo.MIN_LUMINANCE;

                if (cc.GetHeight(x, z) <= y) {
                    light = BlockInfo.MAX_LUMINANCE;
                }
                else {
                    int lle = NeighborSkyLight(k.x, k.y, k.z - 1);
                    int lln = NeighborSkyLight(k.x - 1, k.y, k.z);
                    int lls = NeighborSkyLight(k.x, k.y, k.z + 1);
                    int llw = NeighborSkyLight(k.x + 1, k.y, k.z);
                    int lld = NeighborSkyLight(k.x, k.y - 1, k.z);
                    int llu = NeighborSkyLight(k.x, k.y + 1, k.z);

                    light = Math.Max(light, lle - 1);
                    light = Math.Max(light, lln - 1);
                    light = Math.Max(light, lls - 1);
                    light = Math.Max(light, llw - 1);
                    light = Math.Max(light, lld - 1);
                    light = Math.Max(light, llu - 1);
                }

                light = Math.Max(light - info.Opacity, 0);

                if (light != lightval) {
                    //Console.WriteLine("Block SkyLight: ({0},{1},{2}) " + lightval + " -> " + light, k.x, k.y, k.z);

                    cc.SetBlockSkyLight(x, y, z, light);
                    cc.MarkDirty();

                    QueueRelight(new BlockKey(k.x - 1, k.y, k.z));
                    QueueRelight(new BlockKey(k.x + 1, k.y, k.z));
                    QueueRelight(new BlockKey(k.x, k.y - 1, k.z));
                    QueueRelight(new BlockKey(k.x, k.y + 1, k.z));
                    QueueRelight(new BlockKey(k.x, k.y, k.z - 1));
                    QueueRelight(new BlockKey(k.x, k.y, k.z + 1));
                }
            }
        }

        private int LightBitmapIndex (BlockKey key)
        {
            int x = key.x + XDim;
            int y = key.y;
            int z = key.z + ZDim;

            int zstride = YDim;
            int xstride = ZDim * 3 * zstride;

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

        private ChunkRef LocalChunk (int lx, int ly, int lz)
        {
            if (ly < 0 || ly >= YDim) {
                return null;
            }

            if (lx < 0) {
                if (lz < 0) {
                    return _container.GetChunkRef(_cx - 1, _cz - 1);
                }
                else if (lz >= ZDim) {
                    return _container.GetChunkRef(_cx - 1, _cz + 1);
                }
                return _container.GetChunkRef(_cx - 1, _cz);
            }
            else if (lx >= XDim) {
                if (lz < 0) {
                    return _container.GetChunkRef(_cx + 1, _cz - 1);
                }
                else if (lz >= ZDim) {
                    return _container.GetChunkRef(_cx + 1, _cz + 1);
                }
                return _container.GetChunkRef(_cx + 1, _cz);
            }
            else {
                if (lz < 0) {
                    return _container.GetChunkRef(_cx, _cz - 1);
                }
                else if (lz >= ZDim) {
                    return _container.GetChunkRef(_cx, _cz + 1);
                }
                return this;
            }
        }

        private int NeighborLight (int x, int y, int z)
        {
            if (y < 0 || y >= YDim) {
                return 0;
            }

            ChunkRef src = LocalChunk(x, y, z);
            if (src == null) {
                return 0;
            }

            x = (x + XDim * 2) % XDim;
            z = (z + ZDim * 2) % ZDim;

            BlockInfo info = src.GetBlockInfo(x, y, z);
            int light = src.GetBlockLight(x, y, z);

            return Math.Max(light, info.Luminance);
        }

        private int NeighborSkyLight (int x, int y, int z)
        {
            if (y < 0 || y >= YDim) {
                return 0;
            }

            ChunkRef src = LocalChunk(x, y, z);
            if (src == null) {
                return 0;
            }

            x = (x + XDim * 2) % XDim;
            z = (z + ZDim * 2) % ZDim;

            int light = src.GetBlockSkyLight(x, y, z);

            return light;
        }

        public void UpdateEdgeBlockLight ()
        {
            GetChunk();
            QueueChunkEdges();
            UpdateBlockLight();
        }

        public void UpdateEdgeSkyLight ()
        {
            GetChunk();
            QueueChunkEdges();
            UpdateSkyLight();
        }

        private void QueueChunkEdges ()
        {
            // X = 0, X = XDim
            for (int z = 0; z < ZDim; z++) {
                for (int y = 0; y < YDim; y++) {
                    QueueRelight(new BlockKey(0, y, z));
                    QueueRelight(new BlockKey(XDim - 1, y, z));
                }
            }

            // Z = 0, Z = ZDim
            for (int x = 1; x < XDim - 1; x++) {
                for (int y = 0; y < YDim; y++) {
                    QueueRelight(new BlockKey(x, y, 0));
                    QueueRelight(new BlockKey(x, y, ZDim - 1));
                }
            }
        }

        public void UpdateEdgeBlockLight (ChunkRef chunk)
        {
            GetChunk();

            if (chunk.X == X && chunk.Z == Z - 1) {
                for (int x = 0; x < XDim; x++) {
                    for (int y = 0; y < YDim; y++) {
                        TestBlockLight(chunk, x, y, 0, x, y, ZDim - 1);
                    }
                }
            }

            else if (chunk.X == X && chunk.Z == Z + 1) {
                for (int x = 0; x < XDim; x++) {
                    for (int y = 0; y < YDim; y++) {
                        TestBlockLight(chunk, x, y, ZDim - 1, x, y, 0);
                    }
                }
            }

            else if (chunk.Z == Z && chunk.X == X - 1) {
                for (int z = 0; z < ZDim; z++) {
                    for (int y = 0; y < YDim; y++) {
                        TestBlockLight(chunk, 0, y, z, XDim - 1, y, z);
                    }
                }
            }

            else if (chunk.Z == Z && chunk.X == X + 1) {
                for (int z = 0; z < ZDim; z++) {
                    for (int y = 0; y < YDim; y++) {
                        TestBlockLight(chunk, XDim - 1, y, z, 0, y, z);
                    }
                }
            }

            UpdateBlockLight();
        }

        public void UpdateEdgeSkyLight (ChunkRef chunk)
        {
            GetChunk();

            if (chunk.X == X && chunk.Z == Z - 1) {
                for (int x = 0; x < XDim; x++) {
                    for (int y = 0; y < YDim; y++) {
                        TestSkyLight(chunk, x, y, 0, x, y, ZDim - 1);
                    }
                }
            }

            else if (chunk.X == X && chunk.Z == Z + 1) {
                for (int x = 0; x < XDim; x++) {
                    for (int y = 0; y < YDim; y++) {
                        TestSkyLight(chunk, x, y, ZDim - 1, x, y, 0);
                    }
                }
            }

            else if (chunk.Z == Z && chunk.X == X - 1) {
                for (int z = 0; z < ZDim; z++) {
                    for (int y = 0; y < YDim; y++) {
                        TestSkyLight(chunk, 0, y, z, XDim - 1, y, z);
                    }
                }
            }

            else if (chunk.Z == Z && chunk.X == X + 1) {
                for (int z = 0; z < ZDim; z++) {
                    for (int y = 0; y < YDim; y++) {
                        TestSkyLight(chunk, XDim - 1, y, z, 0, y, z);
                    }
                }
            }

            UpdateSkyLight();
        }

        private void TestBlockLight (ChunkRef chunk, int x1, int y1, int z1, int x2, int y2, int z2)
        {
            int light1 = GetBlockLight(x1, y1, z1);
            int light2 = chunk.GetBlockLight(x2, y2, z2);
            int lum1 = GetBlockInfo(x1, y1, z1).Luminance;
            int lum2 = chunk.GetBlockInfo(x2, y2, z2).Luminance;

            int v1 = Math.Max(light1, lum1);
            int v2 = Math.Max(light2, lum2);
            if (Math.Abs(v1 - v2) > 1) {
                QueueRelight(new BlockKey(x1, y1, z1));
            }
        }

        private void TestSkyLight (ChunkRef chunk, int x1, int y1, int z1, int x2, int y2, int z2)
        {
            int light1 = GetBlockSkyLight(x1, y1, z1);
            int light2 = chunk.GetBlockSkyLight(x2, y2, z2);

            if (Math.Abs(light1 - light2) > 1) {
                QueueRelight(new BlockKey(x1, y1, z1));
            }
        }
    }

}
