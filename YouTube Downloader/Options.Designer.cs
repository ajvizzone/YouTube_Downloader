namespace YouTube_Downloader
{
    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbAutoDelete = new System.Windows.Forms.CheckBox();
            this.chbAutoConvert = new System.Windows.Forms.CheckBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtDefaultDirectory = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cbAutoDelete
            // 
            this.cbAutoDelete.AutoSize = true;
            this.cbAutoDelete.Location = new System.Drawing.Point(15, 71);
            this.cbAutoDelete.Name = "cbAutoDelete";
            this.cbAutoDelete.Size = new System.Drawing.Size(193, 17);
            this.cbAutoDelete.TabIndex = 13;
            this.cbAutoDelete.Text = "Auto Delete Video After Conversion";
            this.cbAutoDelete.UseVisualStyleBackColor = true;
            // 
            // chbAutoConvert
            // 
            this.chbAutoConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbAutoConvert.AutoSize = true;
            this.chbAutoConvert.Location = new System.Drawing.Point(15, 94);
            this.chbAutoConvert.Name = "chbAutoConvert";
            this.chbAutoConvert.Size = new System.Drawing.Size(164, 17);
            this.chbAutoConvert.TabIndex = 12;
            this.chbAutoConvert.Text = "Convert to MP3 automatically";
            this.chbAutoConvert.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(104, 136);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 14;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Default Directory:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(253, 45);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(27, 20);
            this.btnBrowse.TabIndex = 17;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtDefaultDirectory
            // 
            this.txtDefaultDirectory.Location = new System.Drawing.Point(15, 45);
            this.txtDefaultDirectory.Name = "txtDefaultDirectory";
            this.txtDefaultDirectory.Size = new System.Drawing.Size(232, 20);
            this.txtDefaultDirectory.TabIndex = 18;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 180);
            this.Controls.Add(this.txtDefaultDirectory);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.cbAutoDelete);
            this.Controls.Add(this.chbAutoConvert);
            this.Name = "Options";
            this.Text = "Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Options_FormClosing);
            this.Load += new System.EventHandler(this.Options_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbAutoDelete;
        private System.Windows.Forms.CheckBox chbAutoConvert;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtDefaultDirectory;
    }
}