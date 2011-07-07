using System;
using System.Collections.Generic;
using System.IO;

namespace Substrate.Core
{

    /// <summary>
    /// Provides a common interface for accessing Alpha-compatible chunk data.
    /// </summary>
    public interface IChunk
    {
        /// <summary>
        /// Gets the global X-coordinate of a chunk.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets the global Z-coordinate of a chunk.
        /// </summary>
        int Z { get; }

        /// <summary>
        /// Gets access to an <see cref="AlphaBlockCollection"/> representing all block data of a chunk.
        /// </summary>
        AlphaBlockCollection Blocks { get; }

        /// <summary>
        /// Gets access to an <see cref="EntityCollection"/> representing all entity data of a chunk.
        /// </summary>
        EntityCollection Entities { get; }

        /// <summary>
        /// Gets or sets the flag indicating that the terrain generator has created terrain features.
        /// </summary>
        /// <remarks>Terrain features include ores, water and lava sources, dungeons, trees, flowers, etc.</remarks>
        bool IsTerrainPopulated { get; set; }

        /// <summary>
        /// Writes out the chunk's data to an output stream.
        /// </summary>
        /// <param name="outStream">A valid, open output stream.</param>
        /// <returns>True if the chunk could be saved; false otherwise.</returns>
        bool Save (Stream outStream);
    }

    /// <summary>
    /// Provides a common interface to any object that acts as a physical or abstract chunk container.
    /// </summary>
    public interface IChunkContainer
    {
        /// <summary>
        /// Returns a global chunk X-coordinate, given a container-defined X-coordinate.
        /// </summary>
        /// <param name="cx">An X-coordinate internally assigned to a <see cref="ChunkRef"/> by a <see cref="IChunkContainer"/>.</param>
        /// <returns>A corresponding global X-coordinate.</returns>
        /// <remarks>This is largely intended for internal use.  If an <see cref="IChunk"/> is assigned coordinates by an
        /// <see cref="IChunkContainer"/>, the interpretation of those coordinates is ambiguous.  This method ensures the coordinate
        /// returned is interpreted as a global coordinate.</remarks>
        int ChunkGlobalX (int cx);

        /// <summary>
        /// Returns a global chunk Z-coordinate, given a container-defined Z-coordinate.
        /// </summary>
        /// <param name="cz">A Z-coordinate internally assigned to a <see cref="ChunkRef"/> by a <see cref="IChunkContainer"/>.</param>
        /// <returns>A corresponding global Z-coordinate.</returns>
        /// <remarks>This is largely intended for internal use.  If an <see cref="IChunk"/> is assigned coordinates by an
        /// <see cref="IChunkContainer"/>, the interpretation of those coordinates is ambiguous.  This method ensures the coordinate
        /// returned is interpreted as a global coordinate.</remarks>
        int ChunkGlobalZ (int cz);

        /// <summary>
        /// Returns a local chunk X-coordinate, given a container-defined X-coordinate.
        /// </summary>
        /// <param name="cx">An X-coordinate internally assigned to a <see cref="ChunkRef"/> by a <see cref="IChunkContainer"/>.</param>
        /// <returns>A corresponding local X-coordinate.</returns>
        /// <remarks>This is largely intended for internal use.  If an <see cref="IChunk"/> is assigned coordinates by an
        /// <see cref="IChunkContainer"/>, the interpretation of those coordinates is ambiguous.  This method ensures the coordinate
        /// returned is interpreted as a local coordinate.</remarks>
        int ChunkLocalX (int cx);

        /// <summary>
        /// Returns a local chunk Z-coordinate, given a container-defined Z-coordinate.
        /// </summary>
        /// <param name="cz">A Z-coordinate internally assigned to a <see cref="ChunkRef"/> by a <see cref="IChunkContainer"/>.</param>
        /// <returns>A corresponding global X-coordinate.</returns>
        /// <remarks>This is largely intended for internal use.  If an <see cref="IChunk"/> is assigned coordinates by an
        /// <see cref="IChunkContainer"/>, the interpretation of those coordinates is ambiguous.  This method ensures the coordinate
        /// returned is interpreted as a local coordinate.</remarks>
        int ChunkLocalZ (int cz);

        /// <summary>
        /// Gets an unwrapped <see cref="Chunk"/> object for the given container-local coordinates.
        /// </summary>
        /// <param name="cx">The container-local X-coordinate of a chunk.</param>
        /// <param name="cz">The container-local Z-coordinate of a chunk.</param>
        /// <returns>A <see cref="Chunk"/> for the given coordinates, or null if no chunk exists at those coordinates.</returns>
        Chunk GetChunk (int cx, int cz);

