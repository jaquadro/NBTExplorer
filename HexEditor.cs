using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace NBTExplorer
{
    public partial class HexEditor : Form
    {
        public HexEditor (string tagName, byte[] data)
        {
            InitializeComponent();

            this.Text = "Editing: " + tagName + " (Read Only)";

            hexBox1.ByteProvider = new DynamicByteProvider(data);

            hexBox1.HorizontalByteCountChanged += HexBox_HorizontalByteCountChanged;
            hexBox1.CurrentLineChanged += HexBox_CurrentLineChanged;
            hexBox1.CurrentPositionInLineChanged += HexBox_CurrentPositionInLineChanged;
        }

        private void HexBox_HorizontalByteCountChanged (object sender, EventArgs e)
        {
            UpdatePosition();
        }

        private void HexBox_CurrentLineChanged (object sender, EventArgs e)
        {
            UpdatePosition();
        }

        private void HexBox_CurrentPositionInLineChanged (object sender, EventArgs e)
        {
            UpdatePosition();
        }

        private void UpdatePosition ()
        {
            long pos = (hexBox1.CurrentLine - 1) * hexBox1.HorizontalByteCount + hexBox1.CurrentPositionInLine;

            _curPositionLabel.Text = pos.ToString();
        }
    }
}
