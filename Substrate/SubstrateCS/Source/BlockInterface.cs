using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    public enum BlockCollectionEdge
    {
        EAST = 0,
        NORTH = 1,
        WEST = 2,
        SOUTH = 3
    }

    public interface IBlock
    {
        BlockInfo Info { get; }
        int ID { get; set; }
    }

    public interface IDataBlock : IBlock
    {
        int Data { get; set; }
    }

    public interface ILitBlock : IBlock
    {
        int BlockLight { get; set; }
        int SkyLight { get; set; }
    }

    public interface IPropertyBlock : IBlock
    {
        TileEntity GetTileEntity ();
        void SetTileEntity (TileEntity te);
        void ClearTileEntity ();
    }

    public interface IAlphaBlock : IDataBlock, IPropertyBlock
    {
    }

    public interface IAlphaBlockRef : IDataBlock, ILitBlock, IPropertyBlock
    {
    }

    public interface IBlockCollection
    {
        IBlock GetBlock (int x, int y, int z);
        IBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, IBlock block);

        int GetID (int x, int y, int z);
        void SetID (int x, int y, int z, int id);

        BlockInfo GetInfo (int x, int y, int z);
    }

    public interface IBoundedBlockCollection : IBlockCollection
    {
        int XDim { get; }
        int YDim { get; }
        int ZDim { get; }

        int CountByID (int id);
    }

    public interface IDataBlockCollection : IBlockCollection
    {
        new IDataBlock GetBlock (int x, int y, int z);
        new IDataBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, IDataBlock block);

        int GetData (int x, int y, int z);
        void SetData (int x, int y, int z, int data);
    }

    public interface IBoundedDataBlockCollection : IDataBlockCollection, IBoundedBlockCollection
    {
        int CountByData (int id, int data);
    }

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

    public interface IBoundedPropertyBlockCollection : IPropertyBlockCollection, IBoundedBlockCollection
    {
    }

    public interface IAlphaBlockCollection : IDataBlockCollection, ILitBlockCollection, IPropertyBlockCollection
    {
        new Block GetBlock (int x, int y, int z);
        new BlockRef GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, Block block);
    }

    public interface IBoundedAlphaBlockCollection : IAlphaBlockCollection, IBoundedDataBlockCollection, IBoundedLitBlockCollection, IBoundedPropertyBlockCollection
    {
    }

    public interface IBlockManager : IAlphaBlockCollection
    { 
    }
}
