namespace Turtles
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            statusStrip1 = new StatusStrip();
            radMenu1 = new Telerik.WinControls.UI.RadMenu();
            radMenuItem1 = new Telerik.WinControls.UI.RadMenuItem();
            rmiFileOpen = new Telerik.WinControls.UI.RadMenuItem();
            rmiFileSave = new Telerik.WinControls.UI.RadMenuItem();
            rmiFileSaveAs = new Telerik.WinControls.UI.RadMenuItem();
            radMenuSeparatorItem1 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            rmiFileExit = new Telerik.WinControls.UI.RadMenuItem();
            radMenuItem2 = new Telerik.WinControls.UI.RadMenuItem();
            rmiHelpAbout = new Telerik.WinControls.UI.RadMenuItem();
            tsslblPosition = new ToolStripStatusLabel();
            tsslblDirection = new ToolStripStatusLabel();
            radSplitContainer1 = new Telerik.WinControls.UI.RadSplitContainer();
            splitPanel1 = new Telerik.WinControls.UI.SplitPanel();
            splitPanel2 = new Telerik.WinControls.UI.SplitPanel();
            pbCanvas = new PictureBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            rtxtCommand = new Telerik.WinControls.UI.RadTextBox();
            rrtxtCode = new Telerik.WinControls.UI.RadRichTextEditor();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radMenu1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radSplitContainer1).BeginInit();
            radSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitPanel1).BeginInit();
            splitPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitPanel2).BeginInit();
            splitPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbCanvas).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)rtxtCommand).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rrtxtCode).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsslblPosition, tsslblDirection });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // radMenu1
            // 
            radMenu1.Items.AddRange(new Telerik.WinControls.RadItem[] { radMenuItem1, radMenuItem2 });
            radMenu1.Location = new Point(0, 0);
            radMenu1.Name = "radMenu1";
            radMenu1.Size = new Size(800, 20);
            radMenu1.TabIndex = 3;
            // 
            // radMenuItem1
            // 
            radMenuItem1.Items.AddRange(new Telerik.WinControls.RadItem[] { rmiFileOpen, rmiFileSave, rmiFileSaveAs, radMenuSeparatorItem1, rmiFileExit });
            radMenuItem1.Name = "radMenuItem1";
            radMenuItem1.Text = "&File";
            // 
            // rmiFileOpen
            // 
            rmiFileOpen.Name = "rmiFileOpen";
            rmiFileOpen.Text = "&Open...";
            rmiFileOpen.Click += rmiFileOpen_Click;
            // 
            // rmiFileSave
            // 
            rmiFileSave.Enabled = false;
            rmiFileSave.Name = "rmiFileSave";
            rmiFileSave.Text = "Save";
            rmiFileSave.Click += rmiFileSave_Click;
            // 
            // rmiFileSaveAs
            // 
            rmiFileSaveAs.Enabled = false;
            rmiFileSaveAs.Name = "rmiFileSaveAs";
            rmiFileSaveAs.Text = "Save &As...";
            rmiFileSaveAs.Click += rmiFileSaveAs_Click;
            // 
            // radMenuSeparatorItem1
            // 
            radMenuSeparatorItem1.Name = "radMenuSeparatorItem1";
            radMenuSeparatorItem1.Text = "radMenuSeparatorItem1";
            radMenuSeparatorItem1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // rmiFileExit
            // 
            rmiFileExit.Name = "rmiFileExit";
            rmiFileExit.Text = "E&xit";
            rmiFileExit.Click += rmiFileExit_Click;
            // 
            // radMenuItem2
            // 
            radMenuItem2.Items.AddRange(new Telerik.WinControls.RadItem[] { rmiHelpAbout });
            radMenuItem2.Name = "radMenuItem2";
            radMenuItem2.Text = "&Help";
            // 
            // rmiHelpAbout
            // 
            rmiHelpAbout.Name = "rmiHelpAbout";
            rmiHelpAbout.Text = "&About...";
            rmiHelpAbout.Click += rmiHelpAbout_Click;
            // 
            // tsslblPosition
            // 
            tsslblPosition.Name = "tsslblPosition";
            tsslblPosition.Size = new Size(25, 17);
            tsslblPosition.Text = "0, 0";
            // 
            // tsslblDirection
            // 
            tsslblDirection.Name = "tsslblDirection";
            tsslblDirection.Size = new Size(13, 17);
            tsslblDirection.Text = "0";
            // 
            // radSplitContainer1
            // 
            radSplitContainer1.Controls.Add(splitPanel1);
            radSplitContainer1.Controls.Add(splitPanel2);
            radSplitContainer1.Dock = DockStyle.Fill;
            radSplitContainer1.Location = new Point(0, 20);
            radSplitContainer1.Name = "radSplitContainer1";
            // 
            // 
            // 
            radSplitContainer1.RootElement.MinSize = new Size(25, 25);
            radSplitContainer1.Size = new Size(800, 408);
            radSplitContainer1.TabIndex = 4;
            radSplitContainer1.TabStop = false;
            // 
            // splitPanel1
            // 
            splitPanel1.Controls.Add(tableLayoutPanel1);
            splitPanel1.Location = new Point(0, 0);
            splitPanel1.Name = "splitPanel1";
            // 
            // 
            // 
            splitPanel1.RootElement.MinSize = new Size(25, 25);
            splitPanel1.Size = new Size(398, 408);
            splitPanel1.TabIndex = 0;
            splitPanel1.TabStop = false;
            splitPanel1.Text = "splitPanel1";
            // 
            // splitPanel2
            // 
            splitPanel2.Controls.Add(pbCanvas);
            splitPanel2.Location = new Point(402, 0);
            splitPanel2.Name = "splitPanel2";
            // 
            // 
            // 
            splitPanel2.RootElement.MinSize = new Size(25, 25);
            splitPanel2.Size = new Size(398, 408);
            splitPanel2.TabIndex = 1;
            splitPanel2.TabStop = false;
            splitPanel2.Text = "splitPanel2";
            // 
            // pbCanvas
            // 
            pbCanvas.BackColor = Color.LightGray;
            pbCanvas.Dock = DockStyle.Fill;
            pbCanvas.Location = new Point(0, 0);
            pbCanvas.Name = "pbCanvas";
            pbCanvas.Size = new Size(398, 408);
            pbCanvas.TabIndex = 3;
            pbCanvas.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(rtxtCommand, 0, 1);
            tableLayoutPanel1.Controls.Add(rrtxtCode, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanel1.Size = new Size(398, 408);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // rtxtCommand
            // 
            rtxtCommand.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            rtxtCommand.Location = new Point(3, 387);
            rtxtCommand.Name = "rtxtCommand";
            rtxtCommand.Size = new Size(392, 20);
            rtxtCommand.TabIndex = 0;
            // 
            // rrtxtCode
            // 
            rrtxtCode.BorderColor = Color.FromArgb(156, 189, 232);
            rrtxtCode.Dock = DockStyle.Fill;
            rrtxtCode.Location = new Point(3, 3);
            rrtxtCode.Name = "rrtxtCode";
            rrtxtCode.SelectionFill = Color.FromArgb(128, 78, 158, 255);
            rrtxtCode.Size = new Size(392, 378);
            rrtxtCode.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new Size(7, 15);
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(radSplitContainer1);
            Controls.Add(radMenu1);
            Controls.Add(statusStrip1);
            Name = "MainForm";
            Text = "Turtles -- Untitled";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)radMenu1).EndInit();
            ((System.ComponentModel.ISupportInitialize)radSplitContainer1).EndInit();
            radSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitPanel1).EndInit();
            splitPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitPanel2).EndInit();
            splitPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbCanvas).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)rtxtCommand).EndInit();
            ((System.ComponentModel.ISupportInitialize)rrtxtCode).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private StatusStrip statusStrip1;
        private Telerik.WinControls.UI.RadMenu radMenu1;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem1;
        private Telerik.WinControls.UI.RadMenuItem rmiFileOpen;
        private Telerik.WinControls.UI.RadMenuItem rmiFileSave;
        private Telerik.WinControls.UI.RadMenuItem rmiFileSaveAs;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem1;
        private Telerik.WinControls.UI.RadMenuItem rmiFileExit;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem2;
        private Telerik.WinControls.UI.RadMenuItem rmiHelpAbout;
        private ToolStripStatusLabel tsslblPosition;
        private ToolStripStatusLabel tsslblDirection;
        private Telerik.WinControls.UI.RadSplitContainer radSplitContainer1;
        private Telerik.WinControls.UI.SplitPanel splitPanel1;
        private Telerik.WinControls.UI.SplitPanel splitPanel2;
        private PictureBox pbCanvas;
        private TableLayoutPanel tableLayoutPanel1;
        private Telerik.WinControls.UI.RadTextBox rtxtCommand;
        private Telerik.WinControls.UI.RadRichTextEditor rrtxtCode;
    }
}