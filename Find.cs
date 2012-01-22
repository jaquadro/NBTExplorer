using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NBTExplorer
{
    public partial class Find : Form
    {
        public Find ()
        {
            InitializeComponent();
        }

        public bool MatchName
        {
            get { return _cbName.Checked; }
        }

        public bool MatchValue
        {
            get { return _cbValue.Checked; }
        }

        public string NameToken
        {
            get { return _textName.Text; }
        }

        public string ValueToken
        {
            get { return _textValue.Text; }
        }

        private void _buttonFind_Click (object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void _buttonCancel_Click (object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
