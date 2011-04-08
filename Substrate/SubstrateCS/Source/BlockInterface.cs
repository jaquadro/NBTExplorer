using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    public interface IBlock
    {
        BlockInfo Info { get; }
        int ID { get; set; }
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
        bool SetTileEntity (TileEntity te);
        bool ClearTileEntity ();
    }

    public interface IBlockContainer
    {
        IBlock GetBlock (int x, int y, int z);
        IBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, IBlock block);

        BlockInfo GetBlockInfo (int x, int y, int z);

        int GetBlockID (int x, int y, int z);
        int GetBlockData (int x, int y, int z);

        bool SetBlockID (int x, int y, int z, int id);
        bool SetBlockData (int x, int y, int z, int data);
    }

    public interface IBoundedBlockContainer : IBlockContainer
    {
        int XDim { get; }
        int YDim { get; }
        int ZDim { get; }
    }

    public interface IResizableBlockContainer : IBoundedBlockContainer
    {
        new int XDim { get; set; }
        new int YDim { get; set; }
        new int ZDim { get; set; }
    }

    public interface ILitBlockContainer : IBlockContainer
    {
        new ILitBlock GetBlock (int x, int y, int z);
        new ILitBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, ILitBlock block);

        int GetBlockLight (int x, int y, int z);
        int GetBlockSkyLight (int x, int y, int z);

        bool SetBlockLight (int x, int y, int z, int light);
        bool SetBlockSkyLight (int x, int y, int z, int light);
    }

    public interface IPropertyBlockContainer : IBlockContainer
    {
        new IPropertyBlock GetBlock (int x, int y, int z);
        new IPropertyBlock GetBlockRef (int x, int y, int z);

        void SetBlock (int x, int y, int z, IPropertyBlock block);

        TileEntity GetTileEntity (int x, int y, int z);
        bool SetTileEntity (int x, int y, int z, TileEntity te);
        bool ClearTileEntity (int x, int y, int z);
    }

    public interface IAlphaBlockContainer : ILitBlockContainer, IPropertyBlockContainer
    {
    }

    public interface IBlockManager : IAlphaBlockContainer
    {
    }

    /*public interface IBlock
    {
        BlockInfo Info { get; }
        int ID { get; set; }
        int Data { get; set; }
        int BlockLight { get; set; }
        int SkyLight { get; set; }

        TileEntity GetTileEntity ();
        bool SetTileEntity (TileEntity te);
        bool ClearTileEntity ();
    }

    public interface IBlockContainer
    {
        int BlockGlobalX (int x);
        int BlockGlobalY (int y);
        int BlockGlobalZ (int z);

        int BlockLocalX (int x);
        int BlockLocalY (int y);
        int BlockLocalZ (int z);

        Block GetBlock (int lx, int ly, int lz);
        BlockRef GetBlockRef (int lx, int ly, int lz);

        BlockInfo GetBlockInfo (int lx, int ly, int lz);

        int GetBlockID (int lx, int ly, int lz);
        int GetBlockData (int lx, int ly, int lz);
        int GetBlockLight (int lx, int ly, int lz);
        int GetBlockSkyLight (int lx, int ly, int lz);

        void SetBlock (int lx, int ly, int lz, Block block);

        bool SetBlockID (int lx, int ly, int lz, int id);
        bool SetBlockData (int lx, int ly, int lz, int data);
        bool SetBlockLight (int lx, int ly, int lz, int light);
        bool SetBlockSkyLight (int lx, int ly, int lz, int light);

        TileEntity GetTileEntity (int lx, int ly, int lz);
        bool SetTileEntity (int lx, int ly, int lz, TileEntity te);
        bool ClearTileEntity (int lx, int ly, int lz);
    }

    public interface IBlockManager : IBlockContainer
    {

    }*/
}
