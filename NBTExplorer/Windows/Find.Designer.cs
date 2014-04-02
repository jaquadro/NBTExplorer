namespace NBTExplorer.Windows
{
    partial class Find
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
            this._cbName = new System.Windows.Forms.CheckBox();
            this._cbValue = new System.Windows.Forms.CheckBox();
            this._textName = new System.Windows.Forms.TextBox();
            this._textValue = new System.Windows.Forms.TextBox();
            this._buttonFind = new System.Windows.Forms.Button();
            this._buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _cbName
            // 
            this._cbName.AutoSize = true;
            this._cbName.Location = new System.Drawing.Point(13, 13);
            this._cbName.Name = "_cbName";
            this._cbName.Size = new System.Drawing.Size(57, 17);
            this._cbName.TabIndex = 0;
            this._cbName.Text = "Name:";
            this._cbName.UseVisualStyleBackColor = true;
            // 
            // _cbValue
            // 
            this._cbValue.AutoSize = true;
            this._cbValue.Location = new System.Drawing.Point(13, 37);
            this._cbValue.Name = "_cbValue";
            this._cbValue.Size = new System.Drawing.Size(56, 17);
            this._cbValue.TabIndex = 1;
            this._cbValue.Text = "Value:";
            this._cbValue.UseVisualStyleBackColor = true;
            // 
            // _textName
            // 
            this._textName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textName.Location = new System.Drawing.Point(76, 12);
            this._textName.Name = "_textName";
            this._textName.Size = new System.Drawing.Size(273, 20);
            this._textName.TabIndex = 2;
            this._textName.TextChanged += new System.EventHandler(this._textName_TextChanged);
            // 
            // _textValue
            // 
            this._textValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textValue.Location = new System.Drawing.Point(76, 34);
            this._textValue.Name = "_textValue";
            this._textValue.Size = new System.Drawing.Size(273, 20);
            this._textValue.TabIndex = 3;
            this._textValue.TextChanged += new System.EventHandler(this._textValue_TextChanged);
            // 
            // _buttonFind
            // 
            this._buttonFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonFind.Location = new System.Drawing.Point(274, 65);
            this._buttonFind.Name = "_buttonFind";
            this._buttonFind.Size = new System.Drawing.Size(75, 23);
            this._buttonFind.TabIndex = 4;
            this._buttonFind.Text = "Find";
            this._buttonFind.UseVisualStyleBackColor = true;
            this._buttonFind.Click += new System.EventHandler(this._buttonFind_Click);
            // 
            // _buttonCancel
            // 
            this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._buttonCancel.Location = new System.Drawing.Point(193, 65);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(75, 23);
            this._buttonCancel.TabIndex = 5;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            this._buttonCancel.Click += new System.EventHandler(this._buttonCancel_Click);
            // 
            // Find
            // 
            this.AcceptButton = this._buttonFind;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._buttonCancel;
            this.ClientSize = new System.Drawing.Size(361, 100);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonFind);
            this.Controls.Add(this._textValue);
            this.Controls.Add(this._textName);
            this.Controls.Add(this._cbValue);
            this.Controls.Add(this._cbName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Find";
            this.Text = "Find";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _cbName;
        private System.Windows.Forms.CheckBox _cbValue;
        private System.Windows.Forms.TextBox _textName;
        private System.Windows.Forms.TextBox _textValue;
        private System.Windows.Forms.Button _buttonFind;
        private System.Windows.Forms.Button _buttonCancel;
    }
}