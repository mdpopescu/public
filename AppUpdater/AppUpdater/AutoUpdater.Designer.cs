namespace Renfield.AppUpdater
{
  partial class AutoUpdater
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

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.picture = new System.Windows.Forms.PictureBox();
      this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.checkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
      this.contextMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // picture
      // 
      this.picture.ContextMenuStrip = this.contextMenu;
      this.picture.Image = global::Renfield.AppUpdater.Properties.Resources.ok;
      this.picture.Location = new System.Drawing.Point(0, 0);
      this.picture.Name = "picture";
      this.picture.Size = new System.Drawing.Size(16, 16);
      this.picture.TabIndex = 0;
      this.picture.TabStop = false;
      this.picture.Click += new System.EventHandler(this.picture_Click);
      // 
      // contextMenu
      // 
      this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkToolStripMenuItem,
            this.updateToolStripMenuItem});
      this.contextMenu.Name = "contextMenu";
      this.contextMenu.Size = new System.Drawing.Size(113, 48);
      // 
      // checkToolStripMenuItem
      // 
      this.checkToolStripMenuItem.Name = "checkToolStripMenuItem";
      this.checkToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.checkToolStripMenuItem.Text = "Check";
      this.checkToolStripMenuItem.Visible = false;
      this.checkToolStripMenuItem.Click += new System.EventHandler(this.checkToolStripMenuItem_Click);
      // 
      // updateToolStripMenuItem
      // 
      this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
      this.updateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.updateToolStripMenuItem.Text = "Update";
      this.updateToolStripMenuItem.Visible = false;
      this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
      // 
      // progressBar
      // 
      this.progressBar.Location = new System.Drawing.Point(16, 0);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(80, 16);
      this.progressBar.TabIndex = 1;
      this.progressBar.Visible = false;
      // 
      // AutoUpdater
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.progressBar);
      this.Controls.Add(this.picture);
      this.Name = "AutoUpdater";
      this.Size = new System.Drawing.Size(16, 16);
      this.Load += new System.EventHandler(this.AutoUpdater_Load);
      ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
      this.contextMenu.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox picture;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.ContextMenuStrip contextMenu;
    private System.Windows.Forms.ToolStripMenuItem checkToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
  }
}
