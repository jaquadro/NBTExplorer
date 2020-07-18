using Substrate.Nbt;
using System;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public partial class EditValue : Form
    {
        public EditValue(TagNode tag)
        {
            InitializeComponent();

            NodeTag = tag;

            if (tag == null)
            {
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }

            textBox1.Text = NodeTag.ToString();
        }

        public TagNode NodeTag { get; }

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
            return ValidateValueInput();
        }

        private bool ValidateValueInput()
        {
            try
            {
                switch (NodeTag.GetTagType())
                {
                    case TagType.TAG_BYTE:
                        NodeTag.ToTagByte().Data = unchecked((byte)sbyte.Parse(textBox1.Text));
                        break;

                    case TagType.TAG_SHORT:
                        NodeTag.ToTagShort().Data = short.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_INT:
                        NodeTag.ToTagInt().Data = int.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_LONG:
                        NodeTag.ToTagLong().Data = long.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_FLOAT:
                        NodeTag.ToTagFloat().Data = float.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_DOUBLE:
                        NodeTag.ToTagDouble().Data = double.Parse(textBox1.Text);
                        break;

                    case TagType.TAG_STRING:
                        NodeTag.ToTagString().Data = textBox1.Text;
                        break;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("The value is formatted incorrectly for the given type.");
                return false;
            }
            catch (OverflowException)
            {
                MessageBox.Show("The value is outside the acceptable range for the given type.");
                return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void _buttonOK_Click(object sender, EventArgs e)
        {
            Apply();
        }
    }
}