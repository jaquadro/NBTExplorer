using System;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public partial class Find : Form
    {
        public Find()
        {
            InitializeComponent();
        }

        public bool MatchName => _cbName.Checked;

        public bool MatchValue => _cbValue.Checked;

        public string NameToken => _textName.Text;

        public string ValueToken => _textValue.Text;

        private void _buttonFind_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void _buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void _textName_TextChanged(object sender, EventArgs e)
        {
            _cbName.Checked = true;
        }

        private void _textValue_TextChanged(object sender, EventArgs e)
        {
            _cbValue.Checked = true;
        }
    }
}