using System;
using System.IO;
using System.Collections.Generic;

namespace Substrate
{
    /// <summary>
    /// Provides a wrapper around a physical Chunk stored in a chunk container.  Modifying data in a ChunkRef will signal to the chunk 
    /// container that the physical chunk needs to be saved.
    /// </summary>
    public class ChunkRef : IChunk
    {
        private IChunkContainer _container;
        private Chunk _chunk;

        private AlphaBlockCollection _blocks;
        private EntityCollection _entities;

        private int _cx;
        private int _cz;

        private bool _dirty;

        /// <summary>
        /// Gets the global X-coordinate of the chunk.
        /// </summary>
        public int X
        {
            get { return _container.ChunkGlobalX(_cx); }
        }

        /// <summary>
        /// Gets the global Z-coordinate of the chunk.
        /// </summary>
        public int Z
        {
            get { return _container.ChunkGlobalZ(_cz); }
        }

        /// <summary>
        /// Gets the local X-coordinate of the chunk within container.
        /// </summary>
        public int LocalX
        {
            get { return _container.ChunkLocalX(_cx); }
        }

        /// <summary>
        /// Gets the local Z-coordinate of the chunk within container.
        /// </summary>
        public int LocalZ
        {
            get { return _container.ChunkLocalZ(_cz); }
        }

        /// <summary>
        /// Gets the collection of all blocks and their data stored in the chunk.
        /// </summary>
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

        /// <summary>
        /// Gets the collection of all entities stored in the chunk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the value indicating that the chunk has been modified, but not saved.
        /// </summary>
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

        /// <summary>
        /// Forbid direct instantiation of ChunkRef objects
        /// </summary>
        private ChunkRef ()
        {
        }

        /// <summary>
        /// Create a reference to a chunk stored in a chunk container.
        /// </summary>
        /// <param name="container">Chunk container</param>
        /// <param name="cx">Local X-coordinate of chunk within container.</param>
        /// <param name="cz">Local Z-coordinate of chunk within container.</param>
        /// <returns>ChunkRef representing a reference to a physical chunk at the specified location within the container.</returns>
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

        /// <summary>
        /// Gets or sets the chunk's TerrainPopulated status.
        /// </summary>
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

        /// <summary>
        /// Saves the underlying physical chunk to the specified output stream.
        /// </summary>
        /// <param name="outStream">An open output stream.</param>
        /// <returns>A value indicating whether the chunk is no longer considered dirty.</returns>
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

        /// <summary>
        /// Gets a ChunkRef to the chunk positioned immediately north (X - 1).
        /// </summary>
        /// <returns>ChunkRef to the northern neighboring chunk.</returns>
        public ChunkRef GetNorthNeighbor ()
        {
            return _container.GetChunkRef(_cx - 1, _cz);
        }

        /// <summary>
        /// Gets a ChunkRef to the chunk positioned immediately south (X + 1).
        /// </summary>
        /// <returns>ChunkRef to the southern neighboring chunk.</returns>
        public ChunkRef GetSouthNeighbor ()
        {
            return _container.GetChunkRef(_cx + 1, _cz);
        }

        /// <summary>
        /// Gets a ChunkRef to the chunk positioned immediatly east (Z - 1).
        /// </summary>
        /// <returns>ChunkRef to the eastern neighboring chunk.</returns>
        public ChunkRef GetEastNeighbor ()
        {
            return _container.GetChunkRef(_cx, _cz - 1);
        }

        /// <summary>
        /// Gets a ChunkRef to the chunk positioned immedately west (Z + 1).
        /// </summary>
        /// <returns>ChunkRef to the western neighboring chunk.</returns>
        public ChunkRef GetWestNeighbor ()
        {
            return _container.GetChunkRef(_cx, _cz + 1);
        }

        /// <summary>
        /// Returns a deep copy of the physical chunk underlying the ChunkRef.
        /// </summary>
        /// <returns>A copy of the physical Chunk object.</returns>
        public Chunk GetChunkCopy ()
        {
            return GetChunk().Copy();
        }

        /// <summary>
        /// Returns the reference of the physical chunk underlying the ChunkRef, and releases the reference from itself.
        /// </summary>
        /// <remarks>
        /// This function returns the reference to the chunk stored in the chunk container.  Because the ChunkRef simultaneously gives up
        /// its "ownership" of the Chunk, the container will not consider the Chunk dirty even if it is modified.  Attempting to use the ChunkRef after
        /// releasing its internal reference will query the container for a new reference.  If the chunk is still cached, it will get the same reference
        /// back, otherwise it will get an independent copy.  Chunks should only be taken from ChunkRefs to transfer them to another ChunkRef, or
        /// to modify them without intending to permanently store the changes.
        /// </remarks>
        /// <returns>The physical Chunk object underlying the ChunkRef</returns>
        public Chunk GetChunkRef ()
        {
            Chunk chunk = GetChunk();
            _chunk = null;
            _dirty = false;

            return chunk;
        }

        /// <summary>
        /// Replaces the underlying physical chunk with a different one, updating its physical location to reflect the ChunkRef.
        /// </summary>
        /// <remarks>
        /// Use this function to save chunks that have been created or manipulated independently of a container, or to
        /// move a physical chunk between locations within a container (by taking the reference from another ChunkRef).
        /// </remarks>
        /// <param name="chunk">Physical Chunk to store into the location represented by this ChunkRef.</param>
        public void SetChunkRef (Chunk chunk)
        {
            _chunk = chunk;
            _chunk.SetLocation(X, Z);
            _dirty = true;
        }

        /// <summary>
        /// Gets an internal Chunk reference from cache or queries the container for it.
        /// </summary>
        /// <returns>The ChunkRef's underlying Chunk.</returns>
        private Chunk GetChunk ()
        {
            if (_chunk == null) {
                _chunk = _container.GetChunk(_cx, _cz);

                if (_chunk != null) {
                    _blocks = _chunk.Blocks;
                    _entities = _chunk.Entities;

                    // Set callback functions in the underlying block collection
                    _blocks.ResolveNeighbor += ResolveNeighborHandler;
                    _blocks.TranslateCoordinates += TranslateCoordinatesHandler;
                }
            }
            return _chunk;
        }

        /// <summary>
        /// Callback function to return the block collection of a ChunkRef at a relative offset to this one.
        /// </summary>
        /// <param name="relx">Relative offset from the X-coordinate.</param>
        /// <param name="rely">Relative offset from the Y-coordinate.</param>
        /// <param name="relz">Relative offset from the Z-coordinate.</param>
        /// <returns>Another ChunkRef's underlying block collection, or null if the ChunkRef cannot be found.</returns>
        private AlphaBlockCollection ResolveNeighborHandler (int relx, int rely, int relz)
        {
            ChunkRef cr = _container.GetChunkRef(_cx + relx, _cz + relz);
            if (cr != null) {
                return cr.Blocks;
            }

            return null;
        }

        /// <summary>
        /// Translates chunk-local block coordinates to corresponding global coordinates.
        /// </summary>
        /// <param name="lx">Chunk-local X-coordinate.</param>
        /// <param name="ly">Chunk-local Y-coordinate.</param>
        /// <param name="lz">Chunk-local Z-coordinate.</param>
        /// <returns>BlockKey containing the global block coordinates.</returns>
        private BlockKey TranslateCoordinatesHandler (int lx, int ly, int lz)
        {
            int x = X * _blocks.XDim + lx;
            int z = Z * _blocks.ZDim + lz;

            return new BlockKey(x, ly, z);
        }
    }
}
