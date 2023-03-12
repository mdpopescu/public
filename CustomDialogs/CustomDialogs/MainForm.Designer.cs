namespace CustomDialogs
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
            this.components = new System.ComponentModel.Container();
            this.rtxtLog = new Telerik.WinControls.UI.RadTextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.rtxtLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // rtxtLog
            // 
            this.rtxtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rtxtLog.Location = new System.Drawing.Point(0, 260);
            this.rtxtLog.Multiline = true;
            this.rtxtLog.Name = "rtxtLog";
            this.rtxtLog.ReadOnly = true;
            // 
            // 
            // 
            this.rtxtLog.RootElement.StretchVertically = true;
            this.rtxtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.rtxtLog.Size = new System.Drawing.Size(800, 190);
            this.rtxtLog.TabIndex = 0;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.rtxtLog);
            this.Name = "MainForm";
            this.Text = "MainForm -- CustomDialogs";
            ((System.ComponentModel.ISupportInitialize)(this.rtxtLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadTextBox rtxtLog;
        private System.Windows.Forms.Timer timer;
    }
}

