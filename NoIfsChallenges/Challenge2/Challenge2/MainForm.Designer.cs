namespace Challenge2
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
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnHold = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblClock = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(12, 12);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(141, 23);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "START/STOP";
            this.btnStartStop.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(159, 12);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // btnHold
            // 
            this.btnHold.Location = new System.Drawing.Point(240, 12);
            this.btnHold.Name = "btnHold";
            this.btnHold.Size = new System.Drawing.Size(69, 23);
            this.btnHold.TabIndex = 3;
            this.btnHold.Text = "HOLD";
            this.btnHold.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel1.Controls.Add(this.lblClock);
            this.panel1.Location = new System.Drawing.Point(65, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 27);
            this.panel1.TabIndex = 4;
            // 
            // lblClock
            // 
            this.lblClock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClock.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClock.Location = new System.Drawing.Point(0, 0);
            this.lblClock.Name = "lblClock";
            this.lblClock.Size = new System.Drawing.Size(200, 27);
            this.lblClock.TabIndex = 0;
            this.lblClock.Text = "00:00:00";
            this.lblClock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 81);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnHold);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStartStop);
            this.Name = "MainForm";
            this.Text = "Stopwatch";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnHold;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblClock;
    }
}

