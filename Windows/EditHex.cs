using System;
using System.Windows.Forms;
using Be.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;

namespace NBTExplorer.Windows
{
    public partial class HexEditor : Form
    {
        private abstract class EditView
        {
            protected EditView (StatusStrip statusBar, int bytesPerElem)
            {
                BytesPerElem = bytesPerElem;
                StatusBar = statusBar;
            }

            public event EventHandler Modified;

            public int BytesPerElem { get; set; }
            public StatusStrip StatusBar { get; set; }

            public abstract TabPage TabPage { get; }

            public abstract void Initialize ();
            public abstract void Activate ();
            public abstract byte[] GetRawData ();
            public abstract void SetRawData (byte[] data);

            protected virtual void OnModified ()
            {
                var ev = Modified;
                if (ev != null)
                    ev(this, EventArgs.Empty);
            }
        }

        private class TextView : EditView
        {
            private TabPage _tabPage;
            private TextBox _textBox;

            private ToolStripStatusLabel _elementLabel;
            private ToolStripStatusLabel _spaceLabel;

            public TextView (StatusStrip statusBar, int bytesPerElem)
                : base(statusBar, bytesPerElem)
            { }

            public override void Initialize ()
            {
                _textBox = new TextBox() {
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(0, 0),
                    Margin = new Padding(0),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Size = new Size(500, 263),
                    TabIndex = 0,
                };

                _tabPage = new TabPage() {
                    Location = new Point(4, 22),
                    Padding = new Padding(3),
                    Size = new Size(500, 263),
                    TabIndex = 1,
                    Text = "Text View",
                    UseVisualStyleBackColor = true,
                };

                _tabPage.Controls.Add(_textBox);

                _textBox.TextChanged += (s, e) => { OnModified(); };

                InitializeStatusBar();
            }

            private void InitializeStatusBar ()
            {
                _elementLabel = new ToolStripStatusLabel() {
                    Size = new Size(100, 17),
                    Text = "Element 0",
                    TextAlign = ContentAlignment.MiddleLeft,
                };

                _spaceLabel = new ToolStripStatusLabel() {
                    Spring = true,
                };
            }

            public override void Activate ()
            {
                StatusBar.Items.Clear();
                StatusBar.Items.AddRange(new ToolStripItem[] {
                    _elementLabel, _spaceLabel,
                });
            }

            public override TabPage TabPage
            {
                get { return _tabPage; }
            }

            public override byte[] GetRawData ()
            {
                return HexEditor.TextToRaw(_textBox.Text, BytesPerElem);
            }

            public override void SetRawData (byte[] data)
            {
                _textBox.Text = HexEditor.RawToText(data, BytesPerElem);
            }
        }

        private class HexView : EditView
        {
            private TabPage _tabPage;
            private HexBox _hexBox;

            private ToolStripStatusLabel _positionLabel;
            private ToolStripStatusLabel _elementLabel;
            private ToolStripStatusLabel _spaceLabel;
            private ToolStripStatusLabel _insertLabel;

            private DynamicByteProvider _byteProvider;

            public HexView (StatusStrip statusBar, int bytesPerElem)
                : base(statusBar, bytesPerElem)
            { }

            public override void Initialize ()
            {
                _byteProvider = new DynamicByteProvider(new byte[0]);
                _byteProvider.Changed += (o, e) => { OnModified(); };

                _hexBox = new HexBox() {
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    LineInfoForeColor = Color.Empty,
                    LineInfoVisible = true,
                    Location = new Point(0, 0),
                    ShadowSelectionColor = Color.FromArgb(100, 60, 188, 255),
                    Size = new Size(500, 263),
                    TabIndex = 0,
                    VScrollBarVisible = true,
                    ByteProvider = _byteProvider,
                };

                _tabPage = new TabPage() {
                    Location = new Point(4, 22),
                    Padding = new Padding(3),
                    Size = new Size(500, 263),
                    TabIndex = 0,
                    Text = "Hex View",
                    UseVisualStyleBackColor = true,
                };

                _tabPage.Controls.Add(_hexBox);

                _hexBox.HorizontalByteCountChanged += (s, e) => { UpdatePosition(); };
                _hexBox.CurrentLineChanged += (s, e) => { UpdatePosition(); };
                _hexBox.CurrentPositionInLineChanged += (s, e) => { UpdatePosition(); };

                _hexBox.InsertActiveChanged += (s, e) => { _insertLabel.Text = (_hexBox.InsertActive) ? "Insert" : "Overwrite"; };

                InitializeStatusBar();
            }

            private void InitializeStatusBar ()
            {
                _positionLabel = new ToolStripStatusLabel() {
                    AutoSize = false,
                    Size = new Size(100, 17),
                    Text = "0000",
                };

                _elementLabel = new ToolStripStatusLabel() {
                    Size = new Size(59, 17),
                    Text = "Element 0",
                    TextAlign = ContentAlignment.MiddleLeft,
                };

                _spaceLabel = new ToolStripStatusLabel() {
                    Size = new Size(300, 17),
                    Spring = true,
                };

                _insertLabel = new ToolStripStatusLabel() {
                    Size = new Size(58, 17),
                    Text = (_hexBox.InsertActive) ? "Insert" : "Overwrite",
                };
            }

