namespace NBTExplorer.Windows
{
    partial class HexEditor
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            //this._curPositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            //this._curElementLabel = new System.Windows.Forms.ToolStripStatusLabel();
            //this._space = new System.Windows.Forms.ToolStripStatusLabel();
            //this._insertStateLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._buttonCancel = new System.Windows.Forms.Button();
            this._buttonOK = new System.Windows.Forms.Button();
            //this.hexBox1 = new Be.Windows.Forms.HexBox();
            this._buttonImport = new System.Windows.Forms.Button();
            this._buttonExport = new System.Windows.Forms.Button();
            this.viewTabs = new System.Windows.Forms.TabControl();
            //this.textView = new System.Windows.Forms.TabPage();
            //this.textBox1 = new System.Windows.Forms.TextBox();
            //this.hexView = new System.Windows.Forms.TabPage();
            this.statusStrip1.SuspendLayout();
            this.viewTabs.SuspendLayout();
            //this.textView.SuspendLayout();
            //this.hexView.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            /*this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._curPositionLabel,
            this._curElementLabel,
            this._space,
            this._insertStateLabel});*/
            this.statusStrip1.Location = new System.Drawing.Point(0, 333);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(532, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // _curPositionLabel
            // 
            /*this._curPositionLabel.AutoSize = false;
            this._curPositionLabel.Name = "_curPositionLabel";
            this._curPositionLabel.Size = new System.Drawing.Size(100, 17);
            this._curPositionLabel.Text = "0000";
            // 
            // _curElementLabel
            // 
            this._curElementLabel.Name = "_curElementLabel";
            this._curElementLabel.Size = new System.Drawing.Size(59, 17);
            this._curElementLabel.Text = "Element 0";
            this._curElementLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _space
            // 
            this._space.Name = "_space";
            this._space.Size = new System.Drawing.Size(300, 17);
            this._space.Spring = true;
            // 
            // _insertStateLabel
            // 
            this._insertStateLabel.Name = "_insertStateLabel";
            this._insertStateLabel.Size = new System.Drawing.Size(58, 17);
            this._insertStateLabel.Text = "Overwrite";*/
            // 
            // _buttonCancel
            // 
            this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._buttonCancel.Location = new System.Drawing.Point(364, 307);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(75, 23);
            this._buttonCancel.TabIndex = 13;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            // 
            // _buttonOK
            // 
            this._buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonOK.Location = new System.Drawing.Point(445, 307);
            this._buttonOK.Name = "_buttonOK";
            this._buttonOK.Size = new System.Drawing.Size(75, 23);
            this._buttonOK.TabIndex = 12;
            this._buttonOK.Text = "OK";
            this._buttonOK.UseVisualStyleBackColor = true;
            this._buttonOK.Click += new System.EventHandler(this._buttonOK_Click);
            // 
            // hexBox1
            // 
            /*this.hexBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox1.LineInfoForeColor = System.Drawing.Color.Empty;
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Location = new System.Drawing.Point(0, 0);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ReadOnly = true;
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(500, 263);
            this.hexBox1.TabIndex = 0;
            this.hexBox1.VScrollBarVisible = true;*/
            // 
            // _buttonImport
            // 
            this._buttonImport.Location = new System.Drawing.Point(12, 307);
            this._buttonImport.Name = "_buttonImport";
            this._buttonImport.Size = new System.Drawing.Size(75, 23);
            this._buttonImport.TabIndex = 14;
            this._buttonImport.Text = "Import";
            this._buttonImport.UseVisualStyleBackColor = true;
            this._buttonImport.Click += new System.EventHandler(this._buttonImport_Click);
            // 
            // _buttonExport
            // 
            this._buttonExport.Location = new System.Drawing.Point(93, 307);
            this._buttonExport.Name = "_buttonExport";
            this._buttonExport.Size = new System.Drawing.Size(75, 23);
            this._buttonExport.TabIndex = 15;
            this._buttonExport.Text = "Export";
            this._buttonExport.UseVisualStyleBackColor = true;
            this._buttonExport.Click += new System.EventHandler(this._buttonExport_Click);
            // 
            // viewTabs
            // 
            //this.viewTabs.Controls.Add(this.textView);
            //this.viewTabs.Controls.Add(this.hexView);
            this.viewTabs.Location = new System.Drawing.Point(12, 12);
            this.viewTabs.Name = "viewTabs";
            this.viewTabs.SelectedIndex = 0;
            this.viewTabs.Size = new System.Drawing.Size(508, 289);
            this.viewTabs.TabIndex = 16;
            // 
            // textView
            // 
            /*this.textView.Controls.Add(this.textBox1);
            this.textView.Location = new System.Drawing.Point(4, 22);
            this.textView.Name = "textView";
            this.textView.Padding = new System.Windows.Forms.Padding(3);
            this.textView.Size = new System.Drawing.Size(500, 263);
            this.textView.TabIndex = 1;
            this.textView.Text = "Text View";
            this.textView.UseVisualStyleBackColor = true;*/
            // 
            // textBox1
            // 
            /*this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(500, 263);
            this.textBox1.TabIndex = 0;*/
            // 
            // hexView
            // 
            /*this.hexView.Controls.Add(this.hexBox1);
            this.hexView.Location = new System.Drawing.Point(4, 22);
            this.hexView.Name = "hexView";
            this.hexView.Padding = new System.Windows.Forms.Padding(3);
            this.hexView.Size = new System.Drawing.Size(500, 263);
            this.hexView.TabIndex = 0;
            this.hexView.Text = "Hex View";
            this.hexView.UseVisualStyleBackColor = true;*/
            // 
            // HexEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._buttonCancel;
            this.ClientSize = new System.Drawing.Size(532, 355);
            this.Controls.Add(this.viewTabs);
            this.Controls.Add(this._buttonExport);
            this.Controls.Add(this._buttonImport);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonOK);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "HexEditor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "HexEditor";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.viewTabs.ResumeLayout(false);
            //this.textView.ResumeLayout(false);
            //this.textView.PerformLayout();
            //this.hexView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private Be.Windows.Forms.HexBox hexBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        //private System.Windows.Forms.ToolStripStatusLabel _curPositionLabel;
        private System.Windows.Forms.Button _buttonCancel;
        private System.Windows.Forms.Button _buttonOK;
        //private System.Windows.Forms.ToolStripStatusLabel _curElementLabel;
        //private System.Windows.Forms.ToolStripStatusLabel _space;
        //private System.Windows.Forms.ToolStripStatusLabel _insertStateLabel;
        private System.Windows.Forms.Button _buttonImport;
        private System.Windows.Forms.Button _buttonExport;
        private System.Windows.Forms.TabControl viewTabs;
        //private System.Windows.Forms.TabPage textView;
        //private System.Windows.Forms.TextBox textBox1;
        //private System.Windows.Forms.TabPage hexView;
    }
}