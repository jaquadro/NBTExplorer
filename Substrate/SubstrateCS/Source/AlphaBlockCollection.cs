using System;
using System.Collections.Generic;
using Substrate.Core;
using Substrate.Nbt;

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
        /// Returns a new <see cref="AlphaBlock"/> object from local coordinates relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of block.</param>
        /// <param name="y">Local Y-coordinate of block.</param>
        /// <param name="z">Local Z-coordiante of block.</param>
        /// <returns>A new <see cref="AlphaBlock"/> object representing context-independent data of a single block.</returns>
        /// <remarks>Context-independent data excludes data such as lighting.  <see cref="AlphaBlock"/> object actually contain a copy
        /// of the data they represent, so changes to the <see cref="AlphaBlock"/> will not affect this container, and vice-versa.</remarks>
        public AlphaBlock GetBlock (int x, int y, int z)
        {
            return new AlphaBlock(this, x, y, z);
        }

        /// <summary>
        /// Returns a new <see cref="AlphaBlockRef"/> object from local coordaintes relative to this collection.
        /// </summary>
        /// <param name="x">Local X-coordinate of block.</param>
        /// <param name="y">Local Y-coordinate of block.</param>
        /// <param name="z">Local Z-coordinate of block.</param>
        /// <returns>A new <see cref="AlphaBlockRef"/> object representing context-dependent data of a single block.</returns>
        /// <remarks>Context-depdendent data includes all data associated with this block.  Since a <see cref="AlphaBlockRef"/> represents
        /// a view of a block within this container, any updates to data in the container will be reflected in the <see cref="AlphaBlockRef"/>,
        /// and vice-versa for updates to the <see cref="AlphaBlockRef"/>.</remarks>
        public AlphaBlockRef GetBlockRef (int x, int y, int z)
        {
            return new AlphaBlockRef(this, _blocks.GetIndex(x, y, z));
        }

        /// <summary>
        /// Updates a block in this collection with values from a <see cref="AlphaBlock"/> object.
        /// </summary>
        /// <param name="x">Local X-coordinate of a block.</param>
        /// <param name="y">Local Y-coordinate of a block.</param>
        /// <param name="z">Local Z-coordinate of a block.</param>
        /// <param name="block">A <see cref="AlphaBlock"/> object to copy block data from.</param>
        public void SetBlock (int x, int y, int z, AlphaBlock block)
        {
            SetID(x, y, z, block.ID);
            SetData(x, y, z, block.Data);

            SetTileEntity(x, y, z, block.GetTileEntity().Copy());
        }

        #region IBoundedBlockCollection Members

        /// <inheritdoc/>
        public int XDim
        {
            get { return _xdim; }
        }

        /// <inheritdoc/>
        public int YDim
        {
            get { return _ydim; }
        }

        /// <inheritdoc/>
        public int ZDim
        {
            get { return _zdim; }
        }

        IBlock IBoundedBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IBlock IBoundedBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        /// <inheritdoc/>
        public void SetBlock (int x, int y, int z, IBlock block)
        {
            SetID(x, y, z, block.ID);
        }

        /// <inheritdoc/>
        public BlockInfo GetInfo (int x, int y, int z)
        {
            return BlockInfo.BlockTable[_blocks[x, y, z]];
        }

        internal BlockInfo GetInfo (int index)
        {
            return BlockInfo.BlockTable[_blocks[index]];
        }

        /// <inheritdoc/>
        public int GetID (int x, int y, int z)
        {
            return _blocks[x, y, z];
        }

        internal int GetID (int index)
        {
            return _blocks[index];
        }

        /// <inheritdoc/>
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
            int x, y, z;
            _blocks.GetMultiIndex(index, out x, out y, out z);

            SetID(x, y, z, id);
        }

        /// <inheritdoc/>
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


        #region IBoundedDataBlockContainer Members

        IDataBlock IBoundedDataBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IDataBlock IBoundedDataBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        /// <inheritdoc/>
        public void SetBlock (int x, int y, int z, IDataBlock block)
        {
            SetID(x, y, z, block.ID);
            SetData(x, y, z, block.Data);
        }

        /// <inheritdoc/>
        public int GetData (int x, int y, int z)
        {
            return _data[x, y, z];
        }

        internal int GetData (int index)
        {
            return _data[index];
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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


        #region IBoundedLitBlockCollection Members

        ILitBlock IBoundedLitBlockCollection.GetBlock (int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        ILitBlock IBoundedLitBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        /// <inheritdoc/>
        public void SetBlock (int x, int y, int z, ILitBlock block)
        {
            SetID(x, y, z, block.ID);
            SetBlockLight(x, y, z, block.BlockLight);
            SetSkyLight(x, y, z, block.SkyLight);
        }

        /// <inheritdoc/>
        public int GetBlockLight (int x, int y, int z)
        {
            return _blockLight[x, y, z];
        }

        internal int GetBlockLight (int index)
        {
            return _blockLight[index];
        }

        /// <inheritdoc/>
        public int GetSkyLight (int x, int y, int z)
        {
            return _skyLight[x, y, z];
        }

        internal int GetSkyLight (int index)
        {
            return _skyLight[index];
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public int GetHeight (int x, int z)
        {
            return _heightMap[x, z];
        }

        /// <inheritdoc/>
        public void SetHeight (int x, int z, int height)
        {
            _heightMap[x, z] = (byte)height;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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


        #region IBoundedPropertyBlockCollection Members

        IPropertyBlock IBoundedPropertyBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IPropertyBlock IBoundedPropertyBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        /// <inheritdoc/>
        public void SetBlock (int x, int y, int z, IPropertyBlock block)
        {
            SetID(x, y, z, block.ID);
            SetTileEntity(x, y, z, block.GetTileEntity().Copy());
        }

        /// <inheritdoc/>
        public TileEntity GetTileEntity (int x, int y, int z)
        {
            return _tileEntityManager.GetTileEntity(x, y, z);
        }

        internal TileEntity GetTileEntity (int index)
        {
            int x, y, z;
            _blocks.GetMultiIndex(index, out x, out y, out z);

            return _tileEntityManager.GetTileEntity(x, y, z);
        }

        /// <inheritdoc/>
        public void SetTileEntity (int x, int y, int z, TileEntity te)
        {
            _tileEntityManager.SetTileEntity(x, y, z, te);
            _dirty = true;
        }

        internal void SetTileEntity (int index, TileEntity te)
        {
            int x, y, z;
            _blocks.GetMultiIndex(index, out x, out y, out z);

            _tileEntityManager.SetTileEntity(x, y, z, te);
            _dirty = true;
        }

        /// <inheritdoc/>
        public void CreateTileEntity (int x, int y, int z)
        {
            _tileEntityManager.CreateTileEntity(x, y, z);
            _dirty = true;
        }

        internal void CreateTileEntity (int index)
        {
            int x, y, z;
            _blocks.GetMultiIndex(index, out x, out y, out z);

            _tileEntityManager.CreateTileEntity(x, y, z);
            _dirty = true;
        }

        /// <inheritdoc/>
        public void ClearTileEntity (int x, int y, int z)
        {
            _tileEntityManager.ClearTileEntity(x, y, z);
            _dirty = true;
        }

        internal void ClearTileEntity (int index)
        {
            int x, y, z;
            _blocks.GetMultiIndex(index, out x, out y, out z);

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


        #region Unbounded Container Implementations

        IBlock IBlockCollection.GetBlock (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetBlock(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        IBlock IBlockCollection.GetBlockRef (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetBlockRef(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void IBlockCollection.SetBlock (int x, int y, int z, IBlock block)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                SetBlock(x, y, z, block);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        BlockInfo IBlockCollection.GetInfo (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetInfo(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        int IBlockCollection.GetID (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetID(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void IBlockCollection.SetID (int x, int y, int z, int id)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                SetID(x, y, z, id);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        IDataBlock IDataBlockCollection.GetBlock (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetBlock(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        IDataBlock IDataBlockCollection.GetBlockRef (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetBlockRef(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void IDataBlockCollection.SetBlock (int x, int y, int z, IDataBlock block)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                SetBlock(x, y, z, block);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        int IDataBlockCollection.GetData (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetData(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void IDataBlockCollection.SetData (int x, int y, int z, int data)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                SetData(x, y, z, data);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        ILitBlock ILitBlockCollection.GetBlock (int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        ILitBlock ILitBlockCollection.GetBlockRef (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetBlockRef(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void ILitBlockCollection.SetBlock (int x, int y, int z, ILitBlock block)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                SetBlock(x, y, z, block);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        int ILitBlockCollection.GetBlockLight (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetBlockLight(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void ILitBlockCollection.SetBlockLight (int x, int y, int z, int light)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                SetBlockLight(x, y, z, light);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        int ILitBlockCollection.GetSkyLight (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetSkyLight(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void ILitBlockCollection.SetSkyLight (int x, int y, int z, int light)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                SetSkyLight(x, y, z, light);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        int ILitBlockCollection.GetHeight (int x, int z)
        {
            if (x >= 0 && x < _xdim && z >= 0 && z < ZDim) {
                return GetHeight(x, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : "z");
        }

        void ILitBlockCollection.SetHeight (int x, int z, int height)
        {
            if (x >= 0 && x < _xdim && z >= 0 && z < ZDim) {
                SetHeight(x, z, height);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : "z");
        }

        void ILitBlockCollection.UpdateBlockLight (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                UpdateBlockLight(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void ILitBlockCollection.UpdateSkyLight (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                UpdateSkyLight(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        IPropertyBlock IPropertyBlockCollection.GetBlock (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetBlock(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        IPropertyBlock IPropertyBlockCollection.GetBlockRef (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetBlockRef(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void IPropertyBlockCollection.SetBlock (int x, int y, int z, IPropertyBlock block)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                SetBlock(x, y, z, block);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        TileEntity IPropertyBlockCollection.GetTileEntity (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetTileEntity(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void IPropertyBlockCollection.SetTileEntity (int x, int y, int z, TileEntity te)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                SetTileEntity(x, y, z, te);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void IPropertyBlockCollection.CreateTileEntity (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                CreateTileEntity(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void IPropertyBlockCollection.ClearTileEntity (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                ClearTileEntity(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        AlphaBlock IAlphaBlockCollection.GetBlock (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetBlock(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        AlphaBlockRef IAlphaBlockCollection.GetBlockRef (int x, int y, int z)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                return GetBlockRef(x, y, z);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        void IAlphaBlockCollection.SetBlock (int x, int y, int z, AlphaBlock block)
        {
            if (x >= 0 && x < _xdim && y >= 0 && y < _ydim && z >= 0 && z < ZDim) {
                SetBlock(x, y, z, block);
            }
            throw new ArgumentOutOfRangeException(x < 0 || x >= _xdim ? "x" : y < 0 || y >= _ydim ? "y" : "z");
        }

        #endregion

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
