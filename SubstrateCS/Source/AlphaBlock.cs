using System;
using Substrate.Core;

namespace Substrate
{
    /// <summary>
    /// A single Alpha-compatible block with context-independent data.
    /// </summary>
    /// <remarks><para>In general, you should prefer other types for accessing block data including <see cref="AlphaBlockRef"/>,
    /// <see cref="BlockManager"/>, and the <see cref="AlphaBlockCollection"/> property of <see cref="IChunk"/> and <see cref="ChunkRef"/>.</para>
    /// <para>You should use the <see cref="AlphaBlock"/> type when you need to copy individual blocks into a custom collection or
    /// container, and context-depdendent data such as coordinates and lighting have no well-defined meaning.  <see cref="AlphaBlock"/>
    /// offers a relatively compact footprint for storing the unique identity of a block's manifestation in the world.</para>
    /// <para>A single <see cref="AlphaBlock"/> object may also provide a convenient way to paste a block into many locations in
    /// a block collection type.</para></remarks>
    public class AlphaBlock : IDataBlock, IPropertyBlock, IActiveBlock, ICopyable<AlphaBlock>
    {
        private int _id;
        private int _data;

        private TileEntity _tileEntity;
        private TileTick _tileTick;

        /// <summary>
        /// Create a new <see cref="AlphaBlock"/> instance of the given type with default data.
        /// </summary>
        /// <param name="id">The id (type) of the block.</param>
        /// <remarks>If the specified block type requires a Tile Entity as part of its definition, a default
        /// <see cref="TileEntity"/> of the appropriate type will automatically be created.</remarks>
        public AlphaBlock (int id)
        {
            _id = id;
            UpdateTileEntity(0, id);
        }

        /// <summary>
        /// Create a new <see cref="AlphaBlock"/> instance of the given type and data value.
        /// </summary>
        /// <param name="id">The id (type) of the block.</param>
        /// <param name="data">The block's supplementary data value, currently limited to the range [0-15].</param>
        /// <remarks>If the specified block type requires a Tile Entity as part of its definition, a default
        /// <see cref="TileEntity"/> of the appropriate type will automatically be created.</remarks>
        public AlphaBlock (int id, int data)
        {
            _id = id;
            _data = data;
            UpdateTileEntity(0, id);
        }

        /// <summary>
        /// Crrates a new <see cref="AlphaBlock"/> from a given block in an existing <see cref="AlphaBlockCollection"/>.
        /// </summary>
        /// <param name="chunk">The block collection to reference.</param>
        /// <param name="lx">The local X-coordinate of a block within the collection.</param>
        /// <param name="ly">The local Y-coordinate of a block within the collection.</param>
        /// <param name="lz">The local Z-coordinate of a block within the collection.</param>
        public AlphaBlock (AlphaBlockCollection chunk, int lx, int ly, int lz)
        {
            _id = chunk.GetID(lx, ly, lz);
            _data = chunk.GetData(lx, ly, lz);

            TileEntity te = chunk.GetTileEntity(lx, ly, lz);
            if (te != null) {
                _tileEntity = te.Copy();
            }
        }


        #region IBlock Members

        /// <summary>
        /// Gets information on the type of the block.
        /// </summary>
        public BlockInfo Info
        {
            get { return BlockInfo.BlockTable[_id]; }
        }

        /// <summary>
        /// Gets or sets the id (type) of the block.
        /// </summary>
        /// <remarks>If the new or old type have non-matching Tile Entity requirements, the embedded Tile Entity data
        /// will be updated to keep consistent with the new block type.</remarks>
        public int ID
        {
            get { return _id; }
            set
            {
                UpdateTileEntity(_id, value);
                _id = value;
            }
        }

        #endregion


        #region IDataBlock Members

        /// <summary>
        /// Gets or sets the supplementary data value of the block.
        /// </summary>
        public int Data
        {
            get { return _data; }
            set
            {
                /*if (BlockManager.EnforceDataLimits && BlockInfo.BlockTable[_id] != null) {
                    if (!BlockInfo.BlockTable[_id].TestData(value)) {
                        return;
                    }
                }*/
                _data = value;
            }
        }

        #endregion


        #region IPropertyBlock Members

