using System;
using System.Collections.Generic;
using Substrate.Core;
using Substrate.NBT;

namespace Substrate
{
    /// <summary>
    /// Functions for reading and modifying a bounded-size collection of Alpha-compatible block data.
    /// </summary>
    /// <remarks>An <see cref="AlphaBlockCollection"/> is a wrapper around existing pieces of data.  Although it
    /// holds references to data, it does not "own" the data in the same way that a <see cref="Chunk"/> does.  An
    /// <see cref="AlphaBlockCollection"/> simply overlays a higher-level interface on top of existing data.</remarks>
    public class AlphaBlockCollection : IBoundedAlphaBlockCollection
    {
        private readonly int _xdim;
        private readonly int _ydim;
        private readonly int _zdim;

        private XZYByteArray _blocks;
        private XZYNibbleArray _data;
        private XZYNibbleArray _blockLight;
        private XZYNibbleArray _skyLight;
        private ZXByteArray _heightMap;

        private TagNodeList _tileEntities;

        private BlockLight _lightManager;
        private BlockFluid _fluidManager;
        private BlockTileEntities _tileEntityManager;

        private bool _dirty = false;
        private bool _autoLight = true;
        private bool _autoFluid = false;

        public delegate AlphaBlockCollection NeighborLookupHandler (int relx, int rely, int relz);

        public event NeighborLookupHandler ResolveNeighbor
        {
            add 
            {
                _lightManager.ResolveNeighbor += delegate(int relx, int rely, int relz) { 
                    return value(relx, rely, relz); 
                };
                _fluidManager.ResolveNeighbor += delegate(int relx, int rely, int relz)
                {
                    return value(relx, rely, relz);
                };
            }

            remove
            {
                _lightManager = new BlockLight(this);
                _fluidManager = new BlockFluid(this);
            }
        }

