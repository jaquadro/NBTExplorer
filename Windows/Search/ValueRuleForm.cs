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
    public partial class ValueRuleForm : Form
    {
        public ValueRuleForm (Dictionary<NumericOperator, string> operators)
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

        public long TagValueAsLong
        {
            get
            {
                long lvalue;
                if (long.TryParse(TagValue, out lvalue))
                    return lvalue;

                double fvalue;
                if (double.TryParse(TagValue, out fvalue))
                    return (long)fvalue;

                return 0;
            }
        }

        public double TagValueAsDouble
        {
            get
            {
                double fvalue;
                if (double.TryParse(TagValue, out fvalue))
                    return fvalue;

                return 0;
            }
        }

        public NumericOperator Operator
        {
            get { return (NumericOperator)_selectOperator.SelectedItem; }
            set { _selectOperator.SelectedItem = value; }
        }

        private void _buttonOK_Click (object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TagName) || !TryParseValue()) {
                MessageBox.Show(this, "Rule missing name or value");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool TryParseValue ()
        {
            if (Operator == NumericOperator.Any)
                return true;

            long lvalue;
            double fvalue;
            if (long.TryParse(TagValue, out lvalue) || double.TryParse(TagValue, out fvalue))
                return true;

            return false;
        }
    }
}
