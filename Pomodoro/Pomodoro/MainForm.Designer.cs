namespace Pomodoro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblRemaining = new System.Windows.Forms.Label();
            this.rb20min = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.rb30min = new System.Windows.Forms.RadioButton();
            this.rb10min = new System.Windows.Forms.RadioButton();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.lblRemaining, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.rb20min, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnStart, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.rb30min, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.rb10min, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 67.47968F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32.52032F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(445, 145);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // lblRemaining
            // 
            this.lblRemaining.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblRemaining.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblRemaining, 2);
            this.lblRemaining.Font = new System.Drawing.Font("Georgia", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemaining.Location = new System.Drawing.Point(45, 12);
            this.lblRemaining.Name = "lblRemaining";
            this.lblRemaining.Size = new System.Drawing.Size(206, 72);
            this.lblRemaining.TabIndex = 5;
            this.lblRemaining.Text = "00:00";
            // 
            // rb20min
            // 
            this.rb20min.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb20min.AutoSize = true;
            this.rb20min.Checked = true;
            this.rb20min.Location = new System.Drawing.Point(177, 107);
            this.rb20min.Name = "rb20min";
            this.rb20min.Size = new System.Drawing.Size(90, 27);
            this.rb20min.TabIndex = 3;
            this.rb20min.TabStop = true;
            this.rb20min.Text = "20 min";
            this.rb20min.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStart.Location = new System.Drawing.Point(320, 24);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(101, 48);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // rb30min
            // 
            this.rb30min.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb30min.AutoSize = true;
            this.rb30min.Location = new System.Drawing.Point(326, 107);
            this.rb30min.Name = "rb30min";
            this.rb30min.Size = new System.Drawing.Size(89, 27);
            this.rb30min.TabIndex = 4;
            this.rb30min.Text = "30 min";
            this.rb30min.UseVisualStyleBackColor = true;
            // 
            // rb10min
            // 
            this.rb10min.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb10min.AutoSize = true;
            this.rb10min.Location = new System.Drawing.Point(30, 107);
            this.rb10min.Name = "rb10min";
            this.rb10min.Size = new System.Drawing.Size(87, 27);
            this.rb10min.TabIndex = 2;
            this.rb10min.Text = "10 min";
            this.rb10min.UseVisualStyleBackColor = true;
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Pomodoro";
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRestore,
            this.mnuExit});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.contextMenu.Size = new System.Drawing.Size(114, 48);
            // 
            // mnuRestore
            // 
            this.mnuRestore.Name = "mnuRestore";
            this.mnuRestore.Size = new System.Drawing.Size(113, 22);
            this.mnuRestore.Text = "Restore";
            this.mnuRestore.Click += new System.EventHandler(this.mnuRestore_Click);
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(113, 22);
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 145);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Georgia", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "MainForm";
            this.Text = "Pomodoro";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblRemaining;
        private System.Windows.Forms.RadioButton rb30min;
        private System.Windows.Forms.RadioButton rb20min;
        private System.Windows.Forms.RadioButton rb10min;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuRestore;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.Timer timer;
    }
}

