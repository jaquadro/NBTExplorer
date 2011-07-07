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
    /// A basic unconstrained container of blocks.
    /// </summary>
    public interface IBlockCollection
    {
        /// <summary>
        /// Gets a basic block from a block container..
        /// </summary>
        /// <param name="x">The X-coordinate of a block.</param>
        /// <param name="y">The Y-coordinate of a block.</param>
        /// <param name="z">The Z-coordinate of a block.</param>
        /// <returns>A basic <see cref="IBlock"/> from the collection at the given coordinates.</returns>
        IBlock GetBlock (int x, int y, int z);

        /// <summary>
        /// Gets a reference object to a basic within a block container.
        /// </summary>
        /// <param name="x">The X-coordinate of a block.</param>
        /// <param name="y">The Y-coordinate of a block.</param>
        /// <param name="z">The Z-coordinate of a block.</param>
        /// <returns>A basic <see cref="IBlock"/> acting as a reference directly into the container at the given coordinates.</returns>
        IBlock GetBlockRef (int x, int y, int z);

        /// <summary>
        /// Updates a block in a block container with data from an existing <see cref="IBlock"/> object.
        /// </summary>
        /// <param name="x">The X-coordinate of a block.</param>
        /// <param name="y">The Y-coordinate of a block.</param>
        /// <param name="z">The Z-coordinate of a block.</param>
        /// <param name="block">The <see cref="IBlock"/> to copy basic data from.</param>
        void SetBlock (int x, int y, int z, IBlock block);

        /// <summary>
        /// Gets a block's id (type) from a block container.
        /// </summary>
        /// <param name="x">The X-coordinate of a block.</param>
        /// <param name="y">The Y-coordinate of a block.</param>
        /// <param name="z">The Z-coordinate of a block.</param>
        /// <returns>The block id (type) from the block container at the given coordinates.</returns>
        int GetID (int x, int y, int z);

        /// <summary>
        /// Sets a block's id (type) within a block container.
        /// </summary>
        /// <param name="x">The X-coordinate of a block.</param>
        /// <param name="y">The Y-coordinate of a block.</param>
        /// <param name="z">The Z-coordinate of a block.</param>
        /// <param name="id">The id (type) to assign to a block at the given coordinates.</param>
        void SetID (int x, int y, int z, int id);

        /// <summary>
        /// Gets info and attributes on a block's type within a block container.
        /// </summary>
        /// <param name="x">The X-coordinate of a block.</param>
        /// <param name="y">The Y-coordinate of a block.</param>
        /// <param name="z">The Z-coordinate of a block.</param>
        /// <returns>A <see cref="BlockInfo"/> instance for the block's type.</returns>
        BlockInfo GetInfo (int x, int y, int z);
    }

    /// <summary>
    /// A container of blocks with set dimensions.
    /// </summary>
    public interface IBoundedBlockCollection : IBlockCollection
    {
        int XDim { get; }
        int YDim { get; }
        int ZDim { get; }

        int CountByID (int id);
    }

    /// <summary>
    /// An unbounded container of blocks supporting data fields.
    /// </summary>
    public interface IDataBlockCollection : IBlockCollection
    {
        new IDataBlock GetBlock (int x, int y, int z);
        new IDataBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, IDataBlock block);

        int GetData (int x, int y, int z);
        void SetData (int x, int y, int z, int data);
    }

    /// <summary>
    /// A bounded version of the <see cref="IDataBlockCollection"/> interface.
    /// </summary>
    public interface IBoundedDataBlockCollection : IDataBlockCollection, IBoundedBlockCollection
    {
        int CountByData (int id, int data);
    }

    /// <summary>
    /// An unbounded container of blocks supporting dual-source lighting.
    /// </summary>
    public interface ILitBlockCollection : IBlockCollection
    {
        new ILitBlock GetBlock (int x, int y, int z);
        new ILitBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, ILitBlock block);

        // Local Light
        int GetBlockLight (int x, int y, int z);
        int GetSkyLight (int x, int y, int z);

        void SetBlockLight (int x, int y, int z, int light);
        void SetSkyLight (int x, int y, int z, int light);

        int GetHeight (int x, int z);
        void SetHeight (int x, int z, int height);

        // Update and propagate light at a single block
        void UpdateBlockLight (int x, int y, int z);
        void UpdateSkyLight (int x, int y, int z);
    }

    /// <summary>
    /// A bounded version of the <see cref="ILitBlockCollection"/> interface.
    /// </summary>
    public interface IBoundedLitBlockCollection : ILitBlockCollection, IBoundedBlockCollection
    {
        // Zero out light in entire collection
        void ResetBlockLight ();
        void ResetSkyLight ();

        // Recalculate light in entire collection
        void RebuildBlockLight ();
        void RebuildSkyLight ();
        void RebuildHeightMap ();

        // Reconcile inconsistent lighting between the edges of two containers of same size
        void StitchBlockLight ();
        void StitchSkyLight ();

        void StitchBlockLight (IBoundedLitBlockCollection blockset, BlockCollectionEdge edge);
        void StitchSkyLight (IBoundedLitBlockCollection blockset, BlockCollectionEdge edge);
    }

    /// <summary>
    /// An unbounded container for blocks supporting additional properties.
    /// </summary>
    public interface IPropertyBlockCollection : IBlockCollection
    {
        new IPropertyBlock GetBlock (int x, int y, int z);
        new IPropertyBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, IPropertyBlock block);

        TileEntity GetTileEntity (int x, int y, int z);
        void SetTileEntity (int x, int y, int z, TileEntity te);

        void CreateTileEntity (int x, int y, int z);
        void ClearTileEntity (int x, int y, int z);
    }

    /// <summary>
    /// A bounded version of the <see cref="IPropertyBlockCollection"/> interface.
    /// </summary>
    public interface IBoundedPropertyBlockCollection : IPropertyBlockCollection, IBoundedBlockCollection
    {
    }

    /// <summary>
    /// An unbounded container of blocks supporting data, lighting, and properties.
    /// </summary>
    public interface IAlphaBlockCollection : IDataBlockCollection, ILitBlockCollection, IPropertyBlockCollection
    {
        new AlphaBlock GetBlock (int x, int y, int z);
        new AlphaBlockRef GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, AlphaBlock block);
    }

    /// <summary>
    /// A bounded version of the <see cref="IAlphaBlockCollection"/> interface.
    /// </summary>
    public interface IBoundedAlphaBlockCollection : IAlphaBlockCollection, IBoundedDataBlockCollection, IBoundedLitBlockCollection, IBoundedPropertyBlockCollection
    {
    }

    /// <summary>
    /// Provides a common interface for block containers that provide global management.
    /// </summary>
    public interface IBlockManager : IAlphaBlockCollection
    { 
    }
}
