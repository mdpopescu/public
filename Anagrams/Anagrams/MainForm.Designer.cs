namespace Renfield.Anagrams
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
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.label2 = new System.Windows.Forms.Label();
      this.btnGenerate = new System.Windows.Forms.Button();
      this.txtOutput = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txtMinWordLength = new System.Windows.Forms.TextBox();
      this.txtText = new System.Windows.Forms.TextBox();
      this.menuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(653, 24);
      this.menuStrip.TabIndex = 3;
      this.menuStrip.Text = "menuStrip";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
      this.exitToolStripMenuItem.Text = "E&xit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.helpToolStripMenuItem.Text = "&Help";
      // 
      // aboutToolStripMenuItem
      // 
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
      this.aboutToolStripMenuItem.Text = "&About...";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(13, 50);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(28, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Text";
      // 
      // btnGenerate
      // 
      this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnGenerate.Location = new System.Drawing.Point(566, 45);
      this.btnGenerate.Name = "btnGenerate";
      this.btnGenerate.Size = new System.Drawing.Size(75, 23);
      this.btnGenerate.TabIndex = 6;
      this.btnGenerate.Text = "Generate";
      this.btnGenerate.UseVisualStyleBackColor = true;
      this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
      // 
      // txtOutput
      // 
      this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtOutput.Location = new System.Drawing.Point(13, 73);
      this.txtOutput.Multiline = true;
      this.txtOutput.Name = "txtOutput";
      this.txtOutput.ReadOnly = true;
      this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtOutput.Size = new System.Drawing.Size(628, 178);
      this.txtOutput.TabIndex = 7;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(433, 50);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(82, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Min word length";
      // 
      // txtMinWordLength
      // 
      this.txtMinWordLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMinWordLength.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Renfield.Anagrams.Properties.Settings.Default, "MinWordLength", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.txtMinWordLength.Location = new System.Drawing.Point(521, 47);
      this.txtMinWordLength.Name = "txtMinWordLength";
      this.txtMinWordLength.Size = new System.Drawing.Size(38, 20);
      this.txtMinWordLength.TabIndex = 9;
      this.txtMinWordLength.Text = global::Renfield.Anagrams.Properties.Settings.Default.MinWordLength;
      this.txtMinWordLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // txtText
      // 
      this.txtText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtText.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Renfield.Anagrams.Properties.Settings.Default, "Text", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.txtText.Location = new System.Drawing.Point(67, 47);
      this.txtText.Name = "txtText";
      this.txtText.Size = new System.Drawing.Size(360, 20);
      this.txtText.TabIndex = 5;
      this.txtText.Text = global::Renfield.Anagrams.Properties.Settings.Default.Text;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(653, 263);
      this.Controls.Add(this.txtMinWordLength);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.txtOutput);
      this.Controls.Add(this.btnGenerate);
      this.Controls.Add(this.txtText);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.menuStrip);
      this.MainMenuStrip = this.menuStrip;
      this.MinimumSize = new System.Drawing.Size(600, 300);
      this.Name = "MainForm";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.TextBox txtText;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btnGenerate;
    private System.Windows.Forms.TextBox txtOutput;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtMinWordLength;
  }
}

