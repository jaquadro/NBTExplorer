using System;

namespace Substrate.Core
{
    /// <summary>
    /// Represents the cardinal direction of a block collection's neighboring collection.
    /// </summary>
    public enum BlockCollectionEdge
    {
        /// <summary>
        /// Refers to a chunk/collection to the east of the current chunk/collection.
        /// </summary>
        EAST = 0,

        /// <summary>
        /// Refers to a chunk/collection to the north of the current chunk/collection.
        /// </summary>
        NORTH = 1,

        /// <summary>
        /// Refers to a chunk/collection to the west of the current chunk/collection.
        /// </summary>
        WEST = 2,

        /// <summary>
        /// Refers to a chunk/collection to the south of the current chunk/collection.
        /// </summary>
        SOUTH = 3
    }

    /// <summary>
    /// A basic block type.
    /// </summary>
    public interface IBlock
    {
        /// <summary>
        /// Gets a variety of info and attributes on the block's type.
        /// </summary>
        BlockInfo Info { get; }

        /// <summary>
        /// Gets or sets the block's id (type).
        /// </summary>
        int ID { get; set; }
    }

    /// <summary>
    /// A block type supporting a data field.
    /// </summary>
    public interface IDataBlock : IBlock
    {
        /// <summary>
        /// Gets or sets a data value on the block.
        /// </summary>
        int Data { get; set; }
    }

    /// <summary>
    /// A block type supporting dual-source lighting.
    /// </summary>
    public interface ILitBlock : IBlock
    {
        /// <summary>
        /// Gets or sets the block-source light value on this block.
        /// </summary>
        int BlockLight { get; set; }

        /// <summary>
        /// Gets or sets the sky-source light value on this block.
        /// </summary>
        int SkyLight { get; set; }
    }

    /// <summary>
    /// A block type supporting properties.
    /// </summary>
    public interface IPropertyBlock : IBlock
    {
        /// <summary>
        /// Gets a tile entity attached to this block.
        /// </summary>
        /// <returns>A <see cref="TileEntity"/> for this block, or null if this block type does not support a tile entity.</returns>
        TileEntity GetTileEntity ();

        /// <summary>
        /// Sets the tile entity attached to this block.
        /// </summary>
        /// <param name="te">A <see cref="TileEntity"/> supported by this block type.</param>
        /// <exception cref="ArgumentException">Thrown when the <see cref="TileEntity"/> being passed is of the wrong type for the given block.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the given block is of a type that does not support a <see cref="TileEntity"/> record.</exception>
        void SetTileEntity (TileEntity te);

        /// <summary>
        /// Creates a default tile entity for this block consistent with its type.
        /// </summary>
        /// <remarks>This method will overwrite any existing <see cref="TileEntity"/> attached to the block.</remarks>
        /// <exception cref="InvalidOperationException">Thrown when the given block is of a type that does not support a <see cref="TileEntity"/> record.</exception>
        /// <exception cref="UnknownTileEntityException">Thrown when the block type requests a <see cref="TileEntity"/> that has not been registered with the <see cref="TileEntityFactory"/>.</exception>
        void CreateTileEntity ();

        /// <summary>
        /// Deletes the tile entity attached to this block if one exists.
        /// </summary>
        void ClearTileEntity ();
    }

    /// <summary>
    /// A block type supporting active tick state.
    /// </summary>
    public interface IActiveBlock : IBlock
    {
        /// <summary>
        /// Gets a <see cref="TileTick"/> entry attached to this block.
        /// </summary>
        /// <returns>A <see cref="TileTick"/> for this block, or null if one has not been created yet.</returns>
        TileTick GetTileTick ();

        /// <summary>
        /// Sets the <see cref="TileTick"/> attached to this block.
        /// </summary>
        /// <param name="tt">A <see cref="TileTick"/> representing the delay until this block is next processed in-game.</param>
        void SetTileTick (TileTick tt);

        /// <summary>
        /// Creates a default <see cref="TileTick"/> entry for this block.
        /// </summary>
        /// <remarks>This method will overwrite any existing <see cref="TileTick"/> attached to the block.</remarks>
        void CreateTileTick ();

        /// <summary>
        /// Deletes the <see cref="TileTick"/> entry attached to this block, if one exists.
        /// </summary>
        void ClearTileTick ();

        /// <summary>
        /// Gets or sets the the actual tick delay associated with this block.
        /// </summary>
        /// <remarks>If no underlying <see cref="TileTick"/> entry exists for this block, one will be created.</remarks>
        int TileTickValue { get; set; }
    }

    /// <summary>
    /// An Alpha-compatible context-free block type supporting data and properties.
    /// </summary>
    public interface IAlphaBlock : IDataBlock, IPropertyBlock
    {
    }

    /// <summary>
    /// An Alpha-compatible block reference type supporting data, lighting, and properties.
    /// </summary>
    public interface IAlphaBlockRef : IDataBlock, ILitBlock, IPropertyBlock
    {
        /// <summary>
        /// Checks if the reference and its backing container are currently valid.
        /// </summary>
        bool IsValid { get; }
    }

    /// <summary>
    /// A version-1.0-compatible context-free block type supporting data, properties, and active tick state.
    /// </summary>
    public interface IVersion10Block : IAlphaBlock, IActiveBlock
    {
    }

    /// <summary>
    /// A version-1.0-compatible reference type supporting data, lighting, properties, and active tick state.
    /// </summary>
    public interface IVersion10BlockRef : IAlphaBlockRef, IActiveBlock
    {
    }

    /// <summary>
    /// Provides a common interface for block containers that provide global management.
    /// </summary>
    public interface IBlockManager : IAlphaBlockCollection
    { 
    }

    /// <summary>
    /// Provides a common interface for block containers that provide global management, extended through Version 1.0 capabilities.
    /// </summary>
    public interface IVersion10BlockManager : IBlockManager, IActiveBlockCollection
    {
    }
}
