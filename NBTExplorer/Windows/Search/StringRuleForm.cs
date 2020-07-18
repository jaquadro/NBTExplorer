using NBTExplorer.Model.Search;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NBTExplorer.Windows.Search
{
    public partial class StringRuleForm : Form
    {
        public StringRuleForm(Dictionary<StringOperator, string> operators)
        {
            InitializeComponent();

            foreach (var op in operators)
                _selectOperator.Items.Add(op.Key);

            _selectOperator.SelectedIndex = 0;
        }

        public string RuleGroupName
        {
            get => _ruleGroup.Text;
            set => _ruleGroup.Text = value;
        }

        public string TagName
        {
            get => _textName.Text;
            set => _textName.Text = value;
        }

        public string TagValue
        {
            get => _textValue.Text;
            set => _textValue.Text = value;
        }

        public StringOperator Operator
        {
            get => (StringOperator)_selectOperator.SelectedItem;
            set => _selectOperator.SelectedItem = value;
        }

        private void _buttonOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TagName))
            {
                MessageBox.Show(this, "Rule missing name");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}