using Be.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public partial class HexEditor : Form
    {
        private readonly int _bytesPerElem;

        private readonly Dictionary<TabPage, EditView> _views = new Dictionary<TabPage, EditView>();

        private TabPage _previousPage;

        public HexEditor(string tagName, byte[] data, int bytesPerElem)
        {
            InitializeComponent();

            EditView textView = new TextView(statusStrip1, bytesPerElem);
            textView.Initialize();
            textView.SetRawData(data);
            textView.Modified += (s, e) => { Modified = true; };

            _views.Add(textView.TabPage, textView);
            viewTabs.TabPages.Add(textView.TabPage);

            EditView hexView = null;

            if (!IsMono())
            {
                hexView = new HexView(statusStrip1, bytesPerElem);
                hexView.Initialize();
                hexView.SetRawData(data);
                hexView.Modified += (s, e) => { Modified = true; };

                _views.Add(hexView.TabPage, hexView);
                viewTabs.TabPages.Add(hexView.TabPage);
            }

            if (bytesPerElem > 1 || IsMono())
            {
                textView.Activate();
                viewTabs.SelectedTab = textView.TabPage;
            }
            else
            {
                hexView.Activate();
                viewTabs.SelectedTab = hexView.TabPage;
            }

            viewTabs.Deselected += (o, e) => { _previousPage = e.TabPage; };
            viewTabs.Selecting += HandleTabChanged;

            Text = "Editing: " + tagName;

            _bytesPerElem = bytesPerElem;

            Data = new byte[data.Length];
            Array.Copy(data, Data, data.Length);
        }

        public byte[] Data { get; private set; }

        public bool Modified { get; private set; }

        private bool IsMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        private void HandleTabChanged(object sender, TabControlCancelEventArgs e)
        {
            if (e.Action != TabControlAction.Selecting)
                return;

            if (e.TabPage == _previousPage)
                return;

            var oldView = _views[_previousPage];
            var newView = _views[e.TabPage];

            var data = oldView.GetRawData();
            newView.SetRawData(data);

            newView.Activate();
        }

        private void Apply()
        {
            var view = _views[viewTabs.SelectedTab];
            Data = view.GetRawData();

            DialogResult = DialogResult.OK;
            Close();
        }

        private string RawToText(byte[] data)
        {
            return RawToText(data, _bytesPerElem);
        }

        private static string RawToText(byte[] data, int bytesPerElem)
        {
            switch (bytesPerElem)
            {
                case 1: return RawToText(data, bytesPerElem, 16);
                case 2: return RawToText(data, bytesPerElem, 8);
                case 4: return RawToText(data, bytesPerElem, 4);
                case 8: return RawToText(data, bytesPerElem, 2);
                default: return RawToText(data, bytesPerElem, 1);
            }
        }

        private static string RawToText(byte[] data, int bytesPerElem, int elementsPerLine)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < data.Length; i += bytesPerElem)
            {
                if (data.Length - i < bytesPerElem)
                    break;

                switch (bytesPerElem)
                {
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

                if (i / bytesPerElem % elementsPerLine == elementsPerLine - 1)
                    builder.AppendLine();
                else
                    builder.Append("  ");
            }

            return builder.ToString();
        }

        private byte[] TextToRaw(string text)
        {
            return TextToRaw(text, _bytesPerElem);
        }

        private static byte[] TextToRaw(string text, int bytesPerElem)
        {
            var items = text.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
            var data = new byte[bytesPerElem * items.Length];

            for (var i = 0; i < items.Length; i++)
            {
                var index = i * bytesPerElem;

                switch (bytesPerElem)
                {
                    case 1:
                        sbyte val1;
                        if (sbyte.TryParse(items[i], out val1))
                            data[index] = (byte)val1;
                        break;

                    case 2:
                        short val2;
                        if (short.TryParse(items[i], out val2))
                        {
                            var buffer = BitConverter.GetBytes(val2);
                            Array.Copy(buffer, 0, data, index, 2);
                        }
                        break;

                    case 4:
                        int val4;
                        if (int.TryParse(items[i], out val4))
                        {
                            var buffer = BitConverter.GetBytes(val4);
                            Array.Copy(buffer, 0, data, index, 4);
                        }
                        break;

                    case 8:
                        long val8;
                        if (long.TryParse(items[i], out val8))
                        {
                            var buffer = BitConverter.GetBytes(val8);
                            Array.Copy(buffer, 0, data, index, 8);
                        }
                        break;
                }
            }

            return data;
        }

        private void ImportRaw(string path)
        {
            try
            {
                using (var fstr = File.OpenRead(path))
                {
                    Data = new byte[fstr.Length];
                    fstr.Read(Data, 0, (int)fstr.Length);

                    var view = _views[viewTabs.SelectedTab];
                    view.SetRawData(Data);

                    Modified = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to import data from \"" + path + "\"\n\nException: " + e.Message);
            }
        }

        private void ImportText(string path)
        {
            try
            {
                using (var fstr = File.OpenRead(path))
                {
                    var raw = new byte[fstr.Length];
                    fstr.Read(raw, 0, (int)fstr.Length);

                    var text = Encoding.UTF8.GetString(raw, 0, raw.Length);
                    Data = TextToRaw(text);

                    var view = _views[viewTabs.SelectedTab];
                    view.SetRawData(Data);

                    Modified = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to import data from \"" + path + "\"\n\nException: " + e.Message);
            }
        }

        private void ExportRaw(string path)
        {
            try
            {
                using (var fstr = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var view = _views[viewTabs.SelectedTab];
                    var data = view.GetRawData();

                    fstr.Write(data, 0, data.Length);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to export data to \"" + path + "\"\n\nException: " + e.Message);
            }
        }

        private void ExportText(string path)
        {
            try
            {
                using (var fstr = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var view = _views[viewTabs.SelectedTab];
                    var text = RawToText(view.GetRawData());

                    var data = Encoding.UTF8.GetBytes(text);
                    fstr.Write(data, 0, data.Length);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to export data to \"" + path + "\"\n\nException: " + e.Message);
            }
        }

        private void _buttonOK_Click(object sender, EventArgs e)
        {
            Apply();
        }

        private void _buttonImport_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.RestoreDirectory = true;
                ofd.Multiselect = false;
                ofd.Filter = "Binary Data|*|Text Data (*.txt)|*.txt";
                ofd.FilterIndex = 0;

                if (ofd.ShowDialog() == DialogResult.OK)
                    if (Path.GetExtension(ofd.FileName) == ".txt")
                        ImportText(ofd.FileName);
                    else
                        ImportRaw(ofd.FileName);
            }
        }

        private void _buttonExport_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.RestoreDirectory = true;
                sfd.Filter = "Binary Data|*|Text Data (*.txt)|*.txt";
                sfd.FilterIndex = 0;
                sfd.OverwritePrompt = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                    if (Path.GetExtension(sfd.FileName) == ".txt")
                        ExportText(sfd.FileName);
                    else
                        ExportRaw(sfd.FileName);
            }
        }

        private abstract class EditView
        {
            protected EditView(StatusStrip statusBar, int bytesPerElem)
            {
                BytesPerElem = bytesPerElem;
                StatusBar = statusBar;
            }

            public int BytesPerElem { get; }
            public StatusStrip StatusBar { get; }

            public abstract TabPage TabPage { get; }

            public event EventHandler Modified;

            public abstract void Initialize();

            public abstract void Activate();

            public abstract byte[] GetRawData();

            public abstract void SetRawData(byte[] data);

            protected virtual void OnModified()
            {
                var ev = Modified;
                if (ev != null)
                    ev(this, EventArgs.Empty);
            }
        }

        private class TextView : EditView
        {
            private readonly Dictionary<int, int> _elemIndex = new Dictionary<int, int>();
            private ToolStripStatusLabel _elementLabel;
            private ToolStripStatusLabel _spaceLabel;
            private TabPage _tabPage;
            private TextBox _textBox;

            public TextView(StatusStrip statusBar, int bytesPerElem)
                : base(statusBar, bytesPerElem)
            {
            }

            public override TabPage TabPage => _tabPage;

            public override void Initialize()
            {
                _textBox = new TextBox
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(0, 0),
                    Margin = new Padding(0),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Size = new Size(500, 263),
                    TabIndex = 0,
                    MaxLength = 0
                };

                _tabPage = new TabPage
                {
                    Location = new Point(4, 22),
                    Padding = new Padding(3),
                    Size = new Size(500, 263),
                    TabIndex = 1,
                    Text = "Text View",
                    UseVisualStyleBackColor = true
                };

                _tabPage.Controls.Add(_textBox);

                _textBox.TextChanged += (s, e) =>
                {
                    OnModified();
                    RebuildElementIndex();
                };
                _textBox.PreviewKeyDown += (s, e) => { e.IsInputKey = true; };
                _textBox.KeyUp += (s, e) => { UpdateElementLabel(); };
                _textBox.MouseClick += (s, e) => { UpdateElementLabel(); };

                InitializeStatusBar();
            }

            private void InitializeStatusBar()
            {
                _elementLabel = new ToolStripStatusLabel
                {
                    Size = new Size(100, 17),
                    Text = "Element 0",
                    TextAlign = ContentAlignment.MiddleLeft
                };

                _spaceLabel = new ToolStripStatusLabel
                {
                    Spring = true
                };
            }

            public override void Activate()
            {
                StatusBar.Items.Clear();
                StatusBar.Items.AddRange(new ToolStripItem[]
                {
                    _elementLabel, _spaceLabel
                });
            }

            public override byte[] GetRawData()
            {
                return TextToRaw(_textBox.Text, BytesPerElem);
            }

            public override void SetRawData(byte[] data)
            {
                _textBox.Text = RawToText(data, BytesPerElem);
                RebuildElementIndex();
            }

            private void RebuildElementIndex()
            {
                _elemIndex.Clear();

                var element = 0;
                var text = _textBox.Text;
                var lcw = true;

                for (var i = 0; i < text.Length; i++)
                {
                    var w = IsWhiteSpace(text[i]);
                    if (lcw && !w)
                        _elemIndex[i] = element++;
                    lcw = w;
                }
            }

            private bool IsWhiteSpace(char c)
            {
                return c == ' ' || c == '\n' || c == '\r' || c == '\t';
            }

            private void UpdateElementLabel()
            {
                var index = _textBox.SelectionStart;
                var element = 0;

                while (index >= 0 && !_elemIndex.TryGetValue(index, out element))
                    index--;

                _elementLabel.Text = "Element " + element;
            }
        }

        private class HexView : EditView
        {
            private DynamicByteProvider _byteProvider;
            private ToolStripStatusLabel _elementLabel;
            private HexBox _hexBox;
            private ToolStripStatusLabel _insertLabel;

            private ToolStripStatusLabel _positionLabel;
            private ToolStripStatusLabel _spaceLabel;
            private TabPage _tabPage;

            public HexView(StatusStrip statusBar, int bytesPerElem)
                : base(statusBar, bytesPerElem)
            {
            }

            public override TabPage TabPage => _tabPage;

            public override void Initialize()
            {
                _byteProvider = new DynamicByteProvider(new byte[0]);
                _byteProvider.Changed += (o, e) => { OnModified(); };

                _hexBox = new HexBox
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    LineInfoForeColor = Color.Empty,
                    LineInfoVisible = true,
                    Location = new Point(0, 0),
                    ShadowSelectionColor = Color.FromArgb(100, 60, 188, 255),
                    Size = new Size(500, 263),
                    TabIndex = 0,
                    VScrollBarVisible = true,
                    ByteProvider = _byteProvider
                };

                _tabPage = new TabPage
                {
                    Location = new Point(4, 22),
                    Padding = new Padding(3),
                    Size = new Size(500, 263),
                    TabIndex = 0,
                    Text = "Hex View",
                    UseVisualStyleBackColor = true
                };

                _tabPage.Controls.Add(_hexBox);

                _hexBox.HorizontalByteCountChanged += (s, e) => { UpdatePosition(); };
                _hexBox.CurrentLineChanged += (s, e) => { UpdatePosition(); };
                _hexBox.CurrentPositionInLineChanged += (s, e) => { UpdatePosition(); };

                _hexBox.InsertActiveChanged += (s, e) =>
                {
                    _insertLabel.Text = _hexBox.InsertActive ? "Insert" : "Overwrite";
                };

                InitializeStatusBar();
            }

            private void InitializeStatusBar()
            {
                _positionLabel = new ToolStripStatusLabel
                {
                    AutoSize = false,
                    Size = new Size(100, 17),
                    Text = "0000"
                };

                _elementLabel = new ToolStripStatusLabel
                {
                    Size = new Size(59, 17),
                    Text = "Element 0",
                    TextAlign = ContentAlignment.MiddleLeft
                };

                _spaceLabel = new ToolStripStatusLabel
                {
                    Size = new Size(300, 17),
                    Spring = true
                };

                _insertLabel = new ToolStripStatusLabel
                {
                    Size = new Size(58, 17),
                    Text = _hexBox.InsertActive ? "Insert" : "Overwrite"
                };
            }

            public override void Activate()
            {
                StatusBar.Items.Clear();
                StatusBar.Items.AddRange(new ToolStripItem[]
                {
                    _positionLabel, _elementLabel, _spaceLabel, _insertLabel
                });

                UpdatePosition();
            }

            public override byte[] GetRawData()
            {
                var data = new byte[_byteProvider.Length];
                for (var i = 0; i < data.Length; i++) data[i] = _byteProvider.Bytes[i];

                return data;
            }

            public override void SetRawData(byte[] data)
            {
                _byteProvider = new DynamicByteProvider(data);
                _byteProvider.Changed += (o, e2) => { OnModified(); };

                _hexBox.ByteProvider = _byteProvider;
            }

            private void UpdatePosition()
            {
                var pos = (_hexBox.CurrentLine - 1) * _hexBox.HorizontalByteCount + _hexBox.CurrentPositionInLine - 1;

                _positionLabel.Text = "0x" + pos.ToString("X4");
                _elementLabel.Text = "Element " + pos / BytesPerElem;
            }
        }

        private class FixedByteProvider : DynamicByteProvider
        {
            public FixedByteProvider(byte[] data)
                : base(data)
            {
            }

            public override bool SupportsInsertBytes()
            {
                return false;
            }
        }
    }
}