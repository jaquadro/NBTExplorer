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
    public enum EditValueType
    {
        Name,
        Value,
    }

    public partial class EditValue : Form
    {
        private string _name;
        private TagNode _tag;
        private EditValueType _type;

        private List<string> _invalidNames = new List<string>();

        public EditValue (TagNode tag)
        {
            InitializeComponent();

            _type = EditValueType.Value;
            _tag = tag;

            if (tag == null) {
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }

            SetTitle();

            textBox1.Text = _tag.ToString();
        }

        public EditValue (string name)
        {
            InitializeComponent();

            _type = EditValueType.Name;
            _name = name ?? "";

            SetTitle();

            textBox1.Text = _name.ToString();
        }

        public string NodeName
        {
            get { return _name; }
        }

        public TagNode NodeTag
        {
            get { return _tag; }
        }

        public List<string> InvalidNames
        {
            get { return _invalidNames; }
        }

        private void SetTitle ()
        {
            switch (_type) {
                case EditValueType.Name:
                    base.Text = "Edit Name...";
                    break;

                case EditValueType.Value:
                    base.Text = "Edit Value...";
                    break;
            }
        }

        private bool ValidateInput ()
        {
            switch (_type) {
                case EditValueType.Name:
                    return ValidateNameInput();
                case EditValueType.Value:
                    return ValidateValueInput();
            }

            return false;
        }

        private bool ValidateNameInput ()
        {
            string text = textBox1.Text.Trim();
            if (String.IsNullOrEmpty(text)) {
                MessageBox.Show("You must provide a nonempty name.");
                return false;
            }

            if (_invalidNames.Contains(text)) {
                MessageBox.Show("Duplicate name provided.");
                return false;
            }

            _name = textBox1.Text.Trim();
            return true;
        }

        private bool ValidateValueInput ()
        {
            try {
                switch (_tag.GetTagType()) {
                    case TagType.TAG_BYTE:
                        _tag.ToTagByte().Data = byte.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_SHORT:
                        _tag.ToTagShort().Data = short.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_INT:
                        _tag.ToTagInt().Data = int.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_LONG:
                        _tag.ToTagLong().Data = long.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_FLOAT:
                        _tag.ToTagFloat().Data = float.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_DOUBLE:
                        _tag.ToTagDouble().Data = double.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_STRING:
                        _tag.ToTagString().Data = textBox1.Text;
                        break;
                }
            }
            catch (FormatException) {
                MessageBox.Show("The value is formatted incorrectly for the given type.");
                return false;
            }
            catch (OverflowException) {
                MessageBox.Show("The value is outside the acceptable range for the given type.");
                return false;
            }
            catch {
                return false;
            }

            return true;
        }

        protected override void OnKeyDown (KeyEventArgs e)
        {
            switch (e.KeyCode) {
                case Keys.Escape:
                    DialogResult = DialogResult.Cancel;
                    Close();
                    return;

                case Keys.Enter:
                    if (ValidateInput()) {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    return;
            }

            base.OnKeyDown(e);
        }
    }
}