        /// <summary>
        /// Gets the Tile Entity record of the block if it has one.
        /// </summary>
        /// <returns>The <see cref="TileEntity"/> attached to this block, or null if the block type does not require a Tile Entity.</returns>
        public TileEntity GetTileEntity ()
        {
            return _tileEntity;
        }

        /// <summary>
        /// Sets a new Tile Entity record for the block.
        /// </summary>
        /// <param name="te">A Tile Entity record compatible with the block's type.</param>
        /// <exception cref="ArgumentException">Thrown when an incompatible <see cref="TileEntity"/> is added to a block.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a <see cref="TileEntity"/> is added to a block that does not use tile entities.</exception>
        public void SetTileEntity (TileEntity te)
        {
            BlockInfoEx info = BlockInfo.BlockTable[_id] as BlockInfoEx;
            if (info == null) {
                throw new InvalidOperationException("The current block type does not accept a Tile Entity");
            }

            if (te.GetType() != TileEntityFactory.Lookup(info.TileEntityName)) {
                throw new ArgumentException("The current block type is not compatible with the given Tile Entity", "te");
            }

            _tileEntity = te;
        }

        /// <summary>
        /// Creates a default Tile Entity record appropriate for the block.
        /// </summary>
        public void CreateTileEntity ()
        {
            BlockInfoEx info = BlockInfo.BlockTable[_id] as BlockInfoEx;
            if (info == null) {
                throw new InvalidOperationException("The given block is of a type that does not support TileEntities.");
            }

            TileEntity te = TileEntityFactory.Create(info.TileEntityName);
            if (te == null) {
                throw new UnknownTileEntityException("The TileEntity type '" + info.TileEntityName + "' has not been registered with the TileEntityFactory.");
            }

            _tileEntity = te;
        }

        /// <summary>
        /// Removes any Tile Entity currently attached to the block.
        /// </summary>
        public void ClearTileEntity ()
        {
            _tileEntity = null;
        }

        #endregion


        #region IActiveBlock Members

        public int TileTickValue
        {
            get
            {
                if (_tileTick == null)
                    return 0;
                return _tileTick.Ticks;
            }

            set
            {
                if (_tileTick == null)
                    CreateTileTick();
                _tileTick.Ticks = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="TileTick"/> record of the block if it has one.
        /// </summary>
        /// <returns>The <see cref="TileTick"/> attached to this block, or null if the block type does not require a Tile Entity.</returns>
        public TileTick GetTileTick ()
        {
            return _tileTick;
        }

        /// <summary>
        /// Sets a new <see cref="TileTick"/> record for the block.
        /// </summary>
        /// <param name="tt">A <see cref="TileTick"/> record compatible with the block's type.</param>
        public void SetTileTick (TileTick tt)
        {
            _tileTick = tt;
        }

        /// <summary>
        /// Creates a default <see cref="TileTick"/> record appropriate for the block.
        /// </summary>
        public void CreateTileTick ()
        {
            _tileTick = new TileTick()
            {
                ID = _id,
            };
        }

        /// <summary>
        /// Removes any <see cref="TileTick"/> currently attached to the block.
        /// </summary>
        public void ClearTileTick ()
        {
            _tileTick = null;
        }

        #endregion


        #region ICopyable<Block> Members

        /// <summary>
        /// Creates a deep copy of the <see cref="AlphaBlock"/>.
        /// </summary>
        /// <returns>A new <see cref="AlphaBlock"/> representing the same data.</returns>
        public AlphaBlock Copy ()
        {
            AlphaBlock block = new AlphaBlock(_id, _data);
            if (_tileEntity != null) {
                block._tileEntity = _tileEntity.Copy();
            }

            return block;
        }

        #endregion

        private void UpdateTileEntity (int old, int value)
        {
            BlockInfoEx info1 = BlockInfo.BlockTable[old] as BlockInfoEx;
            BlockInfoEx info2 = BlockInfo.BlockTable[value] as BlockInfoEx;

            if (info1 != info2) {
                if (info1 != null) {
                    _tileEntity = null;
                }

                if (info2 != null) {
                    _tileEntity = TileEntityFactory.Create(info2.TileEntityName);
                }
            }
        }
    }
}
