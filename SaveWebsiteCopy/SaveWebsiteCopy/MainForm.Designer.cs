namespace SaveWebsiteCopy
{
    partial class MainForm
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
      this.label1 = new System.Windows.Forms.Label();
      this.txtURL = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.txtSearchString = new System.Windows.Forms.TextBox();
      this.txtSaveDirectory = new System.Windows.Forms.TextBox();
      this.btnLoad = new System.Windows.Forms.Button();
      this.pbLoading = new System.Windows.Forms.ProgressBar();
      this.btnSelectDirectory = new System.Windows.Forms.Button();
      this.txtLog = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(29, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "URL";
      // 
      // txtURL
      // 
      this.txtURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtURL.Location = new System.Drawing.Point(116, 10);
      this.txtURL.Name = "txtURL";
      this.txtURL.Size = new System.Drawing.Size(493, 20);
      this.txtURL.TabIndex = 1;
      this.txtURL.TextChanged += new System.EventHandler(this.txt_TextChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(13, 39);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(69, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Search string";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(13, 66);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(75, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Save directory";
      // 
      // txtSearchString
      // 
      this.txtSearchString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSearchString.Location = new System.Drawing.Point(116, 36);
      this.txtSearchString.Name = "txtSearchString";
      this.txtSearchString.Size = new System.Drawing.Size(493, 20);
      this.txtSearchString.TabIndex = 3;
      this.txtSearchString.TextChanged += new System.EventHandler(this.txt_TextChanged);
      // 
      // txtSaveDirectory
      // 
      this.txtSaveDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSaveDirectory.Location = new System.Drawing.Point(116, 63);
      this.txtSaveDirectory.Name = "txtSaveDirectory";
      this.txtSaveDirectory.Size = new System.Drawing.Size(457, 20);
      this.txtSaveDirectory.TabIndex = 5;
      this.txtSaveDirectory.TextChanged += new System.EventHandler(this.txt_TextChanged);
      // 
      // btnLoad
      // 
      this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnLoad.Enabled = false;
      this.btnLoad.Location = new System.Drawing.Point(534, 89);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(75, 23);
      this.btnLoad.TabIndex = 7;
      this.btnLoad.Text = "&Load";
      this.btnLoad.UseVisualStyleBackColor = true;
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
      // 
      // pbLoading
      // 
      this.pbLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pbLoading.Location = new System.Drawing.Point(16, 118);
      this.pbLoading.Name = "pbLoading";
      this.pbLoading.Size = new System.Drawing.Size(593, 23);
      this.pbLoading.TabIndex = 8;
      // 
      // btnSelectDirectory
      // 
      this.btnSelectDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSelectDirectory.Location = new System.Drawing.Point(579, 62);
      this.btnSelectDirectory.Name = "btnSelectDirectory";
      this.btnSelectDirectory.Size = new System.Drawing.Size(30, 21);
      this.btnSelectDirectory.TabIndex = 6;
      this.btnSelectDirectory.Text = "...";
      this.btnSelectDirectory.UseVisualStyleBackColor = true;
      this.btnSelectDirectory.Click += new System.EventHandler(this.btnSelectDirectory_Click);
      // 
      // txtLog
      // 
      this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtLog.Location = new System.Drawing.Point(13, 147);
      this.txtLog.Multiline = true;
      this.txtLog.Name = "txtLog";
      this.txtLog.ReadOnly = true;
      this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtLog.Size = new System.Drawing.Size(596, 175);
      this.txtLog.TabIndex = 9;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(621, 334);
      this.Controls.Add(this.txtLog);
      this.Controls.Add(this.btnSelectDirectory);
      this.Controls.Add(this.pbLoading);
      this.Controls.Add(this.btnLoad);
      this.Controls.Add(this.txtSaveDirectory);
      this.Controls.Add(this.txtSearchString);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.txtURL);
      this.Controls.Add(this.label1);
      this.Name = "MainForm";
      this.Text = "Save Website Copy";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSearchString;
        private System.Windows.Forms.TextBox txtSaveDirectory;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ProgressBar pbLoading;
        private System.Windows.Forms.Button btnSelectDirectory;
    private System.Windows.Forms.TextBox txtLog;
  }
}

