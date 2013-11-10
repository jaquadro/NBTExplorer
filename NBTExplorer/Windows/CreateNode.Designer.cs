namespace NBTExplorer.Windows
{
    partial class CreateNodeForm
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
            this._sizeField = new System.Windows.Forms.TextBox();
            this._sizeFieldLabel = new System.Windows.Forms.Label();
            this._nameFieldLabel = new System.Windows.Forms.Label();
            this._nameField = new System.Windows.Forms.TextBox();
            this._buttonCancel = new System.Windows.Forms.Button();
            this._buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _sizeField
            // 
            this._sizeField.Location = new System.Drawing.Point(56, 26);
            this._sizeField.Name = "_sizeField";
            this._sizeField.Size = new System.Drawing.Size(67, 20);
            this._sizeField.TabIndex = 7;
            // 
            // _sizeFieldLabel
            // 
            this._sizeFieldLabel.AutoSize = true;
            this._sizeFieldLabel.Location = new System.Drawing.Point(12, 29);
            this._sizeFieldLabel.Name = "_sizeFieldLabel";
            this._sizeFieldLabel.Size = new System.Drawing.Size(30, 13);
            this._sizeFieldLabel.TabIndex = 6;
            this._sizeFieldLabel.Text = "Size:";
            // 
            // _nameFieldLabel
            // 
            this._nameFieldLabel.AutoSize = true;
            this._nameFieldLabel.Location = new System.Drawing.Point(12, 9);
            this._nameFieldLabel.Name = "_nameFieldLabel";
            this._nameFieldLabel.Size = new System.Drawing.Size(38, 13);
            this._nameFieldLabel.TabIndex = 5;
            this._nameFieldLabel.Text = "Name:";
            // 
            // _nameField
            // 
            this._nameField.Location = new System.Drawing.Point(56, 6);
            this._nameField.Name = "_nameField";
            this._nameField.Size = new System.Drawing.Size(209, 20);
            this._nameField.TabIndex = 4;
            // 
            // _buttonCancel
            // 
            this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._buttonCancel.Location = new System.Drawing.Point(109, 57);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(75, 23);
            this._buttonCancel.TabIndex = 9;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            // 
            // _buttonOK
            // 
            this._buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonOK.Location = new System.Drawing.Point(190, 57);
            this._buttonOK.Name = "_buttonOK";
            this._buttonOK.Size = new System.Drawing.Size(75, 23);
            this._buttonOK.TabIndex = 8;
            this._buttonOK.Text = "OK";
            this._buttonOK.UseVisualStyleBackColor = true;
            this._buttonOK.Click += new System.EventHandler(this._buttonOK_Click);
            // 
            // CreateNode
            // 
            this.AcceptButton = this._buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._buttonCancel;
            this.ClientSize = new System.Drawing.Size(277, 92);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonOK);
            this.Controls.Add(this._sizeField);
            this.Controls.Add(this._sizeFieldLabel);
            this.Controls.Add(this._nameFieldLabel);
            this.Controls.Add(this._nameField);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateNode";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Create Tag...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _sizeField;
        private System.Windows.Forms.Label _sizeFieldLabel;
        private System.Windows.Forms.Label _nameFieldLabel;
        private System.Windows.Forms.TextBox _nameField;
        private System.Windows.Forms.Button _buttonCancel;
        private System.Windows.Forms.Button _buttonOK;
    }
}