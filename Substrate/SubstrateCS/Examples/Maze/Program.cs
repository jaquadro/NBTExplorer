using System;
using System.Collections.Generic;
using Substrate;

namespace Maze
{
    class Program
    {
        static void Main (string[] args)
        {
            BetaWorld world = BetaWorld.Open("F:\\Minecraft\\test");
            BlockManager bm = world.GetBlockManager();

            bm.AutoLight = false;

            Grid grid = new Grid();
            grid.BuildInit(bm);

            Generator gen = new Generator();
            List<Generator.Edge> edges = gen.Generate();

            foreach (Generator.Edge e in edges) {
                int x1;
                int y1;
                int z1;
                gen.UnIndex(e.node1, out x1, out y1, out z1);

                int x2;
                int y2;
                int z2;
                gen.UnIndex(e.node2, out x2, out y2, out z2);

                grid.LinkRooms(bm, x1, y1, z1, x2, y2, z2);
            }

            // Entrance Room
            grid.BuildRoom(bm, 2, 5, 2);
            grid.LinkRooms(bm, 2, 5, 2, 1, 5, 2);
            grid.LinkRooms(bm, 2, 5, 2, 3, 5, 2);
            grid.LinkRooms(bm, 2, 5, 2, 2, 5, 1);
            grid.LinkRooms(bm, 2, 5, 2, 2, 5, 3);
            grid.LinkRooms(bm, 2, 4, 2, 2, 5, 2);

            // Exit Room
            grid.BuildRoom(bm, 2, -1, 2);
            grid.LinkRooms(bm, 2, -1, 2, 2, 0, 2);
            grid.AddPrize(bm, 2, -1, 2);

            Console.WriteLine("Relight Chunks");

            ChunkManager cm = world.GetChunkManager();
            cm.RelightDirtyChunks();

            world.Save();
        }

    }

    class Grid
    {
        int originx;
        int originy;
        int originz;

        int xlen;
        int ylen;
        int zlen;

        int cellxlen;
        int cellylen;
        int cellzlen;
        int wallxwidth;
        int wallywidth;
        int wallzwidth;

        public Grid ()
        {
            originx = 0;
            originy = 27;
            originz = 0;

            xlen = 5;
            ylen = 5;
            zlen = 5;

            cellxlen = 5;
            cellylen = 5;
            cellzlen = 5;
            wallxwidth = 2;
            wallywidth = 2;
            wallzwidth = 2;
        }

        public void BuildInit (BlockManager bm)
        {
            for (int xi = 0; xi < xlen; xi++) {
                for (int yi = 0; yi < ylen; yi++) {
                    for (int zi = 0; zi < zlen; zi++) {
                        BuildRoom(bm, xi, yi, zi);
                    }
                }
            }
        }

