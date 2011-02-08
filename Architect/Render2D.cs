using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace TestApp
{
    class Render2D
    {
        static int[] ZOOM_LEVELS = { 1, 2, 4, 8, 12, 16, 24, 32, 48, 64 };

        public OpenTK.GLControl glControl;
        public MC.World world;

        int zoom = 5;

        // Position is in 8px ticks
        // Position points to lower-left corner of window
        int posX = -125 * 2;   // North/South
        int posZ = -4 * 2;   // East/West

        public void SetHScrollLimits (System.Windows.Forms.HScrollBar scroll)
        {
            scroll.Minimum = (world.GetMaxZ() * -16 * ZOOM_LEVELS[zoom]) / 8;
            scroll.Maximum = (world.GetMinZ() * -16 * ZOOM_LEVELS[zoom]) / 8;
        }

        public void SetVScrollLimits (System.Windows.Forms.VScrollBar scroll)
        {
            scroll.Minimum = (world.GetMinX() * 16 * ZOOM_LEVELS[zoom]) / 8;
            scroll.Maximum = (world.GetMaxX() * 16 * ZOOM_LEVELS[zoom]) / 8;
        }

        public void SetPosition (int scrollX, int scrollZ)
        {
            posX = scrollX * 1;
            posZ = scrollZ * -1;
        }

        public Util.Coord2 GetPositionByBlock (Util.Coord2 uc2)
        {
            return GetPositionByBlock(uc2.x, uc2.z);
        }

        public Util.Coord2 GetPositionByBlock (int blockX, int blockZ)
        {
            int x = blockX * ZOOM_LEVELS[zoom] / 8;
            int z = blockZ * ZOOM_LEVELS[zoom] / -8;
            return new Util.Coord2(x, z);
        }

        public void SetZoom (int z)
        {
            if (z < 0) {
                z = 0;
            }
            if (z > ZOOM_LEVELS.Length) {
                z = ZOOM_LEVELS.Length - 1;
            }

            zoom = z;
            glControl.Invalidate();
        }

        public void Render ()
        {
            SetupViewport();

            int blockX = posX * 8 / ZOOM_LEVELS[zoom];
            int blockZ = posZ * 8 / ZOOM_LEVELS[zoom];

            MC.Chunk c = world.CoordToChunk(blockX, 0, blockZ);
            RenderChunk(c);
            RenderChunk(c.nWest);
            RenderChunk(c.nNorth);
            RenderChunk(c.nEast);
            RenderChunk(c.nSouth);
            RenderChunk(c.nWest.nNorth);
            RenderChunk(c.nEast.nNorth);
            RenderChunk(c.nWest.nSouth);
            RenderChunk(c.nEast.nSouth);
        }

        public void SetupViewport ()
        {
            int w = glControl.Width;
            int h = glControl.Height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            int pxX = posX * -8 - (16 * ZOOM_LEVELS[zoom]);
            int pxZ = posZ * -8 - (16 * ZOOM_LEVELS[zoom]);

            GL.Ortho(pxZ, pxZ + w, pxX, pxX + h, -1, 1);
            Console.WriteLine("GL.Ortho({0}, {1}, {2}, {3})", pxZ, pxZ + w, pxX, pxX + h);
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void RenderChunk (MC.Chunk chunk)
        {
            if (chunk == null) {
                return;
            }

            world.ActivateChunk(chunk.GetX(), chunk.GetZ());

            int cx = chunk.GetX() * -16;
            int cz = chunk.GetZ() * -16;

            for (int z = 0; z < 16; z++) {
                for (int x = 0; x < 16; x++) {
                    for (int y = 127; y >= 0; y--) {
                        int blockid = chunk.GetBlockId(x, y, z);
                        if (blockid == 0) {
                            continue;
                        }

                        int relx = cx - x - 1;
                        int relz = cz - z - 1;

                        NBT.Block blk = NBT.BlockInfo.Index[blockid];
                        if (blk != null) {
                            blk.model().Draw2D(blk, relx, relz, ZOOM_LEVELS[zoom]);
                        }

                        break;
                    }
                }
            }
        }
    }
}
