using System;
using System.Windows.Forms;
using Be.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace NBTExplorer.Windows
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

            _byteProvider = new DynamicByteProvider(_data);
            _byteProvider.Changed += (o, e) => { _modified = true; };

            hexBox1.ByteProvider = _byteProvider;

            hexBox1.HorizontalByteCountChanged += HexBox_HorizontalByteCountChanged;
            hexBox1.CurrentLineChanged += HexBox_CurrentLineChanged;
            hexBox1.CurrentPositionInLineChanged += HexBox_CurrentPositionInLineChanged;
            hexBox1.InsertActiveChanged += HexBox_InsertActiveChanged;

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

        private void HexBox_InsertActiveChanged (object sender, EventArgs e)
        {
            if (hexBox1.InsertActive)
                _insertStateLabel.Text = "Insert";
            else
                _insertStateLabel.Text = "Overwrite";
        }

        private void UpdatePosition ()
        {
            long pos = (hexBox1.CurrentLine - 1) * hexBox1.HorizontalByteCount + hexBox1.CurrentPositionInLine - 1;

            _curPositionLabel.Text = "0x" + pos.ToString("X4");
            _curElementLabel.Text = "Element " + pos / _bytesPerElem;
        }

        private void Apply ()
        {
            if (_data.Length != _byteProvider.Length)
                _data = new byte[_byteProvider.Length];

            for (int i = 0; i < _data.Length; i++) {
                _data[i] = _byteProvider.Bytes[i];
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ImportRaw (string path)
        {
            try {
                using (FileStream fstr = File.OpenRead(path)) {
                    _data = new byte[fstr.Length];
                    fstr.Read(_data, 0, (int)fstr.Length);

                    _byteProvider = new DynamicByteProvider(_data);
                    _byteProvider.Changed += (o, e) => { _modified = true; };

                    hexBox1.ByteProvider = _byteProvider;
                    _modified = true;
                }
            }
            catch (Exception e) {
                MessageBox.Show("Failed to import data from \"" + path + "\"\n\nException: " + e.Message);
            }
        }

        private void ExportRaw (string path)
        {
            try {
                using (FileStream fstr = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    byte[] data = _byteProvider.Bytes.ToArray();
                    fstr.Write(data, 0, data.Length);
                }
            }
            catch (Exception e) {
                MessageBox.Show("Failed to export data to \"" + path + "\"\n\nException: " + e.Message);
            }
        }

        private void _buttonOK_Click (object sender, EventArgs e)
        {
            Apply();
        }

        private void _buttonImport_Click (object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()) {
                ofd.RestoreDirectory = true;
                ofd.Multiselect = false;
                ofd.Filter = "Binary Data|*|Text List (*.txt)|*.txt";
                ofd.FilterIndex = 0;

                if (ofd.ShowDialog() == DialogResult.OK) {
                    if (Path.GetExtension(ofd.FileName) == "txt")
                        return;
                    else
                        ImportRaw(ofd.FileName);
                }
            }
        }

        private void _buttonExport_Click (object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog()) {
                sfd.RestoreDirectory = true;
                sfd.Filter = "Binary Data|*|Text List (*.txt)|*.txt";
                sfd.FilterIndex = 0;
                sfd.OverwritePrompt = true;

                if (sfd.ShowDialog() == DialogResult.OK) {
                    if (Path.GetExtension(sfd.FileName) == "txt")
                        return;
                    else
                        ExportRaw(sfd.FileName);
                }
            }
        }
    }
}