            public override void Activate ()
            {
                StatusBar.Items.Clear();
                StatusBar.Items.AddRange(new ToolStripItem[] {
                    _positionLabel, _elementLabel, _spaceLabel, _insertLabel,
                });

                UpdatePosition();
            }

            public override TabPage TabPage
            {
                get { return _tabPage; }
            }

            public override byte[] GetRawData ()
            {
                byte[] data = new byte[_byteProvider.Length];
                for (int i = 0; i < data.Length; i++) {
                    data[i] = _byteProvider.Bytes[i];
                }

                return data;
            }

            public override void SetRawData (byte[] data)
            {
                _byteProvider = new DynamicByteProvider(data);
                _byteProvider.Changed += (o, e2) => { OnModified(); };

                _hexBox.ByteProvider = _byteProvider;
            }

            private void UpdatePosition ()
            {
                long pos = (_hexBox.CurrentLine - 1) * _hexBox.HorizontalByteCount + _hexBox.CurrentPositionInLine - 1;

                _positionLabel.Text = "0x" + pos.ToString("X4");
                _elementLabel.Text = "Element " + pos / BytesPerElem;
            }
        }

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

        private TabPage _previousPage;
        private int _bytesPerElem;
        private byte[] _data;
        private bool _modified;
        //DynamicByteProvider _byteProvider;

        private Dictionary<TabPage, EditView> _views = new Dictionary<TabPage, EditView>();

        public HexEditor (string tagName, byte[] data, int bytesPerElem)
        {
            InitializeComponent();

            TextView textView = new TextView(statusStrip1, bytesPerElem);
            textView.Initialize();
            _views.Add(textView.TabPage, textView);
            viewTabs.TabPages.Add(textView.TabPage);

            if (!IsMono()) {
                HexView hexView = new HexView(statusStrip1, bytesPerElem);
                hexView.Initialize();
                _views.Add(hexView.TabPage, hexView);
                viewTabs.TabPages.Add(hexView.TabPage);
            }

            viewTabs.Deselected += (o, e) => { _previousPage = e.TabPage; };
            viewTabs.Selecting += HandleTabChanged;
            //textBox1.TextChanged += (o, e) => { _modified = true; };

            this.Text = "Editing: " + tagName;

            _bytesPerElem = bytesPerElem;
            //_curPositionLabel.Text = "0x0000";
            //_curElementLabel.Text = "Element 0";

            _data = new byte[data.Length];
            Array.Copy(data, _data, data.Length);

            //_byteProvider = new DynamicByteProvider(_data);
            //_byteProvider.Changed += (o, e) => { _modified = true; };

            //hexBox1.ByteProvider = _byteProvider;

            //hexBox1.HorizontalByteCountChanged += HexBox_HorizontalByteCountChanged;
            //hexBox1.CurrentLineChanged += HexBox_CurrentLineChanged;
            //hexBox1.CurrentPositionInLineChanged += HexBox_CurrentPositionInLineChanged;
            //hexBox1.InsertActiveChanged += HexBox_InsertActiveChanged;

            //hexBox1.ReadOnly = false;

            //textBox1.Text = RawToText(data);
        }

        private bool IsMono ()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        public byte[] Data
        {
            get { return _data; }
        }

        public bool Modified
        {
            get { return _modified; }
        }

        private void HandleTabChanged (object sender, TabControlCancelEventArgs e)
        {
            if (e.Action != TabControlAction.Selecting)
                return;

            if (e.TabPage == _previousPage)
                return;

            EditView oldView = _views[_previousPage];
            EditView newView = _views[e.TabPage];

            byte[] data = oldView.GetRawData();
            newView.SetRawData(data);

            newView.Activate();

            /*if (e.TabPage == textView) {
                if (_previousPage == textView)
                    return;

                byte[] data = DataFromHexBox();
                textBox1.Text = RawToText(data);

                _insertStateLabel.Text = "Insert";
            }
            else if (e.TabPage == hexView) {
                if (_previousPage == hexView)
                    return;

                byte[] data = TextToRaw(textBox1.Text);
                _byteProvider = new DynamicByteProvider(data);
                _byteProvider.Changed += (o, e2) => { _modified = true; };

                hexBox1.ByteProvider = _byteProvider;

                if (hexBox1.InsertActive)
                    _insertStateLabel.Text = "Insert";
                else
                    _insertStateLabel.Text = "Overwrite";
            }*/
        }

        /*private void HexBox_HorizontalByteCountChanged (object sender, EventArgs e)
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
        }*/

        private void Apply ()
        {
            EditView view = _views[viewTabs.SelectedTab];
            _data = view.GetRawData();

            DialogResult = DialogResult.OK;
            Close();
        }

