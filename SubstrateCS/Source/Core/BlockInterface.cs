using System;

namespace Substrate.Core
{
    public enum BlockCollectionEdge
    {
        EAST = 0,
        NORTH = 1,
        WEST = 2,
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
    /// Provides a common interface for block containers that provide global management.
    /// </summary>
    public interface IBlockManager : IAlphaBlockCollection
    { 
    }
}
