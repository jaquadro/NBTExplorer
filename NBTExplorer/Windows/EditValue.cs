using System;
using System.Windows.Forms;
using Substrate.Nbt;

namespace NBTExplorer.Windows
{
    public partial class EditValue : Form
    {
        private TagNode _tag;

        public EditValue (TagNode tag)
        {
            InitializeComponent();

            _tag = tag;

            if (tag == null) {
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }

            textBox1.Text = _tag.ToString();
        }

        public TagNode NodeTag
        {
            get { return _tag; }
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
            return ValidateValueInput();
        }

        private bool ValidateValueInput ()
        {
            try {
                switch (_tag.GetTagType()) {
                    case TagType.TAG_BYTE:
                        _tag.ToTagByte().Data = unchecked((byte)sbyte.Parse(textBox1.Text));
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

        private void _buttonOK_Click (object sender, EventArgs e)
        {
            Apply();
        }
    }
}
