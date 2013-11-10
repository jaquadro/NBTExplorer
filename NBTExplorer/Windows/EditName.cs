using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public partial class EditName : Form
    {
        private string _originalName;
        private string _name;

        private List<string> _invalidNames = new List<string>();

        public EditName (String name)
        {
            InitializeComponent();

            _originalName = name;
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

        public bool AllowEmpty { get; set; }

        public bool IsModified
        {
            get { return _name != _originalName; }
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
            if (String.IsNullOrEmpty(text) && !AllowEmpty) {
                MessageBox.Show("You must provide a nonempty name.");
                return false;
            }

            if (text != _originalName && _invalidNames.Contains(text)) {
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
