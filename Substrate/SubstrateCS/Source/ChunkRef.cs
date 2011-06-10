using System;
using System.IO;
using System.Collections.Generic;

namespace Substrate
{
    public class ChunkRef : IChunk
    {
        private IChunkContainer _container;
        private Chunk _chunk;

        private AlphaBlockCollection _blocks;
        private EntityCollection _entities;

        private int _cx;
        private int _cz;

        private bool _dirty;

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

        public AlphaBlockCollection Blocks
        {
            get 
            {
                if (_blocks == null) {
                    GetChunk();
                }
                return _blocks;
            }
        }

        public EntityCollection Entities
        {
            get
            {
                if (_entities == null) {
                    GetChunk();
                }
                return _entities;
            }
        }

        public bool IsDirty
        {
            get
            {
                return _dirty
                    || (_blocks != null && _blocks.IsDirty)
                    || (_entities != null && _entities.IsDirty);
            }

            set
            {
                _dirty = value;
                if (_blocks != null)
                    _blocks.IsDirty = false;
                if (_entities != null)
                    _entities.IsDirty = false;
            }
        }

        private ChunkRef ()
        {
        }

        public static ChunkRef Create (IChunkContainer container, int cx, int cz)
        {
            if (!container.ChunkExists(cx, cz)) {
                return null;
            }

            ChunkRef c = new ChunkRef();

            c._container = container;
            c._cx = cx;
            c._cz = cz;

            return c;
        }

        public bool IsTerrainPopulated
        {
            get { return GetChunk().IsTerrainPopulated; }
            set
            {
                if (GetChunk().IsTerrainPopulated != value) {
                    GetChunk().IsTerrainPopulated = value;
                    _dirty = true;
                }
            }
        }

        public bool Save (Stream outStream)
        {
            if (IsDirty) {
                if (GetChunk().Save(outStream)) {
                    IsDirty = false;
                    return true;
                }
                return false;
            }
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
            _dirty = false;

            return chunk;
        }

        public void SetChunkRef (Chunk chunk)
        {
            _chunk = chunk;
            _chunk.SetLocation(X, Z);
            _dirty = true;
        }


        private Chunk GetChunk ()
        {
            if (_chunk == null) {
                _chunk = _container.GetChunk(_cx, _cz);

                if (_chunk != null) {
                    _blocks = _chunk.Blocks;
                    _entities = _chunk.Entities;

                    _blocks.ResolveNeighbor += ResolveNeighborHandler;
                    _blocks.TranslateCoordinates += TranslateCoordinatesHandler;
                }
            }
            return _chunk;
        }

        private AlphaBlockCollection ResolveNeighborHandler (int relx, int rely, int relz)
        {
            ChunkRef cr = _container.GetChunkRef(_cx + relx, _cz + relz);
            if (cr != null) {
                return cr.Blocks;
            }

            return null;
        }

        private BlockKey TranslateCoordinatesHandler (int lx, int ly, int lz)
        {
            int x = X * _blocks.XDim + lx;
            int z = Z * _blocks.ZDim + lz;

            return new BlockKey(x, ly, z);
        }
    }
}
