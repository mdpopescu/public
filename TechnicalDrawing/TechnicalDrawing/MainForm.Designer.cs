namespace TechnicalDrawing
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileOpenMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.YZImage = new System.Windows.Forms.PictureBox();
            this.XZImage = new System.Windows.Forms.PictureBox();
            this.XYImage = new System.Windows.Forms.PictureBox();
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YZImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XZImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XYImage)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1316, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileOpenMenu});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // FileOpenMenu
            // 
            this.FileOpenMenu.Name = "FileOpenMenu";
            this.FileOpenMenu.Size = new System.Drawing.Size(103, 22);
            this.FileOpenMenu.Text = "&Open";
            this.FileOpenMenu.Click += new System.EventHandler(this.FileOpenMenu_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1316, 520);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.YZImage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.XZImage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.XYImage, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1316, 520);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // YZImage
            // 
            this.YZImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.YZImage.Location = new System.Drawing.Point(3, 263);
            this.YZImage.Name = "YZImage";
            this.YZImage.Size = new System.Drawing.Size(652, 254);
            this.YZImage.TabIndex = 2;
            this.YZImage.TabStop = false;
            // 
            // XZImage
            // 
            this.XZImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.XZImage.Location = new System.Drawing.Point(661, 3);
            this.XZImage.Name = "XZImage";
            this.XZImage.Size = new System.Drawing.Size(652, 254);
            this.XZImage.TabIndex = 1;
            this.XZImage.TabStop = false;
            // 
            // XYImage
            // 
            this.XYImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.XYImage.Location = new System.Drawing.Point(3, 3);
            this.XYImage.Name = "XYImage";
            this.XYImage.Size = new System.Drawing.Size(652, 254);
            this.XYImage.TabIndex = 0;
            this.XYImage.TabStop = false;
            // 
            // StatusBar
            // 
            this.StatusBar.Location = new System.Drawing.Point(0, 522);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(1316, 22);
            this.StatusBar.TabIndex = 2;
            this.StatusBar.Text = "statusStrip1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1316, 544);
            this.Controls.Add(this.StatusBar);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Technical Drawing";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.YZImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XZImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XYImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileOpenMenu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox YZImage;
        private System.Windows.Forms.PictureBox XZImage;
        private System.Windows.Forms.PictureBox XYImage;
        private System.Windows.Forms.StatusStrip StatusBar;
    }
}

