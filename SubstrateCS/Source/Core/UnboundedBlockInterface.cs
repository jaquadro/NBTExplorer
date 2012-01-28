using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Core
{
    /// <summary>
    /// A basic unconstrained container of blocks.
    /// </summary>
    /// <remarks>The scope of coordinates is undefined for unconstrained block containers.</remarks>
    public interface IBlockCollection
    {
        /// <summary>
        /// Gets a basic block from an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>A basic <see cref="IBlock"/> from the collection at the given coordinates.</returns>
        IBlock GetBlock (int x, int y, int z);

        /// <summary>
        /// Gets a reference object to a basic block within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>A basic <see cref="IBlock"/> acting as a reference directly into the container at the given coordinates.</returns>
        IBlock GetBlockRef (int x, int y, int z);

        /// <summary>
        /// Updates a block in an unbounded block container with data from an existing <see cref="IBlock"/> object.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="block">The <see cref="IBlock"/> to copy basic data from.</param>
        void SetBlock (int x, int y, int z, IBlock block);

        /// <summary>
        /// Gets a block's id (type) from an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>The block id (type) from the block container at the given coordinates.</returns>
        int GetID (int x, int y, int z);

        /// <summary>
        /// Sets a block's id (type) within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="id">The id (type) to assign to a block at the given coordinates.</param>
        void SetID (int x, int y, int z, int id);

        /// <summary>
        /// Gets info and attributes on a block's type within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>A <see cref="BlockInfo"/> instance for the block's type.</returns>
        BlockInfo GetInfo (int x, int y, int z);
    }

    /// <summary>
    /// An unbounded container of blocks supporting data fields.
    /// </summary>
    /// <seealso cref="IBoundedDataBlockCollection"/>
    public interface IDataBlockCollection : IBlockCollection
    {
        /// <summary>
        /// Gets a block with data field from an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>An <see cref="IDataBlock"/> from the collection at the given coordinates.</returns>
        new IDataBlock GetBlock (int x, int y, int z);

        /// <summary>
        /// Gets a reference object to a block with data field within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>An <see cref="IDataBlock"/> acting as a reference directly into the container at the given coordinates.</returns>
        new IDataBlock GetBlockRef (int x, int y, int z);

        /// <summary>
        /// Updates a block in an unbounded block container with data from an existing <see cref="IDataBlock"/> object.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="block">The <see cref="IDataBlock"/> to copy data from.</param>
        void SetBlock (int x, int y, int z, IDataBlock block);

        /// <summary>
        /// Gets a block's data field from an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>The data field of a block at the given coordinates.</returns>
        int GetData (int x, int y, int z);

        /// <summary>
        /// Sets a block's data field within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="data">The data field to assign to a block at the given coordinates.</param>
        void SetData (int x, int y, int z, int data);
    }

    /// <summary>
    /// An unbounded container of blocks supporting dual-source lighting.
    /// </summary>
    /// <seealso cref="IBoundedLitBlockCollection"/>
    public interface ILitBlockCollection : IBlockCollection
    {
        /// <summary>
        /// Gets a block with lighting information from an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>An <see cref="ILitBlock"/> from the collection at the given coordinates.</returns>
        new ILitBlock GetBlock (int x, int y, int z);

        /// <summary>
        /// Gets a reference object to a block with lighting information within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>An <see cref="ILitBlock"/> acting as a reference directly into the container at the given coordinates.</returns>
        new ILitBlock GetBlockRef (int x, int y, int z);

        /// <summary>
        /// Updates a block in an unbounded block container with data from an existing <see cref="ILitBlock"/> object.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="block">The <see cref="ILitBlock"/> to copy data from.</param>
        void SetBlock (int x, int y, int z, ILitBlock block);

        /// <summary>
        /// Gets a block's block-source light value from an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>The block-source light value of a block at the given coordinates.</returns>
        int GetBlockLight (int x, int y, int z);

        /// <summary>
        /// Gets a block's sky-source light value from an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>The sky-source light value of a block at the given coordinates.</returns>
        int GetSkyLight (int x, int y, int z);

        /// <summary>
        /// Sets a block's block-source light value within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="light">The block-source light value to assign to a block at the given coordinates.</param>
        void SetBlockLight (int x, int y, int z, int light);

        /// <summary>
        /// Sets a block's sky-source light value within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="light">The sky-source light value to assign to a block at the given coordinates.</param>
        void SetSkyLight (int x, int y, int z, int light);

        /// <summary>
        /// Gets the Y-coordinate of the lowest block with unobstructed view of the sky at the given coordinates within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>The height value of an X-Z coordinate pair in the block container.</returns>
        /// <remarks>The height value represents the lowest block with an unobstructed view of the sky.  This is the lowest block with
        /// a maximum-value sky-light value.  Fully transparent blocks, like glass, do not count as an obstruction.</remarks>
        int GetHeight (int x, int z);

        /// <summary>
        /// Sets the Y-coordinate of the lowest block with unobstructed view of the sky at the given coordinates within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="height">The height value of an X-Z coordinate pair in the block container.</param>
        /// <remarks>Minecraft lighting algorithms rely heavily on this value being correct.  Setting this value too low may result in
        /// rooms that can never get dark, for example.</remarks>
        void SetHeight (int x, int z, int height);

        /// <summary>
        /// Recalculates the block-source light value of a single block within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <remarks><para>The lighting of the block will be updated to be consistent with the lighting in neighboring blocks.
        /// If the block is itself a light source, many nearby blocks may be updated to maintain consistent lighting.  These
        /// updates may also touch neighboring <see cref="ILitBlockCollection"/> objects, if they can be resolved.</para>
        /// <para>This function assumes that the entire <see cref="ILitBlockCollection"/> and neighboring <see cref="ILitBlockCollection"/>s
        /// already have consistent lighting, with the exception of the block being updated.  If this assumption is violated, 
        /// lighting may fail to converge correctly.</para></remarks>
        void UpdateBlockLight (int x, int y, int z);

        /// <summary>
        /// Recalculates the sky-source light value of a single block within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <remarks><para>The lighting of the block will be updated to be consistent with the lighting in neighboring blocks.
        /// If the block is itself a light source, many nearby blocks may be updated to maintain consistent lighting.  These
        /// updates may also touch neighboring <see cref="ILitBlockCollection"/> objects, if they can be resolved.</para>
        /// <para>This function assumes that the entire <see cref="ILitBlockCollection"/> and neighboring <see cref="ILitBlockCollection"/>s
        /// already have consistent lighting, with the exception of the block being updated.  If this assumption is violated,
        /// lighting may fail to converge correctly.</para></remarks>
        void UpdateSkyLight (int x, int y, int z);
    }

    /// <summary>
    /// An unbounded container for blocks supporting extra properties.
    /// </summary>
    /// <seealso cref="IBoundedPropertyBlockCollection"/>
    public interface IPropertyBlockCollection : IBlockCollection
    {
        /// <summary>
        /// Gets a block supporting extra properties from an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>An <see cref="IPropertyBlock"/> from the collection at the given coordinates.</returns>
        new IPropertyBlock GetBlock (int x, int y, int z);

        /// <summary>
        /// Gets a reference object to a block supporting extra properties within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>An <see cref="IPropertyBlock"/> acting as a reference directly into the container at the given coordinates.</returns>
        new IPropertyBlock GetBlockRef (int x, int y, int z);

        /// <summary>
        /// Updates a block in an unbounded block container with data from an existing <see cref="IPropertyBlock"/> object.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="block">The <see cref="IPropertyBlock"/> to copy data from.</param>
        void SetBlock (int x, int y, int z, IPropertyBlock block);

        /// <summary>
        /// Gets the <see cref="TileEntity"/> record of a block within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>A <see cref="TileEntity"/> record attached to a block at the given coordinates, or null if no tile entity is set.</returns>
        TileEntity GetTileEntity (int x, int y, int z);

        /// <summary>
        /// Sets a <see cref="TileEntity"/> record to a block within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="te">The <see cref="TileEntity"/> record to assign to the given block.</param>
        /// <exception cref="ArgumentException">Thrown when an incompatible <see cref="TileEntity"/> is added to a block.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a <see cref="TileEntity"/> is added to a block that does not use tile entities.</exception>
        void SetTileEntity (int x, int y, int z, TileEntity te);

        /// <summary>
        /// Creates a new default <see cref="TileEntity"/> record for a block within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <exception cref="InvalidOperationException">Thrown when a <see cref="TileEntity"/> is created for a block that does not use tile entities.</exception>
        /// <exception cref="UnknownTileEntityException">Thrown when the tile entity type associated with the given block has not been registered with <see cref="TileEntityFactory"/>.</exception>
        void CreateTileEntity (int x, int y, int z);

        /// <summary>
        /// Deletes a <see cref="TileEntity"/> record associated with a block within an unbounded block container, if it exists.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        void ClearTileEntity (int x, int y, int z);
    }

    /// <summary>
    /// An unbounded container for blocks supporting active processing properties.
    /// </summary>
    /// <seealso cref="IBoundedActiveBlockCollection"/>
    public interface IActiveBlockCollection : IBlockCollection
    {
        /// <summary>
        /// Gets a block supporting active processing properties from an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>An <see cref="IActiveBlock"/> from the collection at the given coordinates.</returns>
        new IActiveBlock GetBlock (int x, int y, int z);

        /// <summary>
        /// Gets a reference object to a block supporting active processing properties within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>An <see cref="IActiveBlock"/> acting as a reference directly into the container at the given coordinates.</returns>
        new IActiveBlock GetBlockRef (int x, int y, int z);

        /// <summary>
        /// Updates a block in an unbounded block container with data from an existing <see cref="IActiveBlock"/> object.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="block">The <see cref="IActiveBlock"/> to copy data from.</param>
        void SetBlock (int x, int y, int z, IActiveBlock block);

        /// <summary>
        /// Gets the <see cref="TileTick"/> record of a block within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>A <see cref="TileTick"/> record attached to a block at the given coordinates, or null if no tile entity is set.</returns>
        TileTick GetTileTick (int x, int y, int z);

        /// <summary>
        /// Sets a <see cref="TileTick"/> record to a block within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="tt">The <see cref="TileTick"/> record to assign to the given block.</param>
        void SetTileTick (int x, int y, int z, TileTick tt);

        /// <summary>
        /// Creates a new default <see cref="TileTick"/> record for a block within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        void CreateTileTick (int x, int y, int z);

        /// <summary>
        /// Deletes a <see cref="TileTick"/> record associated with a block within an unbounded block container, if it exists.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        void ClearTileTick (int x, int y, int z);

        /// <summary>
        /// Gets the tick delay specified in a block's <see cref="TileTick"/> entry, if it exists.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>The tick delay in a block's <see cref="TileTick"/> entry, or <c>0</c> if no entry exists.</returns>
        int GetTileTickValue (int x, int y, int z);

        /// <summary>
        /// Sets the tick delay in a block's <see cref="TileTick"/> entry.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="tickValue">The tick delay that specifies when this block should next be processed for update.</param>
        void SetTileTickValue (int x, int y, int z, int tickValue);
    }

    /// <summary>
    /// An unbounded container of blocks supporting data, lighting, and properties.
    /// </summary>
    /// <seealso cref="IBoundedAlphaBlockCollection"/>
    public interface IAlphaBlockCollection : IDataBlockCollection, ILitBlockCollection, IPropertyBlockCollection
    {
        /// <summary>
        /// Gets a context-insensitive Alpha-compatible block from an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>An <see cref="AlphaBlock"/> from the collection at the given coordinates.</returns>
        new AlphaBlock GetBlock (int x, int y, int z);

        /// <summary>
        /// Gets a reference object to a context-insensitive Alpha-compatible block within an unbounded block container.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <returns>An <see cref="AlphaBlockRef"/> acting as a reference directly into the container at the given coordinates.</returns>
        new AlphaBlockRef GetBlockRef (int x, int y, int z);

        /// <summary>
        /// Updates a block in an unbounded block container with data from an existing <see cref="AlphaBlock"/> object.
        /// </summary>
        /// <param name="x">The global X-coordinate of a block.</param>
        /// <param name="y">The global Y-coordinate of a block.</param>
        /// <param name="z">The global Z-coordinate of a block.</param>
        /// <param name="block">The <see cref="AlphaBlock"/> to copy data from.</param>
        void SetBlock (int x, int y, int z, AlphaBlock block);
    }
}