        public void BuildRoom (BlockManager bm, int x, int y, int z)
        {
            int ox = originx + (cellxlen + wallxwidth) * x;
            int oy = originy + (cellylen + wallywidth) * y;
            int oz = originz + (cellzlen + wallzwidth) * z;

            // Hollow out room
            for (int xi = 0; xi < cellxlen; xi++) {
                int xx = ox + wallxwidth + xi;
                for (int zi = 0; zi < cellzlen; zi++) {
                    int zz = oz + wallzwidth + zi;
                    for (int yi = 0; yi < cellylen; yi++) {
                        int yy = oy + wallywidth + yi;
                        bm.SetID(xx, yy, zz, (int)BlockType.AIR);
                    }
                }
            }

            // Build walls
            for (int xi = 0; xi < cellxlen + 2 * wallxwidth; xi++) {
                for (int zi = 0; zi < cellzlen + 2 * wallzwidth; zi++) {
                    for (int yi = 0; yi < wallywidth; yi++) {
                        bm.SetID(ox + xi, oy + yi, oz + zi, (int)BlockType.BEDROCK);
                        bm.SetID(ox + xi, oy + yi + cellylen + wallywidth, oz + zi, (int)BlockType.BEDROCK);
                    }
                }
            }

            for (int xi = 0; xi < cellxlen + 2 * wallxwidth; xi++) {
                for (int zi = 0; zi < wallzwidth; zi++) {
                    for (int yi = 0; yi < cellylen + 2 * wallywidth; yi++) {
                        bm.SetID(ox + xi, oy + yi, oz + zi, (int)BlockType.BEDROCK);
                        bm.SetID(ox + xi, oy + yi, oz + zi + cellzlen + wallzwidth, (int)BlockType.BEDROCK);
                    }
                }
            }

            for (int xi = 0; xi < wallxwidth; xi++) {
                for (int zi = 0; zi < cellzlen + 2 * wallzwidth; zi++) {
                    for (int yi = 0; yi < cellylen + 2 * wallywidth; yi++) {
                        bm.SetID(ox + xi, oy + yi, oz + zi, (int)BlockType.BEDROCK);
                        bm.SetID(ox + xi + cellxlen + wallxwidth, oy + yi, oz + zi, (int)BlockType.BEDROCK);
                    }
                }
            }

            // Torchlight
            bm.SetID(ox + wallxwidth, oy + wallywidth + 2, oz + wallzwidth + 1, (int)BlockType.TORCH);
            bm.SetID(ox + wallxwidth, oy + wallywidth + 2, oz + wallzwidth + cellzlen - 2, (int)BlockType.TORCH);
            bm.SetID(ox + wallxwidth + cellxlen - 1, oy + wallywidth + 2, oz + wallzwidth + 1, (int)BlockType.TORCH);
            bm.SetID(ox + wallxwidth + cellxlen - 1, oy + wallywidth + 2, oz + wallzwidth + cellzlen - 2, (int)BlockType.TORCH);
            bm.SetID(ox + wallxwidth + 1, oy + wallywidth + 2, oz + wallzwidth, (int)BlockType.TORCH);
            bm.SetID(ox + wallxwidth + cellxlen - 2, oy + wallywidth + 2, oz + wallzwidth, (int)BlockType.TORCH);
            bm.SetID(ox + wallxwidth + 1, oy + wallywidth + 2, oz + wallzwidth + cellzlen - 1, (int)BlockType.TORCH);
            bm.SetID(ox + wallxwidth + cellxlen - 2, oy + wallywidth + 2, oz + wallzwidth + cellzlen - 1, (int)BlockType.TORCH);
        }

        public void LinkRooms (BlockManager bm, int x1, int y1, int z1, int x2, int y2, int z2)
        {
            int xx = originx + (cellxlen + wallxwidth) * x1;
            int yy = originy + (cellylen + wallywidth) * y1;
            int zz = originz + (cellzlen + wallzwidth) * z1;

            if (x1 != x2) {
                xx = originx + (cellxlen + wallxwidth) * Math.Max(x1, x2);
                for (int xi = 0; xi < wallxwidth; xi++) {
                    int zc = zz + wallzwidth + (cellzlen / 2);
                    int yb = yy + wallywidth;
                    bm.SetID(xx + xi, yb, zc - 1, (int)BlockType.AIR);
                    bm.SetID(xx + xi, yb, zc, (int)BlockType.AIR);
                    bm.SetID(xx + xi, yb, zc + 1, (int)BlockType.AIR);
                    bm.SetID(xx + xi, yb + 1, zc - 1, (int)BlockType.AIR);
                    bm.SetID(xx + xi, yb + 1, zc, (int)BlockType.AIR);
                    bm.SetID(xx + xi, yb + 1, zc + 1, (int)BlockType.AIR);
                    bm.SetID(xx + xi, yb + 2, zc, (int)BlockType.AIR);
                }
            }
            else if (z1 != z2) {
                zz = originz + (cellzlen + wallzwidth) * Math.Max(z1, z2);
                for (int zi = 0; zi < wallxwidth; zi++) {
                    int xc = xx + wallxwidth + (cellxlen / 2);
                    int yb = yy + wallywidth;
                    bm.SetID(xc - 1, yb, zz + zi, (int)BlockType.AIR);
                    bm.SetID(xc, yb, zz + zi, (int)BlockType.AIR);
                    bm.SetID(xc + 1, yb, zz + zi, (int)BlockType.AIR);
                    bm.SetID(xc - 1, yb + 1, zz + zi, (int)BlockType.AIR);
                    bm.SetID(xc, yb + 1, zz + zi, (int)BlockType.AIR);
                    bm.SetID(xc + 1, yb + 1, zz + zi, (int)BlockType.AIR);
                    bm.SetID(xc, yb + 2, zz + zi, (int)BlockType.AIR);
                }
            }
            else if (y1 != y2) {
                yy = originy + (cellylen + wallywidth) * Math.Max(y1, y2);
                for (int yi = 0 - cellylen + 1; yi < wallywidth + 1; yi++) {
                    int xc = xx + wallxwidth + (cellxlen / 2);
                    int zc = zz + wallzwidth + (cellzlen / 2);

                    bm.SetID(xc, yy + yi, zc, (int)BlockType.BEDROCK);
                    bm.SetID(xc - 1, yy + yi, zc, (int)BlockType.LADDER);
                    bm.SetData(xc - 1, yy + yi, zc, 4);
                    bm.SetID(xc + 1, yy + yi, zc, (int)BlockType.LADDER);
                    bm.SetData(xc + 1, yy + yi, zc, 5);
                    bm.SetID(xc, yy + yi, zc - 1, (int)BlockType.LADDER);
                    bm.SetData(xc, yy + yi, zc - 1, 2);
                    bm.SetID(xc, yy + yi, zc + 1, (int)BlockType.LADDER);
                    bm.SetData(xc, yy + yi, zc + 1, 3);
                }
            }
        }

