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
}
