namespace AppUpdater.Tester
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
      this.autoUpdater = new Renfield.AppUpdater.AutoUpdater();
      this.SuspendLayout();
      // 
      // autoUpdater
      // 
      this.autoUpdater.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.autoUpdater.Location = new System.Drawing.Point(256, 12);
      this.autoUpdater.ManifestURI = "S:\\GIT-public\\AppUpdater\\AppUpdater.Tester\\manifest.xml";
      this.autoUpdater.Name = "autoUpdater";
      this.autoUpdater.Size = new System.Drawing.Size(16, 16);
      this.autoUpdater.TabIndex = 0;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.Controls.Add(this.autoUpdater);
      this.Name = "MainForm";
      this.Text = "AutoUpdater Tester";
      this.ResumeLayout(false);

    }

    #endregion

    private Renfield.AppUpdater.AutoUpdater autoUpdater;
  }
}