        /*private void ApplyHex ()
        {
            if (_data.Length != _byteProvider.Length)
                _data = new byte[_byteProvider.Length];

            for (int i = 0; i < _data.Length; i++) {
                _data[i] = _byteProvider.Bytes[i];
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ApplyText ()
        {
            _data = TextToRaw(textBox1.Text);

            DialogResult = DialogResult.OK;
            Close();
        }*/

        /*private byte[] DataFromHexBox ()
        {
            byte[] data = new byte[_byteProvider.Length];
            for (int i = 0; i < data.Length; i++) {
                data[i] = _byteProvider.Bytes[i];
            }

            return data;
        }*/

        private String RawToText (byte[] data)
        {
            return RawToText(data, _bytesPerElem);
        }

        private static String RawToText (byte[] data, int bytesPerElem)
        {
            switch (bytesPerElem) {
                case 1: return RawToText(data, bytesPerElem, 16);
                case 2: return RawToText(data, bytesPerElem, 8);
                case 4: return RawToText(data, bytesPerElem, 4);
                case 8: return RawToText(data, bytesPerElem, 2);
                default: return RawToText(data, bytesPerElem, 1);
            }
        }

        //private String RawToText (byte[] data, int elementsPerLine)
        //{
        //    return RawToText(data, _bytesPerElem, elementsPerLine);
        //}

        private static String RawToText (byte[] data, int bytesPerElem, int elementsPerLine)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < data.Length; i += bytesPerElem) {
                if (data.Length - i < bytesPerElem)
                    break;

                switch (bytesPerElem) {
                    case 1:
                        builder.Append(((sbyte)data[i]).ToString());
                        break;

                    case 2:
                        builder.Append(BitConverter.ToInt16(data, i).ToString());
                        break;

                    case 4:
                        builder.Append(BitConverter.ToInt32(data, i).ToString());
                        break;

                    case 8:
                        builder.Append(BitConverter.ToInt64(data, i).ToString());
                        break;
                }

                if ((i / bytesPerElem) % elementsPerLine == elementsPerLine - 1)
                    builder.AppendLine();
                else
                    builder.Append("  ");
            }

            return builder.ToString();
        }

        private byte[] TextToRaw (string text)
        {
            return TextToRaw(text, _bytesPerElem);
        }

        private static byte[] TextToRaw (string text, int bytesPerElem)
        {
            string[] items = text.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
            byte[] data = new byte[bytesPerElem * items.Length];

            for (int i = 0; i < items.Length; i++) {
                int index = i * bytesPerElem;

                switch (bytesPerElem) {
                    case 1:
                        sbyte val1;
                        if (sbyte.TryParse(items[i], out val1))
                            data[index] = (byte)val1;
                        break;

                    case 2:
                        short val2;
                        if (short.TryParse(items[i], out val2)) {
                            byte[] buffer = BitConverter.GetBytes(val2);
                            Array.Copy(buffer, 0, data, index, 2);
                        }
                        break;

                    case 4:
                        int val4;
                        if (int.TryParse(items[i], out val4)) {
                            byte[] buffer = BitConverter.GetBytes(val4);
                            Array.Copy(buffer, 0, data, index, 4);
                        }
                        break;

                    case 8:
                        long val8;
                        if (long.TryParse(items[i], out val8)) {
                            byte[] buffer = BitConverter.GetBytes(val8);
                            Array.Copy(buffer, 0, data, index, 8);
                        }
                        break;
                }
            }

            return data;
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

        private void ImportText (string path)
        {
            try {
                using (FileStream fstr = File.OpenRead(path)) {
                    byte[] raw = new byte[fstr.Length];
                    fstr.Read(raw, 0, (int)fstr.Length);

                    string text = System.Text.Encoding.UTF8.GetString(raw, 0, raw.Length);
                    _data = TextToRaw(text);

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

        private void ExportText (string path)
        {
            try {
                using (FileStream fstr = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    string text = RawToText(_byteProvider.Bytes.ToArray());
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(text);
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
                ofd.Filter = "Binary Data|*|Text Data (*.txt)|*.txt";
                ofd.FilterIndex = 0;

                if (ofd.ShowDialog() == DialogResult.OK) {
                    if (Path.GetExtension(ofd.FileName) == ".txt")
                        ImportText(ofd.FileName);
                    else
                        ImportRaw(ofd.FileName);
                }
            }
        }

        private void _buttonExport_Click (object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog()) {
                sfd.RestoreDirectory = true;
                sfd.Filter = "Binary Data|*|Text Data (*.txt)|*.txt";
                sfd.FilterIndex = 0;
                sfd.OverwritePrompt = true;

                if (sfd.ShowDialog() == DialogResult.OK) {
                    if (Path.GetExtension(sfd.FileName) == ".txt")
                        ExportText(sfd.FileName);
                    else
                        ExportRaw(sfd.FileName);
                }
            }
        }
    }
}
