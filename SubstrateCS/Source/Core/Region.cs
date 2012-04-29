using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Substrate.Nbt;
using Substrate.Core;

namespace Substrate.Core
{
    

    /// <summary>
    /// Represents a single region containing 32x32 chunks.
    /// </summary>
    public abstract class Region : IDisposable, IRegion
    {
        protected const int XDIM = 32;
        protected const int ZDIM = 32;
        protected const int XMASK = XDIM - 1;
        protected const int ZMASK = ZDIM - 1;
        protected const int XLOG = 5;
        protected const int ZLOG = 5;

        protected int _rx;
        protected int _rz;
        private bool _disposed = false;

        protected RegionManager _regionMan;

        private static Regex _namePattern = new Regex("r\\.(-?[0-9]+)\\.(-?[0-9]+)\\.mca$");

        private WeakReference _regionFile;

        protected ChunkCache _cache;

        protected abstract IChunk CreateChunkCore (int cx, int cz);

        protected abstract IChunk CreateChunkVerifiedCore (NbtTree tree);

        protected abstract bool ParseFileNameCore (string filename, out int x, out int z);

        /// <inherit />
        public int X
        {
            get { return _rx; }
        }

        /// <inherit />
        public int Z
        {
            get { return _rz; }
        }

        /// <summary>
        /// Gets the length of the X-dimension of the region in chunks.
        /// </summary>
        public int XDim
        {
            get { return XDIM; }
        }

        /// <summary>
        /// Gets the length of the Z-dimension of the region in chunks.
        /// </summary>
        public int ZDim
        {
            get { return ZDIM; }
        }

        public abstract string GetFileName ();

        public abstract string GetFilePath ();

