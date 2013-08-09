using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NBTExplorer.Model.Search;

namespace NBTExplorer.Windows.Search
{
    public partial class StringRuleForm : Form
    {
        public StringRuleForm (Dictionary<StringOperator, string> operators)
        {
            InitializeComponent();

            foreach (var op in operators)
                _selectOperator.Items.Add(op.Key);

            _selectOperator.SelectedIndex = 0;
        }

        public string RuleGroupName
        {
            get { return _ruleGroup.Text; }
            set { _ruleGroup.Text = value; }
        }

        public string TagName
        {
            get { return _textName.Text; }
            set { _textName.Text = value; }
        }

        public string TagValue
        {
            get { return _textValue.Text; }
            set { _textValue.Text = value; }
        }

        public StringOperator Operator
        {
            get { return (StringOperator)_selectOperator.SelectedItem; }
            set { _selectOperator.SelectedItem = value; }
        }

        private void _buttonOK_Click (object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TagName)) {
                MessageBox.Show(this, "Rule missing name");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
