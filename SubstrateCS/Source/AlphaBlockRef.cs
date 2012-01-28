using System;
using Substrate.Core;

//TODO: Benchmark struct vs. class.  If no difference, prefer class.

namespace Substrate
{
    /// <summary>
    /// A reference to a single Alpha-compatible block in an <see cref="AlphaBlockCollection"/>.
    /// </summary>
    /// <remarks><para>The <see cref="AlphaBlockRef"/> type provides a reasonably lightweight reference to an individual block in a
    /// <see cref="AlphaBlockCollection"/>.  The <see cref="AlphaBlockRef"/> does not store any of the data itself.  If the referenced
    /// block in the <see cref="AlphaBlockCollection"/> is updated externally, those changes will be automatically reflected in the
    /// <see cref="AlphaBlockRef"/>, and any changes made via the <see cref="AlphaBlockRef"/> will be applied directly to the corresponding
    /// block within the <see cref="AlphaBlockCollection"/>.  Such changes will also set the dirty status of the <see cref="AlphaBlockCollection"/>,
    /// which can make this type particularly useful.</para>
    /// <para>Despite being lightweight, using an <see cref="AlphaBlockRef"/> to get and set block data is still more expensive then directly
    /// getting and setting data in the <see cref="AlphaBlockCollection"/> object, and can be significantly slow in a tight loop 
    /// (<see cref="AlphaBlockCollection"/> does not provide an interface for enumerating <see cref="AlphaBlockRef"/> objects specifically
    /// to discourage this kind of use).</para>
    /// <para><see cref="AlphaBlockRef"/> objects are most appropriate in cases where looking up an object requires expensive checks, such as
    /// accessing blocks through a derived <see cref="BlockManager"/> type with enhanced block filtering.  By getting an <see cref="AlphaBlockRef"/>,
    /// any number of block attributes can be read or written to while only paying the lookup cost once to get the reference.  Using the
    /// <see cref="BlockManager"/> (or similar) directly would incur the expensive lookup on each operation.  See NBToolkit for an example of this
    /// use case.</para>
    /// <para>Unlike the <see cref="AlphaBlock"/> object, this type exposed access to context-dependent data such as lighting.</para></remarks>
    public struct AlphaBlockRef : IVersion10BlockRef
    {
        private readonly AlphaBlockCollection _collection;
        private readonly int _index;

        internal AlphaBlockRef (AlphaBlockCollection collection, int index)
        {
            _collection = collection;
            _index = index;
        }

        /// <summary>
        /// Checks if this object is currently a valid ref into another <see cref="AlphaBlockCollection"/>.
        /// </summary>
        public bool IsValid
        {
            get { return _collection != null; }
        }

        #region IBlock Members

        /// <summary>
        /// Gets information on the type of the block.
        /// </summary>
        public BlockInfo Info
        {
            get { return _collection.GetInfo(_index); }
        }

        /// <summary>
        /// Gets or sets the id (type) of the block.
        /// </summary>
        public int ID
        {
            get
            {
                return _collection.GetID(_index);
            }
            set
            {
                _collection.SetID(_index, value);
            }
        }

        #endregion

        #region IDataBlock Members

        /// <summary>
        /// Gets or sets the supplementary data value of the block.
        /// </summary>
        public int Data
        {
            get
            {
                return _collection.GetData(_index);
            }
            set
            {
                _collection.SetData(_index, value);
            }
        }

        #endregion

        #region ILitBlock Members

        /// <summary>
        /// Gets or sets the block-source light component of the block.
        /// </summary>
        public int BlockLight
        {
            get
            {
                return _collection.GetBlockLight(_index);
            }
            set
            {
                _collection.SetBlockLight(_index, value);
            }
        }

        /// <summary>
        /// Gets or sets the sky-source light component of the block.
        /// </summary>
        public int SkyLight
        {
            get
            {
                return _collection.GetSkyLight(_index);
            }
            set
            {
                _collection.SetSkyLight(_index, value);
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
            return _collection.GetTileEntity(_index);
        }

        /// <summary>
        /// Sets a new Tile Entity record for the block.
        /// </summary>
        /// <param name="te">A Tile Entity record compatible with the block's type.</param>
        public void SetTileEntity (TileEntity te)
        {
            _collection.SetTileEntity(_index, te);
        }

        /// <summary>
        /// Creates a default Tile Entity record appropriate for the block.
        /// </summary>
        public void CreateTileEntity ()
        {
            _collection.CreateTileEntity(_index);
        }

        /// <summary>
        /// Removes any Tile Entity currently attached to the block.
        /// </summary>
        public void ClearTileEntity ()
        {
            _collection.ClearTileEntity(_index);
        }

        #endregion


        #region IActiveBlock Members

        public int TileTickValue
        {
            get { return _collection.GetTileTickValue(_index); }
            set { _collection.SetTileTickValue(_index, value); }
        }

        /// <summary>
        /// Gets the <see cref="TileTick"/> record of the block if it has one.
        /// </summary>
        /// <returns>The <see cref="TileTick"/> attached to this block, or null if the block type does not require a Tile Entity.</returns>
        public TileTick GetTileTick ()
        {
            return _collection.GetTileTick(_index);
        }

        /// <summary>
        /// Sets a new <see cref="TileTick"/> record for the block.
        /// </summary>
        /// <param name="te">A <see cref="TileTick"/> record compatible with the block's type.</param>
        public void SetTileTick (TileTick te)
        {
            _collection.SetTileTick(_index, te);
        }

        /// <summary>
        /// Creates a default <see cref="TileTick"/> record appropriate for the block.
        /// </summary>
        public void CreateTileTick ()
        {
            _collection.CreateTileTick(_index);
        }

        /// <summary>
        /// Removes any <see cref="TileTick"/> currently attached to the block.
        /// </summary>
        public void ClearTileTick ()
        {
            _collection.ClearTileTick(_index);
        }

        #endregion
    }
}
