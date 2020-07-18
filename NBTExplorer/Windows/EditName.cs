using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public partial class EditName : Form
    {
        private readonly string _originalName;

        public EditName(string name)
        {
            InitializeComponent();

            _originalName = name;
            TagName = name;

            _nameField.Text = TagName;
        }

        public string TagName { get; private set; }

        public List<string> InvalidNames { get; } = new List<string>();

        public bool AllowEmpty { get; set; }

        public bool IsModified => TagName != _originalName;

        private void Apply()
        {
            if (ValidateInput())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool ValidateInput()
        {
            return ValidateNameInput();
        }

        private bool ValidateNameInput()
        {
            var text = _nameField.Text.Trim();
            if (string.IsNullOrEmpty(text) && !AllowEmpty)
            {
                MessageBox.Show("You must provide a nonempty name.");
                return false;
            }

            if (text != _originalName && InvalidNames.Contains(text))
            {
                MessageBox.Show("Duplicate name provided.");
                return false;
            }

            TagName = _nameField.Text.Trim();
            return true;
        }

        private void _buttonOK_Click(object sender, EventArgs e)
        {
            Apply();
        }
    }
}