        /// <summary>
        /// Creates an instance of a <see cref="Region"/> for a given set of coordinates.
        /// </summary>
        /// <param name="rm">The <see cref="RegionManager"/> that should be managing this region.</param>
        /// <param name="cache">A shared cache for holding chunks.</param>
        /// <param name="rx">The global X-coordinate of the region.</param>
        /// <param name="rz">The global Z-coordinate of the region.</param>
        /// <remarks><para>The constructor will not actually open or parse any region files.  Given just the region coordinates, the
        /// region will be able to determien the correct region file to look for based on the naming pattern for regions:
        /// r.x.z.mcr, given x and z are integers representing the region's coordinates.</para>
        /// <para>Regions require a <see cref="ChunkCache"/> to be provided because they do not actually store any chunks or references
        /// to chunks on their own.  This allows regions to easily pass off requests outside of their bounds, if necessary.</para></remarks>
        public Region (RegionManager rm, ChunkCache cache, int rx, int rz)
        {
            _regionMan = rm;
            _cache = cache;
            _regionFile = new WeakReference(null);
            _rx = rx;
            _rz = rz;

            if (!File.Exists(GetFilePath())) {
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// Creates an instance of a <see cref="Region"/> for the given region file.
        /// </summary>
        /// <param name="rm">The <see cref="RegionManager"/> that should be managing this region.</param>
        /// <param name="cache">A shared cache for holding chunks.</param>
        /// <param name="filename">The region file to derive the region from.</param>
        /// <remarks><para>The constructor will not actually open or parse the region file.  It will only read the file's name in order
        /// to derive the region's coordinates, based on a strict naming pattern for regions: r.x.z.mcr, given x and z are integers
        /// representing the region's coordinates.</para>
        /// <para>Regions require a <see cref="ChunkCache"/> to be provided because they do not actually store any chunks or references
        /// to chunks on their own.  This allows regions to easily pass off requests outside of their bounds, if necessary.</para></remarks>
        public Region (RegionManager rm, ChunkCache cache, string filename)
        {
            _regionMan = rm;
            _cache = cache;
            _regionFile = new WeakReference(null);

            ParseFileNameCore(filename, out _rx, out _rz);

            if (!File.Exists(Path.Combine(_regionMan.GetRegionPath(), filename))) {
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// Region finalizer that ensures any resources are cleaned up
        /// </summary>
        ~Region ()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes any managed and unmanaged resources held by the region.
        /// </summary>
        public void Dispose ()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Conditionally dispose managed or unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if the call to Dispose was explicit.</param>
        protected virtual void Dispose (bool disposing)
        {
            if (!_disposed) {
                if (disposing) {
                    // Cleanup managed resources
                    RegionFile rf = _regionFile.Target as RegionFile;
                    if (rf != null) {
                        rf.Dispose();
                        rf = null;
                    }
                }

                // Cleanup unmanaged resources
            }
            _disposed = true;
        }

        private RegionFile GetRegionFile ()
        {
            RegionFile rf = _regionFile.Target as RegionFile;
            if (rf == null) {
                rf = new RegionFile(GetFilePath());
                _regionFile.Target = rf;
            }

            return rf;
        }

        /// <inherits />
        public NbtTree GetChunkTree (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunkTree(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            Stream nbtstr = rf.GetChunkDataInputStream(lcx, lcz);
            if (nbtstr == null) {
                return null;
            }

            NbtTree tree = new NbtTree(nbtstr);

            nbtstr.Close();
            return tree;
        }

        // XXX: Exceptions
        /// <inherits />
        public bool SaveChunkTree (int lcx, int lcz, NbtTree tree)
        {
            return SaveChunkTree(lcx, lcz, tree, null);
        }

        /// <inherits />
        public bool SaveChunkTree (int lcx, int lcz, NbtTree tree, int timestamp)
        {
            return SaveChunkTree(lcx, lcz, tree, timestamp);
        }

        private bool SaveChunkTree (int lcx, int lcz, NbtTree tree, int? timestamp)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? false : alt.SaveChunkTree(ForeignX(lcx), ForeignZ(lcz), tree);
            }

            RegionFile rf = GetRegionFile();
            Stream zipstr = (timestamp == null)
                ? rf.GetChunkDataOutputStream(lcx, lcz)
                : rf.GetChunkDataOutputStream(lcx, lcz, (int)timestamp);

            if (zipstr == null) {
                return false;
            }

            tree.WriteTo(zipstr);
            zipstr.Close();

            return true;
        }

        /// <inherits />
        public Stream GetChunkOutStream (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunkOutStream(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            return rf.GetChunkDataOutputStream(lcx, lcz);
        }

        /// <inherits />
        public int ChunkCount ()
        {
            RegionFile rf = GetRegionFile();

            int count = 0;
            for (int x = 0; x < XDIM; x++) {
                for (int z = 0; z < ZDIM; z++) {
                    if (rf.HasChunk(x, z)) {
                        count++;
                    }
                }
            }

            return count;
        }

        // XXX: Consider revising foreign lookup support
        /// <inherits />
        public ChunkRef GetChunkRef (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunkRef(ForeignX(lcx), ForeignZ(lcz));
            }

            int cx = lcx + _rx * XDIM;
            int cz = lcz + _rz * ZDIM;

            ChunkKey k = new ChunkKey(cx, cz);
            ChunkRef c = _cache.Fetch(k);
            if (c != null) {
                return c;
            }

            c = ChunkRef.Create(this, lcx, lcz);
            if (c != null) {
                _cache.Insert(c);
            }

            return c;
        }

        /// <inherits />
        public ChunkRef CreateChunk (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.CreateChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            DeleteChunk(lcx, lcz);

            int cx = lcx + _rx * XDIM;
            int cz = lcz + _rz * ZDIM;

            IChunk c = CreateChunkCore(cx, cz);
            c.Save(GetChunkOutStream(lcx, lcz));

            ChunkRef cr = ChunkRef.Create(this, lcx, lcz);
            _cache.Insert(cr);

            return cr;
        }


        #region IChunkCollection Members

        // XXX: This also feels dirty.
        /// <summary>
        /// Gets the global X-coordinate of a chunk given an internal coordinate handed out by a <see cref="Region"/> container.
        /// </summary>
        /// <param name="cx">An internal X-coordinate given to a <see cref="ChunkRef"/> by any instance of a <see cref="Region"/> container.</param>
        /// <returns>The global X-coordinate of the corresponding chunk.</returns>
        public int ChunkGlobalX (int cx)
        {
            return _rx * XDIM + cx;
        }

        /// <summary>
        /// Gets the global Z-coordinate of a chunk given an internal coordinate handed out by a <see cref="Region"/> container.
        /// </summary>
        /// <param name="cz">An internal Z-coordinate given to a <see cref="ChunkRef"/> by any instance of a <see cref="Region"/> container.</param>
        /// <returns>The global Z-coordinate of the corresponding chunk.</returns>
        public int ChunkGlobalZ (int cz)
        {
            return _rz * ZDIM + cz;
        }

        /// <summary>
        /// Gets the region-local X-coordinate of a chunk given an internal coordinate handed out by a <see cref="Region"/> container.
        /// </summary>
        /// <param name="cx">An internal X-coordinate given to a <see cref="ChunkRef"/> by any instance of a <see cref="Region"/> container.</param>
        /// <returns>The region-local X-coordinate of the corresponding chunk.</returns>
        public int ChunkLocalX (int cx)
        {
            return cx;
        }

        /// <summary>
        /// Gets the region-local Z-coordinate of a chunk given an internal coordinate handed out by a <see cref="Region"/> container.
        /// </summary>
        /// <param name="cz">An internal Z-coordinate given to a <see cref="ChunkRef"/> by any instance of a <see cref="Region"/> container.</param>
        /// <returns>The region-local Z-coordinate of the corresponding chunk.</returns>
        public int ChunkLocalZ (int cz)
        {
            return cz;
        }

        /// <summary>
        /// Returns a <see cref="IChunk"/> given local coordinates relative to this region.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <returns>A <see cref="IChunk"/> object for the given coordinates, or null if the chunk does not exist.</returns>
        /// <remarks>If the local coordinates are out of bounds for this region, the action will be forwarded to the correct region
        /// transparently.  The returned <see cref="IChunk"/> object may either come from cache, or be regenerated from disk.</remarks>
        public IChunk GetChunk (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            if (!ChunkExists(lcx, lcz)) {
                return null;
            }

            return CreateChunkVerifiedCore(GetChunkTree(lcx, lcz));
        }

        /// <summary>
        /// Checks if a chunk exists at the given local coordinates relative to this region.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <returns>True if there is a chunk at the given coordinates; false otherwise.</returns>
        /// <remarks>If the local coordinates are out of bounds for this region, the action will be forwarded to the correct region
        /// transparently.</remarks>
        public bool ChunkExists (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? false : alt.ChunkExists(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            return rf.HasChunk(lcx, lcz);
        }

        /// <summary>
        /// Deletes a chunk from the underlying data store at the given local coordinates relative to this region.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <returns>True if there is a chunk was deleted; false otherwise.</returns>
        /// <remarks>If the local coordinates are out of bounds for this region, the action will be forwarded to the correct region
        /// transparently.</remarks>
        public bool DeleteChunk (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? false : alt.DeleteChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            if (!rf.HasChunk(lcx, lcz)) {
                return false;
            }

            rf.DeleteChunk(lcx, lcz);

            ChunkKey k = new ChunkKey(ChunkGlobalX(lcx), ChunkGlobalZ(lcz));
            _cache.Remove(k);

            if (ChunkCount() == 0) {
                _regionMan.DeleteRegion(X, Z);
                _regionFile.Target = null;
            }

            return true;
        }

        /// <summary>
        /// Saves an existing <see cref="IChunk"/> to the region at the given local coordinates.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <param name="chunk">A <see cref="IChunk"/> to save to the given location.</param>
        /// <returns>A <see cref="ChunkRef"/> represneting the <see cref="IChunk"/> at its new location.</returns>
        /// <remarks>If the local coordinates are out of bounds for this region, the action will be forwarded to the correct region
        /// transparently.  The <see cref="IChunk"/>'s internal global coordinates will be updated to reflect the new location.</remarks>
        public ChunkRef SetChunk (int lcx, int lcz, IChunk chunk)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.CreateChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            DeleteChunk(lcx, lcz);

            int cx = lcx + _rx * XDIM;
            int cz = lcz + _rz * ZDIM;

            chunk.SetLocation(cx, cz);
            chunk.Save(GetChunkOutStream(lcx, lcz));

            ChunkRef cr = ChunkRef.Create(this, lcx, lcz);
            _cache.Insert(cr);

            return cr;
        }

        /// <summary>
        /// Saves all chunks within this region that have been marked as dirty.
        /// </summary>
        /// <returns>The number of chunks that were saved.</returns>
        public int Save ()
        {
            _cache.SyncDirty();

            int saved = 0;
            IEnumerator<ChunkRef> en = _cache.GetDirtyEnumerator();
            while (en.MoveNext()) {
                ChunkRef chunk = en.Current;

                if (!ChunkExists(chunk.LocalX, chunk.LocalZ)) {
                    throw new MissingChunkException();
                }

                if (chunk.Save(GetChunkOutStream(chunk.LocalX, chunk.LocalZ))) {
                    saved++;
                }
            }

            _cache.ClearDirty();
            return saved;
        }

        // XXX: Allows a chunk not part of this region to be saved to it
        /// <exclude/>
        public bool SaveChunk (IChunk chunk)
        {
            //Console.WriteLine("Region[{0}, {1}].Save({2}, {3})", _rx, _rz, ForeignX(chunk.X),ForeignZ(chunk.Z));
            return chunk.Save(GetChunkOutStream(ForeignX(chunk.X), ForeignZ(chunk.Z)));
        }

        /// <summary>
        /// Checks if this container supports delegating an action on out-of-bounds coordinates to another container. 
        /// </summary>
        public bool CanDelegateCoordinates
        {
            get { return true; }
        }

        /// <inherits />
        public int GetChunkTimestamp (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? 0 : alt.GetChunkTimestamp(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            return rf.GetTimestamp(lcx, lcz);
        }

        /// <inherits />
        public void SetChunkTimestamp (int lcx, int lcz, int timestamp)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                IRegion alt = GetForeignRegion(lcx, lcz);
                if (alt != null) 
                    alt.SetChunkTimestamp(ForeignX(lcx), ForeignZ(lcz), timestamp);
            }

            RegionFile rf = GetRegionFile();
            rf.SetTimestamp(lcx, lcz, timestamp);
        }

        #endregion

        protected bool LocalBoundsCheck (int lcx, int lcz)
        {
            return (lcx >= 0 && lcx < XDIM && lcz >= 0 && lcz < ZDIM);
        }

        protected IRegion GetForeignRegion (int lcx, int lcz)
        {
            return _regionMan.GetRegion(_rx + (lcx >> XLOG), _rz + (lcz >> ZLOG));
        }

        protected int ForeignX (int lcx)
        {
            return (lcx + XDIM * 10000) & XMASK;
        }

        protected int ForeignZ (int lcz)
        {
            return (lcz + ZDIM * 10000) & ZMASK;
        }
    }
}