        /// <summary>
        /// Gets a <see cref="ChunkRef"/> binding a chunk to this container for the given container-local coordinates.
        /// </summary>
        /// <param name="cx">The container-local X-coordinate of a chunk.</param>
        /// <param name="cz">The container-local Z-coordinate of a chunk.</param>
        /// <returns>A <see cref="ChunkRef"/> for the given coordinates binding a <see cref="Chunk"/> to this container, or null if
        /// no chunk exists at the given coordinates.</returns>
        ChunkRef GetChunkRef (int cx, int cz);

        /// <summary>
        /// Creates an empty chunk at the given coordinates, if no chunk previously exists.
        /// </summary>
        /// <param name="cx">The container-local X-coordinate of a chunk.</param>
        /// <param name="cz">The container-local Z-coordinate of a chunk.</param>
        /// <returns>A <see cref="ChunkRef"/> for the newly created chunk if no previous chunk existed; a <see cref="ChunkRef"/> 
        /// to the existing chunk otherwise.</returns>
        /// <remarks>This method ensures that an empty/default chunk is written out to the underlying data store before returning.</remarks>
        ChunkRef CreateChunk (int cx, int cz);

        /// <summary>
        /// Saves an unwrapped <see cref="Chunk"/> to the container at the given container-local coordinates.
        /// </summary>
        /// <param name="cx">The container-local X-coordinate to save the chunk to.</param>
        /// <param name="cz">The container-local Z-coordinate to save the chunk to.</param>
        /// <param name="chunk">The <see cref="Chunk"/> to save at the given coordinates.</param>
        /// <returns>A <see cref="ChunkRef"/> binding <paramref name="chunk"/> to this container at the given location.</returns>
        /// <remarks><para>The <see cref="Chunk"/> argument will be updated to reflect new global coordinates corresponding to
        /// the given location in this container.  It is up to the developer to ensure that no competing <see cref="ChunkRef"/>
        /// has a handle to the <see cref="Chunk"/> argument, or an inconsistency could develop where the chunk held by the
        /// other <see cref="ChunkRef"/> is written to the underlying data store with invalid coordinates.</para>
        /// <para>The <see cref="ChunkRef"/> specification is designed to avoid this situation from occuring, but
        /// class hierarchy extensions could violate these safeguards.</para></remarks>
        ChunkRef SetChunk (int cx, int cz, Chunk chunk);

        /// <summary>
        /// Checks if a chunk exists at the given container-local coordinates.
        /// </summary>
        /// <param name="cx">The container-local X-coordinate of a chunk.</param>
        /// <param name="cz">The container-local Z-coordinate of a chunk.</param>
        /// <returns>True if a chunk exists at the given coordinates; false otherwise.</returns>
        bool ChunkExists (int cx, int cz);

        /// <summary>
        /// Deletes a chunk at the given container-local coordinates if it exists.
        /// </summary>
        /// <param name="cx">The container-local X-coordinate of a chunk.</param>
        /// <param name="cz">The container-local Z-coordinate of a chunk.</param>
        /// <returns>True if a chunk existed and was deleted; false otherwise.</returns>
        bool DeleteChunk (int cx, int cz);

        /// <summary>
        /// Saves any chunks in the container that currently have unsaved changes.
        /// </summary>
        /// <returns>The number of chunks that were saved.</returns>
        /// <remarks>If this container supports delegating out-of-bounds coordinates to other containers, then any chunk
        /// modified by an action on this container that was delegated to another container will not be saved.  The foreign
        /// containers must be individually saved, but are guaranteed to know about the unsaved changes originating from
        /// an action in another container.</remarks>
        int Save ();

        // TODO: Check that this doesn't violate borders
        /// <exclude/>
        bool SaveChunk (Chunk chunk);

        /// <summary>
        /// Checks if this container supports delegating an action on out-of-bounds coordinates to another container.
        /// </summary>
        /// <remarks>If a container does not support this property, it is expected to throw <see cref="ArgumentOutOfRangeException"/>
        /// for any action on out-of-bounds coordinates.</remarks>
        bool CanDelegateCoordinates { get; }
    }

    /// <summary>
    /// Provides a common interface for chunk containers that provide global management.
    /// </summary>
    public interface IChunkManager : IChunkContainer, IEnumerable<ChunkRef>
    {

    }
}
