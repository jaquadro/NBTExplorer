using System;
using System.Collections.Generic;

using Substrate.Utility;
using Substrate.NBT;

namespace Substrate
{
    public struct BlockData {
        public XZYByteArray blocks;

        public BlockData (XZYByteArray b)
        {
            blocks = b;
        }
    }

    public struct BlockDataData {
        public XZYNibbleArray data;

        public BlockDataData (XZYNibbleArray d)
        {
            data = d;
        }
    }

    public struct BlockLightData {
        public XZYNibbleArray light;
        public XZYNibbleArray skyLight;
        public ZXByteArray heightMap;

        public BlockLightData (XZYNibbleArray l, XZYNibbleArray s, ZXByteArray h)
        {
            light = l;
            skyLight = s;
            heightMap = h;
        }
    }

    public struct BlockPropertyData {
        public TagList tileEntities;

        public BlockPropertyData (TagList t)
        {
            tileEntities = t;
        }
    }

    public class AlphaBlockCollection : IBoundedAlphaBlockCollection
    {
        private XZYByteArray _blocks;
        private XZYNibbleArray _data;
        private XZYNibbleArray _blockLight;
        private XZYNibbleArray _skyLight;
        private ZXByteArray _heightMap;

        private TagList _tileEntities;

        private BlockLight _lightManager;
        private BlockTileEntities _tileEntityManager;

        private bool _dirty = false;
        private bool _autoLight = true;

        public delegate AlphaBlockCollection NeighborLookupHandler (int relx, int rely, int relz);

        public event NeighborLookupHandler ResolveNeighbor
        {
            add 
            {
                _lightManager.ResolveNeighbor += delegate(int relx, int rely, int relz) { 
                    return value(relx, rely, relz); 
                };
            }

            remove
            {
                _lightManager = new BlockLight(this);
            }
        }

        public event BlockCoordinateHandler TranslateCoordinates
        {
            add { _tileEntityManager.TranslateCoordinates += value; }
            remove { _tileEntityManager.TranslateCoordinates -= value; }
        }

        public bool AutoLight
        {
            get { return _autoLight; }
            set { _autoLight = value; }
        }

        public bool IsDirty
        {
            get { return _dirty; }
            set { _dirty = value; }
        }

        public AlphaBlockCollection (BlockData root, BlockDataData data, BlockLightData light, BlockPropertyData properties)
        {
            _blocks = root.blocks;
            _data = data.data;
            _blockLight = light.light;
            _skyLight = light.skyLight;
            _heightMap = light.heightMap;
            _tileEntities = properties.tileEntities;

            _lightManager = new BlockLight(this);
            _tileEntityManager = new BlockTileEntities(_blocks, _tileEntities);
        }

        public Block GetBlock (int x, int y, int z)
        {
            return new Block(this, x, y, z);
        }

        public BlockRef GetBlockRef (int x, int y, int z)
        {
            return new BlockRef(this, x, y, z);
        }

        public void SetBlock (int x, int y, int z, Block block)
        {
            SetID(x, y, z, block.ID);
            SetData(x, y, z, block.Data);

            SetTileEntity(x, y, z, block.GetTileEntity().Copy());
        }

        #region IBlockCollection Members

        public int XDim
        {
            get { return _blocks.XDim; }
        }

        public int YDim
        {
            get { return _blocks.YDim; }
        }

        public int ZDim
        {
            get { return _blocks.ZDim; }
        }

        IBlock IBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IBlock IBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, IBlock block)
        {
            SetID(x, y, z, block.ID);
        }

        public BlockInfo GetInfo (int x, int y, int z)
        {
            return BlockInfo.BlockTable[_blocks[x, y, z]];
        }

        public int GetID (int x, int y, int z)
        {
            return _blocks[x, y, z];
        }

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

            // Update height map

            if (BlockInfo.BlockTable[id] != null) {
                int tileHeight = _heightMap[x, z];
                int newOpacity = BlockInfo.BlockTable[id].Opacity;

                if (y > tileHeight && newOpacity > BlockInfo.MIN_OPACITY) {
                    _heightMap[x, z] = (byte)y;
                }
                else if (y == tileHeight && newOpacity == BlockInfo.MIN_OPACITY) {
                    for (int i = y - 1; i >= 0; i--) {
                        if (BlockInfo.BlockTable[_blocks[x, i, z]].Opacity > BlockInfo.MIN_OPACITY) {
                            _heightMap[x, z] = (byte)i;
                            break;
                        }
                    }
                }
            }

            // Light consistency

