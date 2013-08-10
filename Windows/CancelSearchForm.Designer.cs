namespace NBTExplorer.Windows
{
    partial class CancelSearchForm
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
            this._buttonCancel = new System.Windows.Forms.Button();
            this._searchPathLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _buttonCancel
            // 
            this._buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._buttonCancel.Location = new System.Drawing.Point(232, 45);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(75, 23);
            this._buttonCancel.TabIndex = 0;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            // 
            // _searchPathLabel
            // 
            this._searchPathLabel.Location = new System.Drawing.Point(12, 19);
            this._searchPathLabel.Name = "_searchPathLabel";
            this._searchPathLabel.Size = new System.Drawing.Size(514, 23);
            this._searchPathLabel.TabIndex = 1;
            this._searchPathLabel.Text = "...";
            // 
            // CancelSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._buttonCancel;
            this.ClientSize = new System.Drawing.Size(538, 84);
            this.Controls.Add(this._searchPathLabel);
            this.Controls.Add(this._buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CancelSearchForm";
            this.Text = "Searching...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _buttonCancel;
        private System.Windows.Forms.Label _searchPathLabel;
    }
}