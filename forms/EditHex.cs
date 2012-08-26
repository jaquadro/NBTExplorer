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
        private int _bytesPerElem;
        private byte[] _data;
        private bool _modified;
        DynamicByteProvider _byteProvider;

        private class FixedByteProvider : DynamicByteProvider
        {
            public FixedByteProvider (byte[] data)
                : base(data)
            { }

            public override bool SupportsInsertBytes ()
            {
                return false;
            }
        }

        public HexEditor (string tagName, byte[] data, int bytesPerElem)
        {
            InitializeComponent();

            this.Text = "Editing: " + tagName;

            _bytesPerElem = bytesPerElem;
            _curPositionLabel.Text = "0x0000";
            _curElementLabel.Text = "Element 0";

            _data = new byte[data.Length];
            Array.Copy(data, _data, data.Length);

            _byteProvider = new FixedByteProvider(_data);
            _byteProvider.Changed += (o, e) => { _modified = true; };

            hexBox1.ByteProvider = _byteProvider;

            hexBox1.HorizontalByteCountChanged += HexBox_HorizontalByteCountChanged;
            hexBox1.CurrentLineChanged += HexBox_CurrentLineChanged;
            hexBox1.CurrentPositionInLineChanged += HexBox_CurrentPositionInLineChanged;

            hexBox1.ReadOnly = false;
        }

        public byte[] Data
        {
            get { return _data; }
        }

        public bool Modified
        {
            get { return _modified; }
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
            long pos = (hexBox1.CurrentLine - 1) * hexBox1.HorizontalByteCount + hexBox1.CurrentPositionInLine - 1;

            _curPositionLabel.Text = "0x" + pos.ToString("X4");
            _curElementLabel.Text = "Element " + pos / _bytesPerElem;
        }

        private void Apply ()
        {
            long len = Math.Min(_data.Length, _byteProvider.Length);

            for (int i = 0; i < len; i++) {
                _data[i] = _byteProvider.Bytes[i];
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void _buttonOK_Click (object sender, EventArgs e)
        {
            Apply();
        }
    }
}