            if (_autoLight) {
                if (info1.Luminance != info2.Luminance || info1.Opacity != info2.Opacity) {
                    UpdateBlockLight(x, y, z);
                }

                if (info1.Opacity != info2.Opacity) {
                    UpdateSkyLight(x, y, z);
                }
            }

            _dirty = true;
        }

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

        IDataBlock IDataBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IDataBlock IDataBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, IDataBlock block)
        {
            SetID(x, y, z, block.ID);
            SetData(x, y, z, block.Data);
        }

        public int GetData (int x, int y, int z)
        {
            return _data[x, y, z];
        }

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

        ILitBlock ILitBlockCollection.GetBlock (int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        ILitBlock ILitBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, ILitBlock block)
        {
            SetID(x, y, z, block.ID);
            SetBlockLight(x, y, z, block.BlockLight);
            SetSkyLight(x, y, z, block.SkyLight);
        }

        public int GetBlockLight (int x, int y, int z)
        {
            return _blockLight[x, y, z];
        }

        public int GetSkyLight (int x, int y, int z)
        {
            return _skyLight[x, y, z];
        }

        public void SetBlockLight (int x, int y, int z, int light)
        {
            if (_blockLight[x, y, z] != light) {
                _blockLight[x, y, z] = (byte)light;
                _dirty = true;
            }
        }

        public void SetSkyLight (int x, int y, int z, int light)
        {
            if (_skyLight[x, y, z] != light) {
                _skyLight[x, y, z] = (byte)light;
                _dirty = true;
            }
        }

        public int GetHeight (int x, int z)
        {
            return _heightMap[x, z];
        }

        public void SetHeight (int x, int z, int height)
        {
            _heightMap[x, z] = (byte)height;
        }

        public void UpdateBlockLight (int x, int y, int z)
        {
            _lightManager.UpdateBlockLight(x, y, z);
            _dirty = true;
        }

        public void UpdateSkyLight (int x, int y, int z)
        {
            _lightManager.UpdateBlockSkyLight(x, y, z);
            _dirty = true;
        }

        public void ResetBlockLight ()
        {
            _blockLight.Clear();
            _dirty = true;
        }

        public void ResetSkyLight ()
        {
            _skyLight.Clear();
            _dirty = true;
        }

        public void RebuildBlockLight ()
        {
            _lightManager.RebuildBlockLight();
            _dirty = true;
        }

        public void RebuildSkyLight ()
        {
            _lightManager.RebuildBlockSkyLight();
            _dirty = true;
        }

        public void RebuildHeightMap ()
        {
            _lightManager.RebuildHeightMap();
            _dirty = true;
        }

        public void StitchBlockLight ()
        {
            _lightManager.StitchBlockLight();
            _dirty = true;
        }

        public void StitchSkyLight ()
        {
            _lightManager.StitchBlockSkyLight();
            _dirty = true;
        }

        public void StitchBlockLight (IBoundedLitBlockCollection blockset, BlockCollectionEdge edge)
        {
            _lightManager.StitchBlockLight(blockset, edge);
            _dirty = true;
        }

        public void StitchSkyLight (IBoundedLitBlockCollection blockset, BlockCollectionEdge edge)
        {
            _lightManager.StitchBlockSkyLight(blockset, edge);
            _dirty = true;
        }

        #endregion


        #region IPropertyBlockCollection Members

        IPropertyBlock IPropertyBlockCollection.GetBlock (int x, int y, int z)
        {
            return GetBlock(x, y, z);
        }

        IPropertyBlock IPropertyBlockCollection.GetBlockRef (int x, int y, int z)
        {
            return GetBlockRef(x, y, z);
        }

        public void SetBlock (int x, int y, int z, IPropertyBlock block)
        {
            SetID(x, y, z, block.ID);
            SetTileEntity(x, y, z, block.GetTileEntity().Copy());
        }

        public TileEntity GetTileEntity (int x, int y, int z)
        {
            return _tileEntityManager.GetTileEntity(x, y, z);
        }

        public void SetTileEntity (int x, int y, int z, TileEntity te)
        {
            _tileEntityManager.SetTileEntity(x, y, z, te);
            _dirty = true;
        }

        public void CreateTileEntity (int x, int y, int z)
        {
            _tileEntityManager.CreateTileEntity(x, y, z);
            _dirty = true;
        }

        public void ClearTileEntity (int x, int y, int z)
        {
            _tileEntityManager.ClearTileEntity(x, y, z);
            _dirty = true;
        }

        #endregion


    }
}
