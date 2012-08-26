using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Substrate.Nbt;

namespace NBTExplorer
{
    public partial class EditString : Form
    {
        private string _string;

        public EditString (string stringVal)
        {
            InitializeComponent();

            _string = stringVal;

            _stringField.Text = _string;
        }

        public string StringValue
        {
            get { return _string; }
        }

        private void Apply ()
        {
            if (ValidateInput()) {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
        }

        private bool ValidateInput ()
        {
            return ValidateStringInput();
        }

        private bool ValidateStringInput ()
        {
            _string = _stringField.Text.Trim();
            return true;
        }

        private void _buttonOK_Click (object sender, EventArgs e)
        {
            Apply();
        }
    }
}
