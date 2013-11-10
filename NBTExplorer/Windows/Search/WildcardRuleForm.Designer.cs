namespace NBTExplorer.Windows.Search
{
    partial class WildcardRuleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.label1 = new System.Windows.Forms.Label();
            this._textName = new System.Windows.Forms.TextBox();
            this._textValue = new System.Windows.Forms.TextBox();
            this._selectOperator = new System.Windows.Forms.ComboBox();
            this._ruleGroup = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this._buttonOK = new System.Windows.Forms.Button();
            this._buttonCancel = new System.Windows.Forms.Button();
            this._ruleGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tag Name:";
            // 
            // _textName
            // 
            this._textName.Location = new System.Drawing.Point(102, 19);
            this._textName.Name = "_textName";
            this._textName.Size = new System.Drawing.Size(142, 20);
            this._textName.TabIndex = 1;
            // 
            // _textValue
            // 
            this._textValue.Location = new System.Drawing.Point(102, 72);
            this._textValue.Name = "_textValue";
            this._textValue.Size = new System.Drawing.Size(142, 20);
            this._textValue.TabIndex = 2;
            // 
            // _selectOperator
            // 
            this._selectOperator.FormattingEnabled = true;
            this._selectOperator.Location = new System.Drawing.Point(102, 45);
            this._selectOperator.Name = "_selectOperator";
            this._selectOperator.Size = new System.Drawing.Size(142, 21);
            this._selectOperator.TabIndex = 3;
            // 
            // _ruleGroup
            // 
            this._ruleGroup.Controls.Add(this.label2);
            this._ruleGroup.Controls.Add(this._textName);
            this._ruleGroup.Controls.Add(this._selectOperator);
            this._ruleGroup.Controls.Add(this.label1);
            this._ruleGroup.Controls.Add(this._textValue);
            this._ruleGroup.Location = new System.Drawing.Point(12, 12);
            this._ruleGroup.Name = "_ruleGroup";
            this._ruleGroup.Size = new System.Drawing.Size(250, 98);
            this._ruleGroup.TabIndex = 4;
            this._ruleGroup.TabStop = false;
            this._ruleGroup.Text = "Rule";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Tag Value:";
            // 
            // _buttonOK
            // 
            this._buttonOK.Location = new System.Drawing.Point(186, 116);
            this._buttonOK.Name = "_buttonOK";
            this._buttonOK.Size = new System.Drawing.Size(75, 23);
            this._buttonOK.TabIndex = 5;
            this._buttonOK.Text = "OK";
            this._buttonOK.UseVisualStyleBackColor = true;
            this._buttonOK.Click += new System.EventHandler(this._buttonOK_Click);
            // 
            // _buttonCancel
            // 
            this._buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._buttonCancel.Location = new System.Drawing.Point(105, 116);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(75, 23);
            this._buttonCancel.TabIndex = 6;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            // 
            // ValueRuleForm
            // 
            this.AcceptButton = this._buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._buttonCancel;
            this.ClientSize = new System.Drawing.Size(273, 151);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonOK);
            this.Controls.Add(this._ruleGroup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ValueRuleForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Edit Value Rule";
            this.TopMost = true;
            this._ruleGroup.ResumeLayout(false);
            this._ruleGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _textName;
        private System.Windows.Forms.TextBox _textValue;
        private System.Windows.Forms.ComboBox _selectOperator;
        private System.Windows.Forms.GroupBox _ruleGroup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button _buttonOK;
        private System.Windows.Forms.Button _buttonCancel;
    }
}