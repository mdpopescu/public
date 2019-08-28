namespace WinFormsClock
{
    partial class Clock
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
            this.clockBackground1 = new WinFormsClock.ClockBackground();
            this.SuspendLayout();
            // 
            // clockBackground1
            // 
            this.clockBackground1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clockBackground1.Location = new System.Drawing.Point(0, 0);
            this.clockBackground1.Name = "clockBackground1";
            this.clockBackground1.Size = new System.Drawing.Size(150, 150);
            this.clockBackground1.TabIndex = 0;
            // 
            // Clock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.clockBackground1);
            this.Name = "Clock";
            this.Load += new System.EventHandler(this.Clock_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Clock_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private ClockBackground clockBackground1;
    }
}
