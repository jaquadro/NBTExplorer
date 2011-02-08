using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MC
{
    public class Chunk
    {
        public Chunk nNorth = null;
        public Chunk nEast = null;
        public Chunk nSouth = null;
        public Chunk nWest = null;

        virtual public int GetX ()
        {
            return 0;
        }

        virtual public int GetZ ()
        {
            return 0;
        }

        virtual public Chunk GetNorthNeighbor ()
        {
            return nNorth;
        }

        virtual public Chunk GetSouthNeighbor ()
        {
            return nSouth;
        }

        virtual public Chunk GetEastNeighbor ()
        {
            return nEast;
        }

        virtual public Chunk GetWestNeighbor ()
        {
            return nWest;
        }

        virtual public int GetBlockId (int x, int y, int z)
        {
            return 0;
        }

        virtual public void SetBlockId (int x, int y, int z, int id) { }

        virtual public int GetBlockData (int x, int y, int z)
        {
            return 0;
        }

        virtual public void SetBlockData (int x, int y, int z, int data) { }

        virtual public int GetBlockLight (int x, int y, int z)
        {
            return 0;
        }

        virtual public void SetBlockLight (int x, int y, int z, int data) { }
    }
}
