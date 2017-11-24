using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Substrate.Nbt;

namespace NBTExplorer.Windows
{
    public partial class CreateNodeForm : Form
    {
        private string _name;
        private int _size;
        private TagType _type;
        private TagNode _tag;

        private bool _hasName;

        private List<string> _invalidNames = new List<string>();

        public CreateNodeForm (TagType tagType)
            : this(tagType, true)
        { }

        public CreateNodeForm (TagType tagType, bool hasName)
        {
            InitializeComponent();

            _type = tagType;
            _hasName = hasName;

            SetNameBoxState();
            SetSizeBoxState();
        }

        private void SetNameBoxState ()
        {
            if (HasName) {
                _nameFieldLabel.Enabled = true;
                _nameField.Enabled = true;
            }
            else {
                _nameFieldLabel.Enabled = false;
                _nameField.Enabled = false;
            }
        }

        private void SetSizeBoxState ()
        {
            if (IsTagSizedType) {
                _sizeFieldLabel.Enabled = true;
                _sizeField.Enabled = true;
            }
            else {
                _sizeFieldLabel.Enabled = false;
                _sizeField.Enabled = false;
            }
        }

        public string TagName
        {
            get { return _name; }
        }

        public TagNode TagNode
        {
            get { return _tag; }
        }

        public List<string> InvalidNames
        {
            get { return _invalidNames; }
        }

        public bool HasName
        {
            get { return _hasName; }
        }

        private void Apply ()
        {
            if (ValidateInput()) {
                _tag = CreateTag();

                DialogResult = DialogResult.OK;
                Close();
                return;
            }
        }

        private TagNode CreateTag ()
        {
            switch (_type) {
                case TagType.TAG_BYTE:
                    return new TagNodeByte();
                case TagType.TAG_BYTE_ARRAY:
                    return new TagNodeByteArray(new byte[_size]);
                case TagType.TAG_COMPOUND:
                    return new TagNodeCompound();
                case TagType.TAG_DOUBLE:
                    return new TagNodeDouble();
                case TagType.TAG_FLOAT:
                    return new TagNodeFloat();
                case TagType.TAG_INT:
                    return new TagNodeInt();
                case TagType.TAG_INT_ARRAY:
                    return new TagNodeIntArray(new int[_size]);
                case TagType.TAG_LIST:
                    return new TagNodeList(TagType.TAG_BYTE);
                case TagType.TAG_LONG:
                    return new TagNodeLong();
                case TagType.TAG_LONG_ARRAY:
                    return new TagNodeLongArray(new long[_size]);
                case TagType.TAG_SHORT:
                    return new TagNodeShort();
                case TagType.TAG_STRING:
                    return new TagNodeString();
                default:
                    return new TagNodeByte();
            }
        }

        private bool ValidateInput ()
        {
            return ValidateNameInput()
                && ValidateSizeInput();
        }

        private bool ValidateNameInput ()
        {
            if (!HasName)
                return true;

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

        private bool ValidateSizeInput ()
        {
            if (!IsTagSizedType)
                return true;

            if (!Int32.TryParse(_sizeField.Text.Trim(), out _size)) {
                MessageBox.Show("Size must be a valid integer value.");
                return false;
            }

            _size = Math.Max(0, _size);
            return true;
        }

        private bool IsTagSizedType
        {
            get
            {
                switch (_type) {
                    case TagType.TAG_BYTE_ARRAY:
                    case TagType.TAG_INT_ARRAY:
                    case TagType.TAG_SHORT_ARRAY:
                    case TagType.TAG_LONG_ARRAY:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private void _buttonOK_Click (object sender, EventArgs e)
        {
            Apply();
        }
    }
}
