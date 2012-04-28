using System.Collections.Generic;
using System.IO;
using Substrate.Nbt;

namespace Substrate.Core
{
    public interface IRegion : IChunkContainer
    {
        /// <summary>
        /// Gets the global X-coordinate of the region.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets the global Z-coordinate of the region.
        /// </summary>
        int Z { get; }

        /// <summary>
        /// Get the appropriate filename for this region.
        /// </summary>
        /// <returns>The filename of the region with encoded coordinates.</returns>
        string GetFileName ();

        /// <summary>
        /// Gets the full path of the region's backing file.
        /// </summary>
        /// <returns>Gets the path of the region's file based on the <see cref="IRegionManager"/>'s region path and the region's on filename.</returns>
        string GetFilePath ();

        /// <summary>
        /// Gets the <see cref="NbtTree"/> for a chunk given local coordinates into the region.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk within the region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk within the region.</param>
        /// <returns>An <see cref="NbtTree"/> for a local chunk, or null if there is no chunk at the given coordinates.</returns>
        NbtTree GetChunkTree (int lcx, int lcz);

        /// <summary>
        /// Saves an <see cref="NbtTree"/> for a chunk back to the region's data store at the given local coordinates.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of the chunk within the region.</param>
        /// <param name="lcz">The local Z-coordinate of the chunk within the region.</param>
        /// <param name="tree">The <see cref="NbtTree"/> of a chunk to write back to the region.</param>
        /// <returns>True if the save succeeded.</returns>
        /// <remarks>It is up to the programmer to ensure that the global coordinates defined within the chunk's tree
        /// are consistent with the local coordinates of the region being written into.</remarks>
        bool SaveChunkTree (int lcx, int lcz, NbtTree tree);

        /// <summary>
        /// Saves an <see cref="NbtTree"/> for a chunk back to the region's data store at the given local coordinates and with the given timestamp.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of the chunk within the region.</param>
        /// <param name="lcz">The local Z-coordinate of the chunk within the region.</param>
        /// <param name="tree">The <see cref="NbtTree"/> of a chunk to write back to the region.</param>
        /// <param name="timestamp">The timestamp to write to the underlying region file for this chunk.</param>
        /// <returns>True if the save succeeded.</returns>
        /// <remarks>It is up to the programmer to ensure that the global coordinates defined within the chunk's tree
        /// are consistent with the local coordinates of the region being written into.</remarks>
        bool SaveChunkTree (int lcx, int lcz, NbtTree tree, int timestamp);

        /// <summary>
        /// Gets an output stream for replacing chunk data at the given coordinates within the region.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of the chunk to replace within the region.</param>
        /// <param name="lcz">The local Z-coordinate of the chunk to replace within the region.</param>
        /// <returns>An output stream that can be written to on demand.</returns>
        /// <remarks>There is no guarantee that any data will be saved until the stream is closed.</remarks>
        Stream GetChunkOutStream (int lcx, int lcz);

        /// <summary>
        /// Returns the count of valid chunks stored in this region.
        /// </summary>
        /// <returns>The count of currently stored chunks.</returns>
        int ChunkCount ();

        /// <summary>
        /// Gets a <see cref="ChunkRef"/> for a chunk at the given local coordinates relative to this region.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <returns>A <see cref="ChunkRef"/> at the given local coordinates, or null if no chunk exists.</returns>
        /// <remarks>The local coordinates do not strictly need to be within the bounds of the region.  If coordinates are detected
        /// as being out of bounds, the lookup will be delegated to the correct region and the lookup will be performed there
        /// instead.  This allows any <see cref="Region"/> to perform a similar task to <see cref="RegionChunkManager"/>, but with a
        /// region-local frame of reference instead of a global frame of reference.</remarks>
        ChunkRef GetChunkRef (int lcx, int lcz);

        /// <summary>
        /// Creates a new chunk at the given local coordinates relative to this region and returns a new <see cref="ChunkRef"/> for it.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <returns>A <see cref="ChunkRef"/> for the newly created chunk.</returns>
        /// <remarks>If the local coordinates are out of bounds for this region, the action will be forwarded to the correct region
        /// transparently.</remarks>
        ChunkRef CreateChunk (int lcx, int lcz);

        /// <summary>
        /// Gets the timestamp of a chunk from the underlying region file.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <returns>The timestamp of the chunk slot in the region.</returns>
        /// <remarks>The value returned may differ from any timestamp stored in the chunk data itself.</remarks>
        int GetChunkTimestamp (int lcx, int lcz);

        /// <summary>
        /// Sets the timestamp of a chunk in the underlying region file.
        /// </summary>
        /// <param name="lcx">The local X-coordinate of a chunk relative to this region.</param>
        /// <param name="lcz">The local Z-coordinate of a chunk relative to this region.</param>
        /// <param name="timestamp">The new timestamp value.</param>
        /// <remarks>This function will only update the timestamp of the chunk slot in the underlying region file.  It will not update
        /// any timestamp information in the chunk data itself.</remarks>
        void SetChunkTimestamp (int lcx, int lcz, int timestamp);
    }

    public interface IRegionContainer
    {
        /// <summary>
        /// Determines if a region exists at the given coordinates.
        /// </summary>
        /// <param name="rx">The global X-coordinate of a region.</param>
        /// <param name="rz">The global Z-coordinate of a region.</param>
        /// <returns>True if a region exists at the given global region coordinates; false otherwise.</returns>
        bool RegionExists (int rx, int rz);

        /// <summary>
        /// Gets an <see cref="IRegion"/> for the given region filename.
        /// </summary>
        /// <param name="filename">The filename of the region to get.</param>
        /// <returns>A <see cref="IRegion"/> corresponding to the coordinates encoded in the filename.</returns>
        IRegion GetRegion (int rx, int rz);

        /// <summary>
        /// Creates a new empty region at the given coordinates, if no region exists.
        /// </summary>
        /// <param name="rx">The global X-coordinate of a region.</param>
        /// <param name="rz">The global Z-coordinate of a region.</param>
        /// <returns>A new empty <see cref="IRegion"/> object for the given coordinates, or an existing <see cref="IRegion"/> if one exists.</returns>
        IRegion CreateRegion (int rx, int rz);

        /// <summary>
        /// Deletes a region at the given coordinates.
        /// </summary>
        /// <param name="rx">The global X-coordinate of a region.</param>
        /// <param name="rz">The global Z-coordinate of a region.</param>
        /// <returns>True if a region was deleted; false otherwise.</returns>
        bool DeleteRegion (int rx, int rz);
    }

    public interface IRegionManager : IRegionContainer, IEnumerable<IRegion>
    {

    }
}
