namespace NBTExplorer.Windows
{
    partial class FindBlock
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this._findButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this._cancelButton = new System.Windows.Forms.Button();
            this._blockZTextBox = new NBTExplorer.Windows.WatermarkTextBox();
            this._blockXTextBox = new NBTExplorer.Windows.WatermarkTextBox();
            this._chunkZTextBox = new NBTExplorer.Windows.WatermarkTextBox();
            this._chunkXTextBox = new NBTExplorer.Windows.WatermarkTextBox();
            this._regionZTextBox = new NBTExplorer.Windows.WatermarkTextBox();
            this._regionXTextBox = new NBTExplorer.Windows.WatermarkTextBox();
            this._localChunkXTextBox = new NBTExplorer.Windows.WatermarkTextBox();
            this._localChunkZTextBox = new NBTExplorer.Windows.WatermarkTextBox();
            this._localBlockZTextBox = new NBTExplorer.Windows.WatermarkTextBox();
            this._localBlockXTextBox = new NBTExplorer.Windows.WatermarkTextBox();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this._regionZTextBox);
            this.groupBox1.Controls.Add(this._regionXTextBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(142, 70);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Region";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox5, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(446, 152);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this._chunkZTextBox);
            this.groupBox2.Controls.Add(this._chunkXTextBox);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(151, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(142, 70);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Chunk";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this._blockZTextBox);
            this.groupBox3.Controls.Add(this._blockXTextBox);
            this.groupBox3.Location = new System.Drawing.Point(299, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(144, 70);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Block";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Z:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Z:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "X:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Z:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "X:";
            // 
            // _findButton
            // 
            this._findButton.Location = new System.Drawing.Point(383, 170);
            this._findButton.Name = "_findButton";
            this._findButton.Size = new System.Drawing.Size(75, 23);
            this._findButton.TabIndex = 2;
            this._findButton.Text = "Find Chunk";
            this._findButton.UseVisualStyleBackColor = true;
            this._findButton.Click += new System.EventHandler(this._findButton_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this._localChunkXTextBox);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this._localChunkZTextBox);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(151, 79);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(142, 70);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Local Chunk";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this._localBlockZTextBox);
            this.groupBox5.Controls.Add(this._localBlockXTextBox);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(299, 79);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(144, 70);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Local Block";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Z:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "X:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Z:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "X:";
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(302, 171);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _blockZTextBox
            // 
            this._blockZTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._blockZTextBox.ForeColor = System.Drawing.Color.Gray;
            this._blockZTextBox.Location = new System.Drawing.Point(38, 45);
            this._blockZTextBox.Name = "_blockZTextBox";
            this._blockZTextBox.Size = new System.Drawing.Size(100, 20);
            this._blockZTextBox.TabIndex = 1;
            this._blockZTextBox.Text = "Type here";
            this._blockZTextBox.WatermarkActive = true;
            this._blockZTextBox.WatermarkText = "Type here";
            this._blockZTextBox.TextChanged += new System.EventHandler(this._blockZTextBox_TextChanged);
            // 
            // _blockXTextBox
            // 
            this._blockXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._blockXTextBox.ForeColor = System.Drawing.Color.Gray;
            this._blockXTextBox.Location = new System.Drawing.Point(38, 19);
            this._blockXTextBox.Name = "_blockXTextBox";
            this._blockXTextBox.Size = new System.Drawing.Size(100, 20);
            this._blockXTextBox.TabIndex = 0;
            this._blockXTextBox.Text = "Type here";
            this._blockXTextBox.WatermarkActive = true;
            this._blockXTextBox.WatermarkText = "Type here";
            this._blockXTextBox.TextChanged += new System.EventHandler(this._blockXTextBox_TextChanged);
            // 
            // _chunkZTextBox
            // 
            this._chunkZTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._chunkZTextBox.ForeColor = System.Drawing.Color.Gray;
            this._chunkZTextBox.Location = new System.Drawing.Point(36, 45);
            this._chunkZTextBox.Name = "_chunkZTextBox";
            this._chunkZTextBox.Size = new System.Drawing.Size(100, 20);
            this._chunkZTextBox.TabIndex = 1;
            this._chunkZTextBox.Text = "Type here";
            this._chunkZTextBox.WatermarkActive = true;
            this._chunkZTextBox.WatermarkText = "Type here";
            this._chunkZTextBox.TextChanged += new System.EventHandler(this._chunkZTextBox_TextChanged);
            // 
            // _chunkXTextBox
            // 
            this._chunkXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._chunkXTextBox.ForeColor = System.Drawing.Color.Gray;
            this._chunkXTextBox.Location = new System.Drawing.Point(36, 19);
            this._chunkXTextBox.Name = "_chunkXTextBox";
            this._chunkXTextBox.Size = new System.Drawing.Size(100, 20);
            this._chunkXTextBox.TabIndex = 0;
            this._chunkXTextBox.Text = "Type here";
            this._chunkXTextBox.WatermarkActive = true;
            this._chunkXTextBox.WatermarkText = "Type here";
            this._chunkXTextBox.TextChanged += new System.EventHandler(this._chunkXTextBox_TextChanged);
            // 
            // _regionZTextBox
            // 
            this._regionZTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._regionZTextBox.ForeColor = System.Drawing.Color.Gray;
            this._regionZTextBox.Location = new System.Drawing.Point(36, 45);
            this._regionZTextBox.Name = "_regionZTextBox";
            this._regionZTextBox.Size = new System.Drawing.Size(100, 20);
            this._regionZTextBox.TabIndex = 1;
            this._regionZTextBox.Text = "Type here";
            this._regionZTextBox.WatermarkActive = true;
            this._regionZTextBox.WatermarkText = "Type here";
            this._regionZTextBox.TextChanged += new System.EventHandler(this._regionZTextBox_TextChanged);
            // 
            // _regionXTextBox
            // 
            this._regionXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._regionXTextBox.ForeColor = System.Drawing.Color.Gray;
            this._regionXTextBox.Location = new System.Drawing.Point(36, 19);
            this._regionXTextBox.Name = "_regionXTextBox";
            this._regionXTextBox.Size = new System.Drawing.Size(100, 20);
            this._regionXTextBox.TabIndex = 0;
            this._regionXTextBox.Text = "Type here";
            this._regionXTextBox.WatermarkActive = true;
            this._regionXTextBox.WatermarkText = "Type here";
            this._regionXTextBox.TextChanged += new System.EventHandler(this._regionXTextBox_TextChanged);
            // 
            // _localChunkXTextBox
            // 
            this._localChunkXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._localChunkXTextBox.ForeColor = System.Drawing.Color.Gray;
            this._localChunkXTextBox.Location = new System.Drawing.Point(36, 19);
            this._localChunkXTextBox.Name = "_localChunkXTextBox";
            this._localChunkXTextBox.Size = new System.Drawing.Size(100, 20);
            this._localChunkXTextBox.TabIndex = 6;
            this._localChunkXTextBox.Text = "Type here";
            this._localChunkXTextBox.WatermarkActive = true;
            this._localChunkXTextBox.WatermarkText = "Type here";
            this._localChunkXTextBox.TextChanged += new System.EventHandler(this._localChunkXTextBox_TextChanged);
            // 
            // _localChunkZTextBox
            // 
            this._localChunkZTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._localChunkZTextBox.ForeColor = System.Drawing.Color.Gray;
            this._localChunkZTextBox.Location = new System.Drawing.Point(36, 45);
            this._localChunkZTextBox.Name = "_localChunkZTextBox";
            this._localChunkZTextBox.Size = new System.Drawing.Size(100, 20);
            this._localChunkZTextBox.TabIndex = 7;
            this._localChunkZTextBox.Text = "Type here";
            this._localChunkZTextBox.WatermarkActive = true;
            this._localChunkZTextBox.WatermarkText = "Type here";
            this._localChunkZTextBox.TextChanged += new System.EventHandler(this._localChunkZTextBox_TextChanged);
            // 
            // _localBlockZTextBox
            // 
            this._localBlockZTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._localBlockZTextBox.ForeColor = System.Drawing.Color.Gray;
            this._localBlockZTextBox.Location = new System.Drawing.Point(38, 45);
            this._localBlockZTextBox.Name = "_localBlockZTextBox";
            this._localBlockZTextBox.Size = new System.Drawing.Size(100, 20);
            this._localBlockZTextBox.TabIndex = 7;
            this._localBlockZTextBox.Text = "Type here";
            this._localBlockZTextBox.WatermarkActive = true;
            this._localBlockZTextBox.WatermarkText = "Type here";
            this._localBlockZTextBox.TextChanged += new System.EventHandler(this._localBlockZTextBox_TextChanged);
            // 
            // _localBlockXTextBox
            // 
            this._localBlockXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._localBlockXTextBox.ForeColor = System.Drawing.Color.Gray;
            this._localBlockXTextBox.Location = new System.Drawing.Point(38, 19);
            this._localBlockXTextBox.Name = "_localBlockXTextBox";
            this._localBlockXTextBox.Size = new System.Drawing.Size(100, 20);
            this._localBlockXTextBox.TabIndex = 6;
            this._localBlockXTextBox.Text = "Type here";
            this._localBlockXTextBox.WatermarkActive = true;
            this._localBlockXTextBox.WatermarkText = "Type here";
            this._localBlockXTextBox.TextChanged += new System.EventHandler(this._localBlockXTextBox_TextChanged);
            // 
            // FindBlock
            // 
            this.AcceptButton = this._findButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(470, 206);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._findButton);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FindBlock";
            this.Text = "Chunk Finder";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private WatermarkTextBox _regionZTextBox;
        private WatermarkTextBox _regionXTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private WatermarkTextBox _blockZTextBox;
        private WatermarkTextBox _blockXTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private WatermarkTextBox _chunkZTextBox;
        private WatermarkTextBox _chunkXTextBox;
        private System.Windows.Forms.Button _findButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private WatermarkTextBox _localChunkXTextBox;
        private System.Windows.Forms.Label label8;
        private WatermarkTextBox _localChunkZTextBox;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private WatermarkTextBox _localBlockZTextBox;
        private WatermarkTextBox _localBlockXTextBox;
        private System.Windows.Forms.Button _cancelButton;
    }
}