        public void AddPrize (BlockManager bm, int x, int y, int z)
        {
            int ox = originx + (cellxlen + wallxwidth) * x + wallxwidth;
            int oy = originy + (cellylen + wallywidth) * y + wallywidth;
            int oz = originz + (cellzlen + wallzwidth) * z + wallzwidth;

            Random rand = new Random();
            for (int xi = 0; xi < cellxlen; xi++) {
                for (int zi = 0; zi < cellzlen; zi++) {
                    if (rand.NextDouble() < 0.1) {
                        bm.SetID(ox + xi, oy, oz + zi, (int)BlockType.DIAMOND_BLOCK);
                    }
                }
            }
        }
    }

    class Generator
    {
        public struct Edge
        {
            public int node1;
            public int node2;

            public Edge (int n1, int n2)
            {
                node1 = n1;
                node2 = n2;
            }
        }

        int xlen;
        int ylen;
        int zlen;

        List<Edge> _edges;
        int[] _cells;

        public Generator ()
        {
            xlen = 5;
            ylen = 5;
            zlen = 5;

            _edges = new List<Edge>();
            _cells = new int[xlen * zlen * ylen];

            for (int x = 0; x < xlen; x++) {
                for (int z = 0; z < zlen; z++) {
                    for (int y = 0; y < ylen; y++) {
                        int n1 = Index(x, y, z);
                        _cells[n1] = n1;
                    }
                }
            }

            for (int x = 0; x < xlen - 1; x++) {
                for (int z = 0; z < zlen; z++) {
                    for (int y = 0; y < ylen; y++) {
                        int n1 = Index(x, y, z);
                        int n2 = Index(x + 1, y, z);
                        _edges.Add(new Edge(n1, n2));
                    }
                }
            }

            for (int x = 0; x < xlen; x++) {
                for (int z = 0; z < zlen - 1; z++) {
                    for (int y = 0; y < ylen; y++) {
                        int n1 = Index(x, y, z);
                        int n2 = Index(x, y, z + 1);
                        _edges.Add(new Edge(n1, n2));
                    }
                }
            }

            for (int x = 0; x < xlen; x++) {
                for (int z = 0; z < zlen; z++) {
                    for (int y = 0; y < ylen - 1; y++) {
                        int n1 = Index(x, y, z);
                        int n2 = Index(x, y + 1, z);
                        _edges.Add(new Edge(n1, n2));
                    }
                }
            }
        }

        public List<Edge> Generate ()
        {
            Random rand = new Random();

            List<Edge> passages = new List<Edge>();

            // Randomize edges
            Queue<Edge> redges = new Queue<Edge>();
            while (_edges.Count > 0) {
                int index = rand.Next(_edges.Count);
                Edge e = _edges[index];
                _edges.RemoveAt(index);
                redges.Enqueue(e);
            }

            while (redges.Count > 0) {
                Edge e = redges.Dequeue();
                
                if (_cells[e.node1] == _cells[e.node2]) {
                    continue;
                }

                passages.Add(e);

                int n1 = _cells[e.node2];
                int n2 = _cells[e.node1];
                for (int i = 0; i < _cells.Length; i++) {
                    if (_cells[i] == n2) {
                        _cells[i] = n1;
                    }
                }
            }

            return passages;
        }

        public int Index (int x, int y, int z)
        {
            return (x * zlen + z) * ylen + y;
        }

        public void UnIndex (int index, out int x, out int y, out int z)
        {
            x = index / (zlen * ylen);
            int xstr = index - (x * zlen * ylen);
            z = xstr / ylen;
            int ystr = xstr - (z * ylen);
            y = ystr;
        }
    }
}
