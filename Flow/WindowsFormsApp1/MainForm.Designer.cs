namespace WindowsFormsApp1
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
            this.tbWeight = new System.Windows.Forms.TrackBar();
            this.lblWeight = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.tbHeight = new System.Windows.Forms.TrackBar();
            this.lblBMI = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // tbWeight
            // 
            this.tbWeight.Location = new System.Drawing.Point(124, 12);
            this.tbWeight.Maximum = 150;
            this.tbWeight.Minimum = 40;
            this.tbWeight.Name = "tbWeight";
            this.tbWeight.Size = new System.Drawing.Size(267, 45);
            this.tbWeight.TabIndex = 0;
            this.tbWeight.Value = 40;
            // 
            // lblWeight
            // 
            this.lblWeight.AutoSize = true;
            this.lblWeight.Location = new System.Drawing.Point(12, 22);
            this.lblWeight.Name = "lblWeight";
            this.lblWeight.Size = new System.Drawing.Size(86, 13);
            this.lblWeight.TabIndex = 1;
            this.lblWeight.Text = "Weight (kg): 000";
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(12, 82);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(85, 13);
            this.lblHeight.TabIndex = 3;
            this.lblHeight.Text = "Height (cm): 000";
            // 
            // tbHeight
            // 
            this.tbHeight.Location = new System.Drawing.Point(124, 72);
            this.tbHeight.Maximum = 220;
            this.tbHeight.Minimum = 150;
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.Size = new System.Drawing.Size(267, 45);
            this.tbHeight.TabIndex = 2;
            this.tbHeight.Value = 150;
            // 
            // lblBMI
            // 
            this.lblBMI.AutoSize = true;
            this.lblBMI.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBMI.Location = new System.Drawing.Point(15, 164);
            this.lblBMI.Name = "lblBMI";
            this.lblBMI.Size = new System.Drawing.Size(152, 37);
            this.lblBMI.TabIndex = 4;
            this.lblBMI.Text = "BMI: 000";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(492, 164);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 347);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblBMI);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.tbHeight);
            this.Controls.Add(this.lblWeight);
            this.Controls.Add(this.tbWeight);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TrackBar tbWeight;
    private System.Windows.Forms.Label lblWeight;
    private System.Windows.Forms.Label lblHeight;
    private System.Windows.Forms.TrackBar tbHeight;
    private System.Windows.Forms.Label lblBMI;
        private System.Windows.Forms.Button btnReset;
    }
}

