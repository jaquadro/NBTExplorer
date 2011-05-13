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

    public interface IBlockCollection
    {
        IBlock GetBlock (int x, int y, int z);
        IBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, IBlock block);

        int GetBlockID (int x, int y, int z);
        void SetBlockID (int x, int y, int z, int id);

        BlockInfo GetBlockInfo (int x, int y, int z);
    }

    public interface IBoundedBlockCollection : IBlockCollection
    {
        int XDim { get; }
        int YDim { get; }
        int ZDim { get; }

        int CountBlockID (int id);
    }

    public interface IDataBlockCollection : IBlockCollection
    {
        new IDataBlock GetBlock (int x, int y, int z);
        new IDataBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, IDataBlock block);

        int GetBlockData (int x, int y, int z);
        void SetBlockData (int x, int y, int z, int data);
    }

    public interface IBoundedDataBlockCollection : IDataBlockCollection, IBoundedBlockCollection
    {
        int CountBlockData (int id, int data);
    }

    public interface ILitBlockCollection : IBlockCollection
    {
        new ILitBlock GetBlock (int x, int y, int z);
        new ILitBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, ILitBlock block);

        // Local Light
        int GetBlockLight (int x, int y, int z);
        int GetBlockSkyLight (int x, int y, int z);

        void SetBlockLight (int x, int y, int z, int light);
        void SetBlockSkyLight (int x, int y, int z, int light);

        int GetHeight (int x, int z);
        void SetHeight (int x, int z, int height);

        // Update and propagate light at a single block
        void UpdateBlockLight (int x, int y, int z);
        void UpdateBlockSkyLight (int x, int y, int z);
    }

    public interface IBoundedLitBlockCollection : ILitBlockCollection, IBoundedBlockCollection
    {
        // Zero out light in entire collection
        void ResetBlockLight ();
        void ResetBlockSkyLight ();

        // Recalculate light in entire collection
        void RebuildBlockLight ();
        void RebuildBlockSkyLight ();
        void RebuildHeightMap ();

        // Reconcile inconsistent lighting between the edges of two containers of same size
        void StitchBlockLight ();
        void StitchBlockSkyLight ();

        void StitchBlockLight (IBoundedLitBlockCollection blockset, BlockCollectionEdge edge);
        void StitchBlockSkyLight (IBoundedLitBlockCollection blockset, BlockCollectionEdge edge);
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
