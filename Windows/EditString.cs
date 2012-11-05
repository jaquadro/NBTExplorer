using System;
using System.Windows.Forms;

namespace NBTExplorer.Windows
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
