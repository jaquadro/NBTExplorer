using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using NBT;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public MC.World world;

        private bool glLoaded = false;

        private Render2D render2;

        public Form1()
        {
            InitializeComponent();

            this.Shown += Form1_Shown;

            // Callback stuff
            hScrollBar1.Scroll += hScrollBar1_Scroll;
            vScrollBar1.Scroll += vScrollBar1_Scroll;

            InitStatusBar();
        }

        public void updateProgressBar (object sender, MC.WorldLoadStepEventArgs e)
        {
            toolStripProgressBar1.Maximum = e.maxValue;
            toolStripProgressBar1.Value = e.value;

            toolStripProgressBar1.Invalidate();
        }

        public void PostLoadMap (object sender, EventArgs e)
        {
            toolStripProgressBar1.Visible = false;

            /*hScrollBar1.Minimum = world.GetMinX() * 16;
            hScrollBar1.Maximum = world.GetMaxX() * 16;
            vScrollBar1.Minimum = world.GetMinZ() * 16;
            vScrollBar1.Maximum = world.GetMaxZ() * 16;*/

            render2 = new Render2D();
            render2.glControl = glControl1;
            render2.world = world;

            render2.SetHScrollLimits(hScrollBar1);
            render2.SetVScrollLimits(vScrollBar1);
        }

        private void Form1_Shown (object sender, EventArgs e)
        {
            world.Initialize();
        }

        private void hScrollBar1_Scroll (object sender, ScrollEventArgs e)
        {
            render2.SetPosition(vScrollBar1.Value, e.NewValue);
            glControl1.Invalidate();
        }

        private void vScrollBar1_Scroll (object sender, ScrollEventArgs e)
        {
            render2.SetPosition(e.NewValue, hScrollBar1.Value);
            glControl1.Invalidate();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click (object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStrip1_ItemClicked (object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void glControl1_Load (object sender, EventArgs e)
        {
            glLoaded = true;
            GL.ClearColor(Color.Black);

            glControl1.Paint += glControl1_Paint;
            glControl1.MouseClick += glControl1_MouseClick;
        }

        private void glControl1_MouseClick (object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }

        private void glControl1_Paint (object sender, PaintEventArgs e)
        {
            if (!glLoaded) {
                return;
            }

            if (render2 != null) {
                render2.Render();
            }

            /*MC.Chunk chunk = world.CoordToChunk(world.GetSpawnX(), world.GetSpawnY(), world.GetSpawnZ());
            NBT.NBTChunk c2 = (NBT.NBTChunk)chunk;
            c2.LoadChunk();
            c2.ActivateChunk();

            if (chunk != null) {
                int cx = chunk.GetX() * 16;
                int cz = chunk.GetZ() * 16;

                int w = glControl1.Width;
                int h = glControl1.Height;
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho((cz - 16) * 32, (cz - 16) * 32 + w, (cx - 16) * 32, (cx - 16) * 32 + h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
                Console.WriteLine("GL.Ortho({0}, {1}, {2}, {3})", (cz - 16) * 32, cz * 32, (cx - 16) * 32, cx * 32);
                GL.Viewport(0, 0, w, h); // Use all of the glControl painting area

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                for (int z = 0; z < 16; z++) {
                    for (int x = 0; x < 16; x++) {
                        for (int y = 127; y >= 0; y--) {
                            int blockid = chunk.GetBlockId(x, y, z);
                            if (blockid == 0) {
                                continue;
                            }

                            uint c = BlockInfo.Index[blockid].color();
                            byte r = (byte)((c & 0xFF0000) >> 16);
                            byte g = (byte)((c & 0xFF00) >> 8);
                            byte b = (byte)((c & 0xFF));
                            byte a = (byte)((c & 0xFF000000) >> 24);

                            int relx = cx - x - 1;
                            int relz = cz - z - 1;

                            GL.Color4(r, g, b, a);
                            GL.Begin(BeginMode.Quads);
                            GL.Vertex2(relz * 32, relx * 32);
                            GL.Vertex2(relz * 32, (relx + 1) * 32);
                            GL.Vertex2((relz + 1) * 32, (relx + 1) * 32);
                            GL.Vertex2((relz + 1) * 32, relx * 32);
                            GL.End();

                            break;
                        }
                    }
                }
            }
            else {
                Console.WriteLine("chunk null");
            }*/

            glControl1.SwapBuffers();
        }
    }
}
