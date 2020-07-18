using System;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public partial class EditString : Form
    {
        public EditString(string stringVal)
        {
            InitializeComponent();

            StringValue = stringVal;

            _stringField.Text = StringValue;
        }

        public string StringValue { get; private set; }

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
            return ValidateStringInput();
        }

        private bool ValidateStringInput()
        {
            StringValue = _stringField.Text.Trim();
            return true;
        }

        private void _buttonOK_Click(object sender, EventArgs e)
        {
            Apply();
        }
    }
}