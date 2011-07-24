using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Substrate.Nbt;
using Substrate.Core;

namespace Substrate
{
    /// <summary>
    /// Represents a single region containing 32x32 chunks.
    /// </summary>
    public class Region : IDisposable, IChunkContainer
    {
        private const int XDIM = 32;
        private const int ZDIM = 32;
        private const int XMASK = XDIM - 1;
        private const int ZMASK = ZDIM - 1;
        private const int XLOG = 5;
        private const int ZLOG = 5;

        private int _rx;
        private int _rz;
        private bool _disposed = false;

        private RegionManager _regionMan;

        private static Regex _namePattern = new Regex("r\\.(-?[0-9]+)\\.(-?[0-9]+)\\.mcr$");

        private WeakReference _regionFile;

        private ChunkCache _cache;

        /// <summary>
        /// Gets the global X-coordinate of the region.
        /// </summary>
        public int X
        {
            get { return _rx; }
        }

        /// <summary>
        /// Gets the global Z-coordinate of the region.
        /// </summary>
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

            ParseFileName(filename, out _rx, out _rz);

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

        /// <summary>
        /// Get the appropriate filename for this region.
        /// </summary>
        /// <returns>The filename of the region with encoded coordinates.</returns>
        public string GetFileName ()
        {
            return "r." + _rx + "." + _rz + ".mcr";
            
        }

