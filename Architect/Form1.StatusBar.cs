using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace TestApp
{
    partial class Form1
    {
        const int PROGRESS_INDEX = 2;
        const int ZOOM_INDEX = PROGRESS_INDEX + 3;
        const int DAYLIGHT_INDEX = ZOOM_INDEX + 4;

        private TrackBar trackBarZoom;
        private TrackBar trackBarDaylight;

        private ToolStripSeparator sepProgress;
        private ToolStripSeparator sepZoom;
        private ToolStripSeparator sepDaylight;

        private void InitStatusBar ()
        {
            // Progress Bar

            sepProgress = new System.Windows.Forms.ToolStripSeparator();

            this.statusStrip1.Items.Insert(1, sepProgress);
            sepProgress.Margin = new System.Windows.Forms.Padding(8, 0, 4, 0);

            toolStripProgressBar1.Visible = true;

            // Zoom Trackbar

            sepZoom = new System.Windows.Forms.ToolStripSeparator();

            this.statusStrip1.Items.Insert(PROGRESS_INDEX + 1, sepZoom);
            sepZoom.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);

            trackBarZoom = new TrackBar();
            trackBarZoom.AutoSize = false;
            trackBarZoom.Height = 22;
            trackBarZoom.TickStyle = TickStyle.None;
            trackBarZoom.Anchor = AnchorStyles.Right;
            trackBarZoom.Minimum = 0;
            trackBarZoom.Maximum = 9;
            trackBarZoom.Value = 5;
            ToolStripItem tsi = new ToolStripControlHost(trackBarZoom);
            this.statusStrip1.Items.Insert(ZOOM_INDEX + 1, tsi);

            // Daylight Trackbar

            sepDaylight = new System.Windows.Forms.ToolStripSeparator();

            this.statusStrip1.Items.Insert(ZOOM_INDEX + 3, sepDaylight);
            sepDaylight.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);

            trackBarDaylight = new TrackBar();
            trackBarDaylight.AutoSize = false;
            trackBarDaylight.Height = 22;
            trackBarDaylight.TickStyle = TickStyle.None;
            trackBarDaylight.Anchor = AnchorStyles.Right;
            trackBarDaylight.Maximum = 15;
            trackBarDaylight.Value = 15;
            ToolStripItem tsi2 = new ToolStripControlHost(trackBarDaylight);
            this.statusStrip1.Items.Insert(DAYLIGHT_INDEX + 1, tsi2);

            // Event Handlers

            trackBarZoom.Scroll += trackBarZoom_Scroll;

            toolStripStatusLabel3.MouseEnter += buttonZoomOut_MouseEnter;
            toolStripStatusLabel3.MouseLeave += buttonZoomOut_MouseLeave;
            toolStripStatusLabel3.MouseDown += buttonZoomOut_MouseDown;
            toolStripStatusLabel3.MouseUp += buttonZoomOut_MouseUp;

            toolStripStatusLabel4.MouseEnter += buttonZoomIn_MouseEnter;
            toolStripStatusLabel4.MouseLeave += buttonZoomIn_MouseLeave;
            toolStripStatusLabel4.MouseDown += buttonZoomIn_MouseDown;
            toolStripStatusLabel4.MouseUp += buttonZoomIn_MouseUp;
        }

        // Zoom In

        public void buttonZoomIn_MouseEnter (object sender, EventArgs e)
        {
            toolStripStatusLabel4.Image = global::TestApp.Properties.Resources.plus16_mo;
        }

        public void buttonZoomIn_MouseLeave (object sender, EventArgs e)
        {
            toolStripStatusLabel4.Image = global::TestApp.Properties.Resources.plus16;
        }

        public void buttonZoomIn_MouseDown (object sender, EventArgs e)
        {
            toolStripStatusLabel4.Image = global::TestApp.Properties.Resources.plus16_clk;
        }

        public void buttonZoomIn_MouseUp (object sender, EventArgs e)
        {
            toolStripStatusLabel4.Image = global::TestApp.Properties.Resources.plus16_mo;
            if (trackBarZoom.Value < trackBarZoom.Maximum) {
                trackBarZoom.Value++;
                render2.SetZoom(trackBarZoom.Value);
            }
        }

        // Zoom Out

        public void buttonZoomOut_MouseEnter (object sender, EventArgs e)
        {
            toolStripStatusLabel3.Image = global::TestApp.Properties.Resources.minus16_mo;
        }

        public void buttonZoomOut_MouseLeave (object sender, EventArgs e)
        {
            toolStripStatusLabel3.Image = global::TestApp.Properties.Resources.minus16;
        }

        public void buttonZoomOut_MouseDown (object sender, EventArgs e)
        {
            toolStripStatusLabel3.Image = global::TestApp.Properties.Resources.minus16_clk;
        }

        public void buttonZoomOut_MouseUp (object sender, EventArgs e)
        {
            toolStripStatusLabel3.Image = global::TestApp.Properties.Resources.minus16_mo;
            if (trackBarZoom.Value > trackBarZoom.Minimum) {
                trackBarZoom.Value--;
                render2.SetZoom(trackBarZoom.Value);
            }
        }

        // Zoom Bar

        private void trackBarZoom_Scroll (object sender, EventArgs e)
        {
            render2.SetZoom(trackBarZoom.Value);
        }
    }
}
