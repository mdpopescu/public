namespace TaskSpikes
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
      this.btnStart = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.lbItems = new System.Windows.Forms.ListBox();
      this.SuspendLayout();
      // 
      // btnStart
      // 
      this.btnStart.Location = new System.Drawing.Point(13, 13);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new System.Drawing.Size(75, 23);
      this.btnStart.TabIndex = 0;
      this.btnStart.Text = "Start";
      this.btnStart.UseVisualStyleBackColor = true;
      this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Enabled = false;
      this.btnCancel.Location = new System.Drawing.Point(155, 13);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // lbItems
      // 
      this.lbItems.FormattingEnabled = true;
      this.lbItems.Location = new System.Drawing.Point(13, 42);
      this.lbItems.Name = "lbItems";
      this.lbItems.Size = new System.Drawing.Size(217, 342);
      this.lbItems.TabIndex = 2;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(242, 398);
      this.Controls.Add(this.lbItems);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnStart);
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "TaskSpikes";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnStart;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.ListBox lbItems;
  }
}