        /// <summary>
        /// Tests if the given filename conforms to the general naming pattern for any region.
        /// </summary>
        /// <param name="filename">The filename to test.</param>
        /// <returns>True if the filename is a valid region name; false if it does not conform to the pattern.</returns>
        public static bool TestFileName (string filename)
        {
            Match match = _namePattern.Match(filename);
            if (!match.Success) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses the given filename to extract encoded region coordinates.
        /// </summary>
        /// <param name="filename">The region filename to parse.</param>
        /// <param name="x">This parameter will contain the X-coordinate of a region.</param>
        /// <param name="z">This parameter will contain the Z-coordinate of a region.</param>
        /// <returns>True if the filename could be correctly parse; false otherwise.</returns>
        public static bool ParseFileName (string filename, out int x, out int z)
        {
            x = 0;
            z = 0;

            Match match = _namePattern.Match(filename);
            if (!match.Success) {
                return false;
            }

            x = Convert.ToInt32(match.Groups[1].Value);
            z = Convert.ToInt32(match.Groups[2].Value);
            return true;
        }

        /// <summary>
        /// Gets the full path of the region's backing file.
        /// </summary>
        /// <returns>Gets the path of the region's file based on the <see cref="RegionManager"/>'s region path and the region's on filename.</returns>
        public string GetFilePath ()
        {
            return System.IO.Path.Combine(_regionMan.GetRegionPath(), GetFileName());
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

        /// <summary>
        /// Gets the <see cref="NbtTree"/> for a chunk given local coordinates into the region.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk within the region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk within the region.</param>
        /// <returns>An <see cref="NbtTree"/> for a local chunk, or null if there is no chunk at the given coordinates.</returns>
        public NbtTree GetChunkTree (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
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
        /// <summary>
        /// Saves an <see cref="NbtTree"/> for a chunk back to the region's data store at the given local coordinates.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of the chunk within the region.</param>
        /// <param name="lcz">The local Z-coordinate of the chunk within the region.</param>
        /// <param name="tree">The <see cref="NbtTree"/> of a chunk to write back to the region.</param>
        /// <returns>True if the save succeeded.</returns>
        /// <remarks>It is up to the programmer to ensure that the global coordinates defined within the chunk's tree
        /// are consistent with the local coordinates of the region being written into.</remarks>
        public bool SaveChunkTree (int lcx, int lcz, NbtTree tree)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? false : alt.SaveChunkTree(ForeignX(lcx), ForeignZ(lcz), tree);
            }

            RegionFile rf = GetRegionFile();
            Stream zipstr = rf.GetChunkDataOutputStream(lcx, lcz);
            if (zipstr == null) {
                return false;
            }

            tree.WriteTo(zipstr);
            zipstr.Close();

            return true;
        }

        /// <summary>
        /// Gets an output stream for replacing chunk data at the given coordinates within the region.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of the chunk to replace within the region.</param>
        /// <param name="lcz">The local Z-coordinate of the chunk to replace within the region.</param>
        /// <returns>An output stream that can be written to on demand.</returns>
        /// <remarks>There is no guarantee that any data will be saved until the stream is closed.</remarks>
        public Stream GetChunkOutStream (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunkOutStream(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            return rf.GetChunkDataOutputStream(lcx, lcz);
        }

        /// <summary>
        /// Returns the count of valid chunks stored in this region.
        /// </summary>
        /// <returns>The count of currently stored chunks.</returns>
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
        /// <summary>
        /// Gets a <see cref="ChunkRef"/> for a chunk at the given local coordinates relative to this region.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <returns>A <see cref="ChunkRef"/> at the given local coordinates, or null if no chunk exists.</returns>
        /// <remarks>The local coordinates do not strictly need to be within the bounds of the region.  If coordinates are detected
        /// as being out of bounds, the lookup will be delegated to the correct region and the lookup will be performed there
        /// instead.  This allows any <see cref="Region"/> to perform a similar task to <see cref="BetaChunkManager"/>, but with a
        /// region-local frame of reference instead of a global frame of reference.</remarks>
        public ChunkRef GetChunkRef (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
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

        /// <summary>
        /// Creates a new chunk at the given local coordinates relative to this region and returns a new <see cref="ChunkRef"/> for it.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <returns>A <see cref="ChunkRef"/> for the newly created chunk.</returns>
        /// <remarks>If the local coordinates are out of bounds for this region, the action will be forwarded to the correct region
        /// transparently.</remarks>
        public ChunkRef CreateChunk (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.CreateChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            DeleteChunk(lcx, lcz);

            int cx = lcx + _rx * XDIM;
            int cz = lcz + _rz * ZDIM;

            Chunk c = Chunk.Create(cx, cz);
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
        /// Returns a <see cref="Chunk"/> given local coordinates relative to this region.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <returns>A <see cref="Chunk"/> object for the given coordinates, or null if the chunk does not exist.</returns>
        /// <remarks>If the local coordinates are out of bounds for this region, the action will be forwarded to the correct region
        /// transparently.  The returned <see cref="Chunk"/> object may either come from cache, or be regenerated from disk.</remarks>
        public Chunk GetChunk (int lcx, int lcz)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? null : alt.GetChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            if (!ChunkExists(lcx, lcz)) {
                return null;
            }

            return Chunk.CreateVerified(GetChunkTree(lcx, lcz));
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
                Region alt = GetForeignRegion(lcx, lcz);
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
                Region alt = GetForeignRegion(lcx, lcz);
                return (alt == null) ? false : alt.DeleteChunk(ForeignX(lcx), ForeignZ(lcz));
            }

            RegionFile rf = GetRegionFile();
            if (!rf.HasChunk(lcx, lcz)) {
                return false;
            }

            rf.DeleteChunk(lcx, lcz);

            ChunkKey k = new ChunkKey(lcx, lcz);
            _cache.Remove(k);

            if (ChunkCount() == 0) {
                _regionMan.DeleteRegion(X, Z);
            }

            return true;
        }

        /// <summary>
        /// Saves an existing <see cref="Chunk"/> to the region at the given local coordinates.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <param name="chunk">A <see cref="Chunk"/> to save to the given location.</param>
        /// <returns>A <see cref="ChunkRef"/> represneting the <see cref="Chunk"/> at its new location.</returns>
        /// <remarks>If the local coordinates are out of bounds for this region, the action will be forwarded to the correct region
        /// transparently.  The <see cref="Chunk"/>'s internal global coordinates will be updated to reflect the new location.</remarks>
        public ChunkRef SetChunk (int lcx, int lcz, Chunk chunk)
        {
            if (!LocalBoundsCheck(lcx, lcz)) {
                Region alt = GetForeignRegion(lcx, lcz);
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
        public bool SaveChunk (Chunk chunk)
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

        #endregion

        private bool LocalBoundsCheck (int lcx, int lcz)
        {
            return (lcx >= 0 && lcx < XDIM && lcz >= 0 && lcz < ZDIM);
        }

        private Region GetForeignRegion (int lcx, int lcz)
        {
            return _regionMan.GetRegion(_rx + (lcx >> XLOG), _rz + (lcz >> ZLOG));
        }

        private int ForeignX (int lcx)
        {
            return (lcx + XDIM * 10000) & XMASK;
        }

        private int ForeignZ (int lcz)
        {
            return (lcz + ZDIM * 10000) & ZMASK;
        }
    }
}
