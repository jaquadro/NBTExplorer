using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Substrate.Nbt;

namespace NBTExplorer
{
    public partial class EditName : Form
    {
        private string _name;

        private List<string> _invalidNames = new List<string>();

        public EditName (String name)
        {
            InitializeComponent();

            _name = name;

            _nameField.Text = _name;
        }

        public String TagName
        {
            get { return _name; }
        }

        public List<string> InvalidNames
        {
            get { return _invalidNames; }
        }

        private void Apply ()
        {
            if (ValidateInput()) {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool ValidateInput ()
        {
            return ValidateNameInput();
        }

        private bool ValidateNameInput ()
        {
            string text = _nameField.Text.Trim();
            if (String.IsNullOrEmpty(text)) {
                MessageBox.Show("You must provide a nonempty name.");
                return false;
            }

            if (_invalidNames.Contains(text)) {
                MessageBox.Show("Duplicate name provided.");
                return false;
            }

            _name = _nameField.Text.Trim();
            return true;
        }

        private void _buttonOK_Click (object sender, EventArgs e)
        {
            Apply();
        }
    }
}
