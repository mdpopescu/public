namespace Renfield.VideoSpinner
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
      this.btnBrowse1 = new System.Windows.Forms.Button();
      this.btnBrowse2 = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.btnBrowse3 = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.btnBrowse5 = new System.Windows.Forms.Button();
      this.label7 = new System.Windows.Forms.Label();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.pbStatus = new System.Windows.Forms.ToolStripProgressBar();
      this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
      this.btnStart = new System.Windows.Forms.Button();
      this.txtOutputFolder = new System.Windows.Forms.TextBox();
      this.txtSounds = new System.Windows.Forms.TextBox();
      this.txtImages = new System.Windows.Forms.TextBox();
      this.txtCsvFile = new System.Windows.Forms.TextBox();
      this.btnBrowse4 = new System.Windows.Forms.Button();
      this.txtWatermarkFile = new System.Windows.Forms.TextBox();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(8, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(44, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "CSV file";
      // 
      // btnBrowse1
      // 
      this.btnBrowse1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowse1.Location = new System.Drawing.Point(568, 30);
      this.btnBrowse1.Name = "btnBrowse1";
      this.btnBrowse1.Size = new System.Drawing.Size(34, 23);
      this.btnBrowse1.TabIndex = 2;
      this.btnBrowse1.Text = ". . .";
      this.btnBrowse1.UseVisualStyleBackColor = true;
      this.btnBrowse1.Click += new System.EventHandler(this.btnBrowse1_Click);
      // 
      // btnBrowse2
      // 
      this.btnBrowse2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowse2.Location = new System.Drawing.Point(568, 70);
      this.btnBrowse2.Name = "btnBrowse2";
      this.btnBrowse2.Size = new System.Drawing.Size(34, 23);
      this.btnBrowse2.TabIndex = 4;
      this.btnBrowse2.Text = ". . .";
      this.btnBrowse2.UseVisualStyleBackColor = true;
      this.btnBrowse2.Click += new System.EventHandler(this.btnBrowse2_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(8, 56);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(41, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Images";
      // 
      // btnBrowse3
      // 
      this.btnBrowse3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowse3.Location = new System.Drawing.Point(568, 110);
      this.btnBrowse3.Name = "btnBrowse3";
      this.btnBrowse3.Size = new System.Drawing.Size(34, 23);
      this.btnBrowse3.TabIndex = 6;
      this.btnBrowse3.Text = ". . .";
      this.btnBrowse3.UseVisualStyleBackColor = true;
      this.btnBrowse3.Click += new System.EventHandler(this.btnBrowse3_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(8, 96);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(43, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Sounds";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(8, 136);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(59, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "Watermark";
      // 
      // btnBrowse5
      // 
      this.btnBrowse5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowse5.Location = new System.Drawing.Point(568, 190);
      this.btnBrowse5.Name = "btnBrowse5";
      this.btnBrowse5.Size = new System.Drawing.Size(34, 23);
      this.btnBrowse5.TabIndex = 10;
      this.btnBrowse5.Text = ". . .";
      this.btnBrowse5.UseVisualStyleBackColor = true;
      this.btnBrowse5.Click += new System.EventHandler(this.btnBrowse5_Click);
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(8, 176);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(68, 13);
      this.label7.TabIndex = 16;
      this.label7.Text = "Output folder";
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pbStatus,
            this.lblStatus});
      this.statusStrip1.Location = new System.Drawing.Point(0, 265);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(610, 22);
      this.statusStrip1.TabIndex = 20;
      this.statusStrip1.Text = "statusStrip";
      // 
      // pbStatus
      // 
      this.pbStatus.Name = "pbStatus";
      this.pbStatus.Size = new System.Drawing.Size(100, 16);
      // 
      // lblStatus
      // 
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(26, 17);
      this.lblStatus.Text = "Idle";
      // 
      // btnStart
      // 
      this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnStart.Location = new System.Drawing.Point(512, 224);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new System.Drawing.Size(91, 31);
      this.btnStart.TabIndex = 11;
      this.btnStart.Text = "&Start";
      this.btnStart.UseVisualStyleBackColor = true;
      this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
      // 
      // txtOutputFolder
      // 
      this.txtOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtOutputFolder.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Renfield.VideoSpinner.Properties.Settings.Default, "outputFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.txtOutputFolder.Location = new System.Drawing.Point(8, 192);
      this.txtOutputFolder.Name = "txtOutputFolder";
      this.txtOutputFolder.Size = new System.Drawing.Size(552, 20);
      this.txtOutputFolder.TabIndex = 9;
      this.txtOutputFolder.Text = global::Renfield.VideoSpinner.Properties.Settings.Default.outputFolder;
      // 
      // txtSounds
      // 
      this.txtSounds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSounds.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Renfield.VideoSpinner.Properties.Settings.Default, "soundsFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.txtSounds.Location = new System.Drawing.Point(8, 112);
      this.txtSounds.Name = "txtSounds";
      this.txtSounds.Size = new System.Drawing.Size(552, 20);
      this.txtSounds.TabIndex = 5;
      this.txtSounds.Text = global::Renfield.VideoSpinner.Properties.Settings.Default.soundsFolder;
      // 
      // txtImages
      // 
      this.txtImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtImages.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Renfield.VideoSpinner.Properties.Settings.Default, "imagesFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.txtImages.Location = new System.Drawing.Point(8, 72);
      this.txtImages.Name = "txtImages";
      this.txtImages.Size = new System.Drawing.Size(552, 20);
      this.txtImages.TabIndex = 3;
      this.txtImages.Text = global::Renfield.VideoSpinner.Properties.Settings.Default.imagesFolder;
      // 
      // txtCsvFile
      // 
      this.txtCsvFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtCsvFile.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Renfield.VideoSpinner.Properties.Settings.Default, "csvFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.txtCsvFile.Location = new System.Drawing.Point(8, 32);
      this.txtCsvFile.Name = "txtCsvFile";
      this.txtCsvFile.Size = new System.Drawing.Size(552, 20);
      this.txtCsvFile.TabIndex = 1;
      this.txtCsvFile.Text = global::Renfield.VideoSpinner.Properties.Settings.Default.csvFile;
      // 
      // btnBrowse4
      // 
      this.btnBrowse4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowse4.Location = new System.Drawing.Point(568, 150);
      this.btnBrowse4.Name = "btnBrowse4";
      this.btnBrowse4.Size = new System.Drawing.Size(34, 23);
      this.btnBrowse4.TabIndex = 8;
      this.btnBrowse4.Text = ". . .";
      this.btnBrowse4.UseVisualStyleBackColor = true;
      this.btnBrowse4.Click += new System.EventHandler(this.btnBrowse4_Click);
      // 
      // txtWatermarkFile
      // 
      this.txtWatermarkFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtWatermarkFile.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Renfield.VideoSpinner.Properties.Settings.Default, "watermarkFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.txtWatermarkFile.Location = new System.Drawing.Point(8, 152);
      this.txtWatermarkFile.Name = "txtWatermarkFile";
      this.txtWatermarkFile.Size = new System.Drawing.Size(552, 20);
      this.txtWatermarkFile.TabIndex = 7;
      this.txtWatermarkFile.Text = global::Renfield.VideoSpinner.Properties.Settings.Default.watermarkFile;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(610, 287);
      this.Controls.Add(this.btnBrowse4);
      this.Controls.Add(this.txtWatermarkFile);
      this.Controls.Add(this.btnStart);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.btnBrowse5);
      this.Controls.Add(this.txtOutputFolder);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.btnBrowse3);
      this.Controls.Add(this.txtSounds);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.btnBrowse2);
      this.Controls.Add(this.txtImages);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.btnBrowse1);
      this.Controls.Add(this.txtCsvFile);
      this.Controls.Add(this.label1);
      this.MinimumSize = new System.Drawing.Size(500, 325);
      this.Name = "MainForm";
      this.Text = "Video Spinner";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCsvFile;
        private System.Windows.Forms.Button btnBrowse1;
        private System.Windows.Forms.Button btnBrowse2;
        private System.Windows.Forms.TextBox txtImages;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowse3;
        private System.Windows.Forms.TextBox txtSounds;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBrowse5;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ToolStripProgressBar pbStatus;
        private System.Windows.Forms.Button btnBrowse4;
        private System.Windows.Forms.TextBox txtWatermarkFile;
    }
}