        public event BlockCoordinateHandler TranslateCoordinates
        {
            add { _tileEntityManager.TranslateCoordinates += value; }
            remove { _tileEntityManager.TranslateCoordinates -= value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether changes to blocks will trigger automatic lighting updates.
        /// </summary>
        /// <remarks>Automatic updates to lighting may spill into neighboring <see cref="AlphaBlockCollection"/> objects, if they can
        /// be resolved.</remarks>
        public bool AutoLight
        {
            get { return _autoLight; }
            set { _autoLight = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether changes to blocks will trigger automatic fluid updates.
        /// </summary>
        /// <remarks>Automatic updates to fluid may cascade through neighboring <see cref="AlphaBlockCollection"/> objects and beyond,
        /// if they can be resolved.</remarks>
        public bool AutoFluid
        {
            get { return _autoFluid; }
            set { _autoFluid = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AlphaBlockCollection"/> needs to be saved.
        /// </summary>
        /// <remarks>If this <see cref="AlphaBlockCollection"/> is backed by a reference conainer type, set this property to false
        /// to prevent any modifications from being saved.  The next update will set this property true again, however.</remarks>
        public bool IsDirty
        {
            get { return _dirty; }
            set { _dirty = value; }
        }

        /// <summary>
        /// Creates a new <see cref="AlphaBlockCollection"/> overlay on top of Alpha-specific units of data.
        /// </summary>
        /// <param name="blocks">An array of Block IDs.</param>
        /// <param name="data">An array of data nibbles.</param>
        /// <param name="blockLight">An array of block light nibbles.</param>
        /// <param name="skyLight">An array of sky light nibbles.</param>
        /// <param name="heightMap">An array of height map values.</param>
        /// <param name="tileEntities">A list of tile entities corresponding to blocks in this collection.</param>
        public AlphaBlockCollection (
            XZYByteArray blocks, 
            XZYNibbleArray data, 
            XZYNibbleArray blockLight, 
            XZYNibbleArray skyLight, 
            ZXByteArray heightMap, 
            TagNodeList tileEntities)
        {
            _blocks = blocks;
            _data = data;
            _blockLight = blockLight;
            _skyLight = skyLight;
            _heightMap = heightMap;
            _tileEntities = tileEntities;

            _xdim = _blocks.XDim;
            _ydim = _blocks.YDim;
            _zdim = _blocks.ZDim;

            _lightManager = new BlockLight(this);
            _fluidManager = new BlockFluid(this);
            _tileEntityManager = new BlockTileEntities(_blocks, _tileEntities);
        }

        /// <summary>
        /// Returns a new <see cref="Block"/> object from local coordinates relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of block.</param>
        /// <param name="y">Local Y-coordinate of block.</param>
        /// <param name="z">Local Z-coordiante of block.</param>
        /// <returns>A new <see cref="Block"/> object representing context-independent data of a single block.</returns>
        /// <remarks>Context-independent data excludes data such as lighting.  <see cref="Block"/> object actually contain a copy
        /// of the data they represent, so changes to the <see cref="Block"/> will not affect this container, and vice-versa.</remarks>
        public Block GetBlock (int x, int y, int z)
        {
            return new Block(this, x, y, z);
        }

        /// <summary>
        /// Returns a new <see cref="BlockRef"/> object from local coordaintes relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of block.</param>
        /// <param name="y">Local Y-coordinate of block.</param>
        /// <param name="z">Local Z-coordinate of block.</param>
        /// <returns>A new <see cref="BlockRef"/> object representing context-dependent data of a single block.</returns>
        /// <remarks>Context-depdendent data includes all data associated with this block.  Since a <see cref="BlockRef"/> represents
        /// a view of a block within this container, any updates to data in the container will be reflected in the <see cref="BlockRef"/>,
        /// and vice-versa for updates to the <see cref="BlockRef"/>.</remarks>
        public BlockRef GetBlockRef (int x, int y, int z)
        {
            return new BlockRef(this, x, y, z);
        }

        /// <summary>
        /// Updates a block in this collection with values from a <see cref="Block"/> object.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="block">A <see cref="Block"/> object to copy block data from.</param>
        public void SetBlock (int x, int y, int z, Block block)
        {
            SetID(x, y, z, block.ID);
            SetData(x, y, z, block.Data);

            SetTileEntity(x, y, z, block.GetTileEntity().Copy());
        }

        #region IBlockCollection Members

        /// <summary>
        /// Gets the length of this collection's X-dimension.
        /// </summary>
        public int XDim
        {
            get { return _xdim; }
        }

        /// <summary>
        /// Gets the length of this collection's Y-dimension.
        /// </summary>
        public int YDim
        {
            get { return _ydim; }
        }

        /// <summary>
        /// Gets the length of this collection's Z-dimension.
        /// </summary>
        public int ZDim
        {
            get { return _zdim; }
        }

        /// <summary>
        /// Returns an object compatible with the <see cref="IBlock"/> interface from local coordinates relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>An <see cref="IBlock"/>-compatible object.</returns>
        /// <seealso cref="Block"/>
        IBlock IBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        /// <summary>
        /// Returns a reference object compatible with the <see cref="IBlock"/> interface from local coordinates relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>An <see cref="IBlock"/>-compatible reference object.</returns>
        /// <seealso cref="BlockRef"/>
        IBlock IBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        /// <summary>
        /// Updates a block in this collection with values from an <see cref="IBlock"/> object.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="block">An <see cref="IBlock"/> object to copy block data from.</param>
        public void SetBlock (int x, int y, int z, IBlock block)
        {
            SetID(x, y, z, block.ID);
        }

        /// <summary>
        /// Gets information on the type of a block at the given local coordinates.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>A <see cref="BlockInfo"/> object containing information of a block's type.</returns>
        public BlockInfo GetInfo (int x, int y, int z)
        {
            return BlockInfo.BlockTable[_blocks[x, y, z]];
        }

        internal BlockInfo GetInfo (int index)
        {
            return BlockInfo.BlockTable[_blocks[index]];
        }

        /// <summary>
        /// Gets the ID (type) of a block at the given local coordinates.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>The ID (type) of the specified block.</returns>
        public int GetID (int x, int y, int z)
        {
            return _blocks[x, y, z];
        }

        internal int GetID (int index)
        {
            return _blocks[index];
        }

        /// <summary>
        /// Sets the ID (type) of a block at the given local coordinates, maintaining consistency of data.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="id">The new ID of the block.</param>
        /// <remarks>Depending on the options set for this <see cref="AlphaBlockCollection"/>, this method can be very
        /// heavy-handed in the amount of work it does to maintain consistency of tile entities, lighting, fluid, etc.
        /// for the affected block and possibly many other indirectly-affected blocks in the collection or neighboring
        /// collections.  If many SetID calls are expected to be made, some of this auto-reconciliation behavior should
        /// be disabled, and the data should be rebuilt at the <see cref="AlphaBlockCollection"/>-level at the end.</remarks>
        public void SetID (int x, int y, int z, int id)
        {
            int oldid = _blocks[x, y, z];
            if (oldid == id) {
                return;
            }

            // Update value

            _blocks[x, y, z] = (byte)id;

            // Update tile entities

            BlockInfo info1 = BlockInfo.BlockTable[oldid];
            BlockInfo info2 = BlockInfo.BlockTable[id];

            BlockInfoEx einfo1 = info1 as BlockInfoEx;
            BlockInfoEx einfo2 = info2 as BlockInfoEx;

            if (einfo1 != einfo2) {
                if (einfo1 != null) {
                    ClearTileEntity(x, y, z);
                }

                if (einfo2 != null) {
                    CreateTileEntity(x, y, z);
                }
            }

            // Light consistency

            if (_autoLight) {
                if (info1.ObscuresLight != info2.ObscuresLight) {
                    _lightManager.UpdateHeightMap(x, y, z);
                }

                if (info1.Luminance != info2.Luminance || info1.Opacity != info2.Opacity || info1.TransmitsLight != info2.TransmitsLight) {
                    UpdateBlockLight(x, y, z);
                }

                if (info1.Opacity != info2.Opacity || info1.TransmitsLight != info2.TransmitsLight) {
                    UpdateSkyLight(x, y, z);
                }
            }

            // Fluid consistency

            if (_autoFluid) {
                if (info1.State == BlockState.FLUID || info2.State == BlockState.FLUID) {
                    UpdateFluid(x, y, z);
                }
            }

            _dirty = true;
        }

        internal void SetID (int index, int id)
        {
            int yzdim = _ydim * _zdim;

            int x = index / yzdim;
            int zy = index - (x * yzdim);

            int z = zy / _ydim;
            int y = zy - (z * _ydim);

            SetID(x, y, z, id);
        }

        /// <summary>
        /// Returns a count of all blocks in this <see cref="AlphaBlockCollection"/> with the given ID (type).
        /// </summary>
        /// <param name="id">The ID of blocks to count.</param>
        /// <returns>A count of all matching blocks.</returns>
        public int CountByID (int id)
        {
            int c = 0;
            for (int i = 0; i < _blocks.Length; i++) {
                if (_blocks[i] == id) {
                    c++;
                }
            }

            return c;
        }

        #endregion


        #region IDataBlockContainer Members

        /// <summary>
        /// Returns an object compatible with the <see cref="IDataBlock"/> interface from local coordinates relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>An <see cref="IDataBlock"/>-compatible object.</returns>
        /// <seealso cref="Block"/>
        IDataBlock IDataBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        /// <summary>
        /// Returns a reference object compatible with the <see cref="IDataBlock"/> interface from local coordinates relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>An <see cref="IDataBlock"/>-compatible reference object.</returns>
        /// <seealso cref="BlockRef"/>
        IDataBlock IDataBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        /// <summary>
        /// Updates a block in this collection with values from an <see cref="IDataBlock"/> object.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="block">An <see cref="IDataBlock"/> object to copy block data from.</param>
        public void SetBlock (int x, int y, int z, IDataBlock block)
        {
            SetID(x, y, z, block.ID);
            SetData(x, y, z, block.Data);
        }

        /// <summary>
        /// Gets the data value of a block at the given local coordinates.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>The data value of the specified block.</returns>
        public int GetData (int x, int y, int z)
        {
            return _data[x, y, z];
        }

        internal int GetData (int index)
        {
            return _data[index];
        }

        /// <summary>
        /// Sets the data value of a block at the given local coordinates.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="data">The new data value of the block.</param>
        public void SetData (int x, int y, int z, int data)
        {
            if (_data[x, y, z] != data) {
                _data[x, y, z] = (byte)data;
                _dirty = true;
            }

            /*if (BlockManager.EnforceDataLimits && BlockInfo.BlockTable[_blocks[index]] != null) {
                if (!BlockInfo.BlockTable[_blocks[index]].TestData(data)) {
                    return false;
                }
            }*/
        }

        internal void SetData (int index, int data)
        {
            if (_data[index] != data) {
                _data[index] = (byte)data;
                _dirty = true;
            }
        }

        /// <summary>
        /// Returns a count of all blocks in this <see cref="AlphaBlockCollection"/> matching the given ID (type) and data value.
        /// </summary>
        /// <param name="id">The ID of blocks to count.</param>
        /// <param name="data">The data value of blocks to count.</param>
        /// <returns>A count of all matching blocks.</returns>
        public int CountByData (int id, int data)
        {
            int c = 0;
            for (int i = 0; i < _blocks.Length; i++) {
                if (_blocks[i] == id && _data[i] == data) {
                    c++;
                }
            }

            return c;
        }

        #endregion


        #region ILitBlockCollection Members

        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>An <see cref="ILitBlock"/>-compatible object.</returns>
        /// <seealso cref="Block"/>
        ILitBlock ILitBlockCollection.GetBlock (int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a reference object compatible with the <see cref="ILitBlock"/> interface from local coordinates relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>An <see cref="ILitBlock"/>-compatible reference object.</returns>
        /// <seealso cref="BlockRef"/>
        ILitBlock ILitBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        /// <summary>
        /// Updates a block in this collection with values from an <see cref="ILitBlock"/> object.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="block">An <see cref="ILitBlock"/> object to copy block data from.</param>
        public void SetBlock (int x, int y, int z, ILitBlock block)
        {
            SetID(x, y, z, block.ID);
            SetBlockLight(x, y, z, block.BlockLight);
            SetSkyLight(x, y, z, block.SkyLight);
        }

        /// <summary>
        /// Gets the block-source light value of a block at the given local coordinates.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>The block-source light value of the specified block.</returns>
        public int GetBlockLight (int x, int y, int z)
        {
            return _blockLight[x, y, z];
        }

        internal int GetBlockLight (int index)
        {
            return _blockLight[index];
        }

        /// <summary>
        /// Gets the sky-source light value of a block at the given local coordinates.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>The sky-source light value of the specified block.</returns>
        public int GetSkyLight (int x, int y, int z)
        {
            return _skyLight[x, y, z];
        }

        internal int GetSkyLight (int index)
        {
            return _skyLight[index];
        }

        /// <summary>
        /// Sets the blocks-source light value of a block at the given local coordinates.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="light">The new block-source light value of the block.</param>
        public void SetBlockLight (int x, int y, int z, int light)
        {
            if (_blockLight[x, y, z] != light) {
                _blockLight[x, y, z] = (byte)light;
                _dirty = true;
            }
        }

        internal void SetBlockLight (int index, int light)
        {
            if (_blockLight[index] != light) {
                _blockLight[index] = (byte)light;
                _dirty = true;
            }
        }

        /// <summary>
        /// Sets the sky-source light value of a block at the given local coordinates.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="light">The new sky-source light value of the block.</param>
        public void SetSkyLight (int x, int y, int z, int light)
        {
            if (_skyLight[x, y, z] != light) {
                _skyLight[x, y, z] = (byte)light;
                _dirty = true;
            }
        }

        internal void SetSkyLight (int index, int light)
        {
            if (_skyLight[index] != light) {
                _skyLight[index] = (byte)light;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets the lowest Y-coordinate of a block at which sky-source light remains unfiltered.
        /// </summary>
        /// <param name="x">Local X-coordinate of a map location.</param>
        /// <param name="z">Local Z-coordinate of a map location.</param>
        /// <returns>The height-map value of a map location for calculating sky-source lighting.</returns>
        public int GetHeight (int x, int z)
        {
            return _heightMap[x, z];
        }

        /// <summary>
        /// Sets the lowest Y-coordinate of a block at which sky-source light remains unfiltered.
        /// </summary>
        /// <param name="x">Local X-coordinate of a map location.</param>
        /// <param name="z">Local Z-coordinate of a map location.</param>
        /// <param name="height">The new height-map value of the given map location.</param>
        public void SetHeight (int x, int z, int height)
        {
            _heightMap[x, z] = (byte)height;
        }

        /// <summary>
        /// Updates the block-source lighting of a block at the given local coordinates to maintain light consistency.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <remarks><para>The lighting of the block will be updated to be consistent with the lighting in neighboring blocks.
        /// If the block is itself a light source, many nearby blocks may be updated to maintain consistent lighting.  These
        /// updates may also touch neighboring <see cref="AlphaBlockCollection"/> objects, if they can be resolved.</para>
        /// <para>This function assumes that the entire <see cref="AlphaBlockCollection"/> and neighboring <see cref="AlphaBlockCollection"/>s
        /// already have consistent lighting, with the exception of the block being updated.  If this assumption is violated,
        /// lighting may fail to converge correctly.</para></remarks>
        public void UpdateBlockLight (int x, int y, int z)
        {
            _lightManager.UpdateBlockLight(x, y, z);
            _dirty = true;
        }

        /// <summary>
        /// Updates the sky-source lighting of a block at the given local coordinates to maintain light consistency.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <remarks><para>The lighting of the block will be updated to be consistent with the lighting in neighboring blocks.
        /// If the block is itself a light source, many nearby blocks may be updated to maintain consistent lighting.  These
        /// updates may also touch neighboring <see cref="AlphaBlockCollection"/> objects, if they can be resolved.</para>
        /// <para>This function assumes that the entire <see cref="AlphaBlockCollection"/> and neighboring <see cref="AlphaBlockCollection"/>s
        /// already have consistent lighting, with the exception of the block being updated.  If this assumption is violated,
        /// lighting may fail to converge correctly.</para></remarks>
        public void UpdateSkyLight (int x, int y, int z)
        {
            _lightManager.UpdateBlockSkyLight(x, y, z);
            _dirty = true;
        }

        /// <summary>
        /// Resets the block-source light value to 0 for all blocks in this <see cref="AlphaBlockCollection"/>.
        /// </summary>
        public void ResetBlockLight ()
        {
            _blockLight.Clear();
            _dirty = true;
        }

        /// <summary>
        /// Resets the sky-source light value to 0 for all blocks in this <see cref="AlphaBlockCollection"/>.
        /// </summary>
        public void ResetSkyLight ()
        {
            _skyLight.Clear();
            _dirty = true;
        }

        /// <summary>
        /// Reconstructs the block-source lighting for all blocks in this <see cref="AlphaBlockCollection"/>.
        /// </summary>
        /// <remarks><para>This function should only be called after the lighting has been reset in this <see cref="AlphaBlockCollection"/>
        /// and all neighboring <see cref="AlphaBlockCollection"/>s, or lighting may fail to converge correctly.  
        /// This function cannot reset the lighting on its own, due to interactions between <see cref="AlphaBlockCollection"/>s.</para>
        /// <para>If many light source or block opacity values will be modified in this <see cref="AlphaBlockCollection"/>, it may
        /// be preferable to avoid explicit or implicit calls to <see cref="UpdateBlockLight"/> and call this function once when
        /// modifications are complete.</para></remarks>
        /// /<seealso cref="ResetBlockLight"/>
        public void RebuildBlockLight ()
        {
            _lightManager.RebuildBlockLight();
            _dirty = true;
        }

        /// <summary>
        /// Reconstructs the sky-source lighting for all blocks in this <see cref="AlphaBlockCollection"/>.
        /// </summary>
        /// <remarks><para>This function should only be called after the lighting has been reset in this <see cref="AlphaBlockCollection"/>
        /// and all neighboring <see cref="AlphaBlockCollection"/>s, or lighting may fail to converge correctly.  
        /// This function cannot reset the lighting on its own, due to interactions between <see cref="AlphaBlockCollection"/>s.</para>
        /// <para>If many light source or block opacity values will be modified in this <see cref="AlphaBlockCollection"/>, it may
        /// be preferable to avoid explicit or implicit calls to <see cref="UpdateSkyLight"/> and call this function once when
        /// modifications are complete.</para></remarks>
        /// <seealso cref="ResetSkyLight"/>
        public void RebuildSkyLight ()
        {
            _lightManager.RebuildBlockSkyLight();
            _dirty = true;
        }

        /// <summary>
        /// Reconstructs the height-map for this <see cref="AlphaBlockCollection"/>.
        /// </summary>
        public void RebuildHeightMap ()
        {
            _lightManager.RebuildHeightMap();
            _dirty = true;
        }

        /// <summary>
        /// Reconciles any block-source lighting inconsistencies between this <see cref="AlphaBlockCollection"/> and any of its neighbors.
        /// </summary>
        /// <remarks>It will be necessary to call this function if an <see cref="AlphaBlockCollection"/> is reset and rebuilt, but
        /// some of its neighbors are not.  A rebuilt <see cref="AlphaBlockCollection"/> will spill lighting updates into its neighbors,
        /// but will not see lighting that should be propagated back from its neighbors.</remarks>
        /// <seealso cref="RebuildBlockLight"/>
        public void StitchBlockLight ()
        {
            _lightManager.StitchBlockLight();
            _dirty = true;
        }

        /// <summary>
        /// Reconciles any sky-source lighting inconsistencies between this <see cref="AlphaBlockCollection"/> and any of its neighbors.
        /// </summary>
        /// <remarks>It will be necessary to call this function if an <see cref="AlphaBlockCollection"/> is reset and rebuilt, but
        /// some of its neighbors are not.  A rebuilt <see cref="AlphaBlockCollection"/> will spill lighting updates into its neighbors,
        /// but will not see lighting that should be propagated back from its neighbors.</remarks>
        /// <seealso cref="RebuildSkyLight"/>
        public void StitchSkyLight ()
        {
            _lightManager.StitchBlockSkyLight();
            _dirty = true;
        }

        /// <summary>
        /// Reconciles any block-source lighting inconsistencies between this <see cref="AlphaBlockCollection"/> and another <see cref="IBoundedLitBlockCollection"/> on a given edge.
        /// </summary>
        /// <param name="blockset">An <see cref="IBoundedLitBlockCollection"/>-compatible object with the same dimensions as this <see cref="AlphaBlockCollection"/>.</param>
        /// <param name="edge">The edge that <paramref name="blockset"/> is a neighbor on.</param>
        /// <remarks>It will be necessary to call this function if an <see cref="AlphaBlockCollection"/> is reset and rebuilt, but
        /// some of its neighbors are not.  A rebuilt <see cref="AlphaBlockCollection"/> will spill lighting updates into its neighbors,
        /// but will not see lighting that should be propagated back from its neighbors.</remarks>
        /// <seealso cref="RebuildBlockLight"/>
        public void StitchBlockLight (IBoundedLitBlockCollection blockset, BlockCollectionEdge edge)
        {
            _lightManager.StitchBlockLight(blockset, edge);
            _dirty = true;
        }

        /// <summary>
        /// Reconciles any sky-source lighting inconsistencies between this <see cref="AlphaBlockCollection"/> and another <see cref="IBoundedLitBlockCollection"/> on a given edge.
        /// </summary>
        /// <param name="blockset">An <see cref="IBoundedLitBlockCollection"/>-compatible object with the same dimensions as this <see cref="AlphaBlockCollection"/>.</param>
        /// <param name="edge">The edge that <paramref name="blockset"/> is a neighbor on.</param>
        /// <remarks>It will be necessary to call this function if an <see cref="AlphaBlockCollection"/> is reset and rebuilt, but
        /// some of its neighbors are not.  A rebuilt <see cref="AlphaBlockCollection"/> will spill lighting updates into its neighbors,
        /// but will not see lighting that should be propagated back from its neighbors.</remarks>
        /// <seealso cref="RebuildSkyLight"/>
        public void StitchSkyLight (IBoundedLitBlockCollection blockset, BlockCollectionEdge edge)
        {
            _lightManager.StitchBlockSkyLight(blockset, edge);
            _dirty = true;
        }

        #endregion


        #region IPropertyBlockCollection Members

        /// <summary>
        /// Returns an object compatible with the <see cref="IPropertyBlock"/> interface from local coordinates relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>An <see cref="IPropertyBlock"/>-compatible object.</returns>
        /// <seealso cref="Block"/>
        IPropertyBlock IPropertyBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        /// <summary>
        /// Returns a reference object compatible with the <see cref="IPropertyBlock"/> interface from local coordinates relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>An <see cref="IPropertyBlock"/>-compatible reference object.</returns>
        /// <seealso cref="BlockRef"/>
        IPropertyBlock IPropertyBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        /// <summary>
        /// Updates a block in this collection with values from an <see cref="IPropertyBlock"/> object.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="block">An <see cref="IPropertyBlock"/> object to copy block data from.</param>
        public void SetBlock (int x, int y, int z, IPropertyBlock block)
        {
            SetID(x, y, z, block.ID);
            SetTileEntity(x, y, z, block.GetTileEntity().Copy());
        }

        /// <summary>
        /// Gets a <see cref="TileEntity"/> record for a block at the given local coordinates, if one exists.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <returns>A <see cref="TileEntity"/> for the given block, or null if the <see cref="TileEntity"/> is missing or the block type does not use a <see cref="TileEntity"/>.</returns>
        public TileEntity GetTileEntity (int x, int y, int z)
        {
            return _tileEntityManager.GetTileEntity(x, y, z);
        }

        /// <summary>
        /// Sets a <see cref="TileEntity"/> record for a block at the given local coordinates.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="te">The <see cref="TileEntity"/> to add to the given block.</param>
        /// <exception cref="ArgumentException">Thrown when the <see cref="TileEntity"/> being passed is of the wrong type for the given block.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the given block is of a type that does not support a <see cref="TileEntity"/> record.</exception>
        public void SetTileEntity (int x, int y, int z, TileEntity te)
        {
            _tileEntityManager.SetTileEntity(x, y, z, te);
            _dirty = true;
        }

        /// <summary>
        /// Creates a default <see cref="TileEntity"/> record suitable for the block at the given local coordinates.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <exception cref="InvalidOperationException">Thrown when the given block is of a type that does not support a <see cref="TileEntity"/> record.</exception>
        /// <exception cref="UnknownTileEntityException">Thrown when the block type requests a <see cref="TileEntity"/> that has not been registered with the <see cref="TileEntityFactory"/>.</exception>
        public void CreateTileEntity (int x, int y, int z)
        {
            _tileEntityManager.CreateTileEntity(x, y, z);
            _dirty = true;
        }

        /// <summary>
        /// Clears any <see cref="TileEntity"/> record set for a block at the givne local coordinates, if one exists.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        public void ClearTileEntity (int x, int y, int z)
        {
            _tileEntityManager.ClearTileEntity(x, y, z);
            _dirty = true;
        }

        #endregion

        public void ResetFluid ()
        {
            _fluidManager.ResetWater(_blocks, _data);
            _fluidManager.ResetLava(_blocks, _data);
            _dirty = true;
        }

        public void RebuildFluid ()
        {
            _fluidManager.RebuildWater();
            _fluidManager.RebuildLava();
            _dirty = true;
        }

        public void UpdateFluid (int x, int y, int z)
        {
            bool autofluid = _autoFluid;
            _autoFluid = false;

            int blocktype = _blocks[x, y, z];

            if (blocktype == BlockType.WATER || blocktype == BlockType.STATIONARY_WATER) {
                _fluidManager.UpdateWater(x, y, z);
                _dirty = true;
            }
            else if (blocktype == BlockType.LAVA || blocktype == BlockType.STATIONARY_LAVA) {
                _fluidManager.UpdateLava(x, y, z);
                _dirty = true;
            }

            _autoFluid = autofluid;
        }


        /*#region IEnumerable<AlphaBlockRef> Members

        public IEnumerator<AlphaBlockRef> GetEnumerator ()
        {
            return new AlphaBlockEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return new AlphaBlockEnumerator(this);
        }

        #endregion

        public class AlphaBlockEnumerator : IEnumerator<AlphaBlockRef>
        {
            private AlphaBlockCollection _collection;
            private int _index;
            private int _size;

            public AlphaBlockEnumerator (AlphaBlockCollection collection)
            {
                _collection = collection;
                _index = -1;
                _size = collection.XDim * collection.YDim * collection.ZDim;
            }

            #region IEnumerator<Entity> Members

            public AlphaBlockRef Current
            {
                get
                {
                    if (_index == -1 || _index == _size) {
                        throw new InvalidOperationException();
                    }
                    return new AlphaBlockRef(_collection, _index);
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose () { }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext ()
            {
                if (++_index == _size) {
                    return false;
                }

                return true;
            }

            public void Reset ()
            {
                _index = -1;
            }

            #endregion
        }*/
    }
}
