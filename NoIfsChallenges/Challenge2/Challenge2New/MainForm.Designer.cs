
namespace Challenge2New
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
            this.gbImpChoice = new System.Windows.Forms.GroupBox();
            this.btnImplWithIfs = new System.Windows.Forms.Button();
            this.btnImplWithState = new System.Windows.Forms.Button();
            this.btnImplRxState = new System.Windows.Forms.Button();
            this.btnImplRxFunc = new System.Windows.Forms.Button();
            this.gbInterface = new System.Windows.Forms.GroupBox();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnHold = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblDisplay = new System.Windows.Forms.Label();
            this.gbImpChoice.SuspendLayout();
            this.gbInterface.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbImpChoice
            // 
            this.gbImpChoice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbImpChoice.Controls.Add(this.btnImplRxFunc);
            this.gbImpChoice.Controls.Add(this.btnImplRxState);
            this.gbImpChoice.Controls.Add(this.btnImplWithState);
            this.gbImpChoice.Controls.Add(this.btnImplWithIfs);
            this.gbImpChoice.Location = new System.Drawing.Point(12, 12);
            this.gbImpChoice.Name = "gbImpChoice";
            this.gbImpChoice.Size = new System.Drawing.Size(330, 79);
            this.gbImpChoice.TabIndex = 0;
            this.gbImpChoice.TabStop = false;
            this.gbImpChoice.Text = "Implementation";
            // 
            // btnImplWithIfs
            // 
            this.btnImplWithIfs.Location = new System.Drawing.Point(6, 32);
            this.btnImplWithIfs.Name = "btnImplWithIfs";
            this.btnImplWithIfs.Size = new System.Drawing.Size(75, 23);
            this.btnImplWithIfs.TabIndex = 0;
            this.btnImplWithIfs.Text = "With Ifs";
            this.btnImplWithIfs.UseVisualStyleBackColor = true;
            this.btnImplWithIfs.Click += new System.EventHandler(this.btnImplWithIfs_Click);
            // 
            // btnImplWithState
            // 
            this.btnImplWithState.Location = new System.Drawing.Point(87, 32);
            this.btnImplWithState.Name = "btnImplWithState";
            this.btnImplWithState.Size = new System.Drawing.Size(75, 23);
            this.btnImplWithState.TabIndex = 1;
            this.btnImplWithState.Text = "With State";
            this.btnImplWithState.UseVisualStyleBackColor = true;
            this.btnImplWithState.Click += new System.EventHandler(this.btnImplWithState_Click);
            // 
            // btnImplRxState
            // 
            this.btnImplRxState.Location = new System.Drawing.Point(168, 32);
            this.btnImplRxState.Name = "btnImplRxState";
            this.btnImplRxState.Size = new System.Drawing.Size(75, 23);
            this.btnImplRxState.TabIndex = 2;
            this.btnImplRxState.Text = "Rx State";
            this.btnImplRxState.UseVisualStyleBackColor = true;
            this.btnImplRxState.Click += new System.EventHandler(this.btnImplRxState_Click);
            // 
            // btnImplRxFunc
            // 
            this.btnImplRxFunc.Location = new System.Drawing.Point(249, 32);
            this.btnImplRxFunc.Name = "btnImplRxFunc";
            this.btnImplRxFunc.Size = new System.Drawing.Size(75, 23);
            this.btnImplRxFunc.TabIndex = 3;
            this.btnImplRxFunc.Text = "Rx Func";
            this.btnImplRxFunc.UseVisualStyleBackColor = true;
            this.btnImplRxFunc.Click += new System.EventHandler(this.btnImplRxFunc_Click);
            // 
            // gbInterface
            // 
            this.gbInterface.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbInterface.Controls.Add(this.lblDisplay);
            this.gbInterface.Controls.Add(this.btnReset);
            this.gbInterface.Controls.Add(this.btnHold);
            this.gbInterface.Controls.Add(this.btnStartStop);
            this.gbInterface.Location = new System.Drawing.Point(12, 97);
            this.gbInterface.Name = "gbInterface";
            this.gbInterface.Size = new System.Drawing.Size(330, 131);
            this.gbInterface.TabIndex = 1;
            this.gbInterface.TabStop = false;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(6, 22);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(156, 39);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "START/STOP";
            this.btnStartStop.UseVisualStyleBackColor = true;
            // 
            // btnHold
            // 
            this.btnHold.Location = new System.Drawing.Point(168, 22);
            this.btnHold.Name = "btnHold";
            this.btnHold.Size = new System.Drawing.Size(75, 39);
            this.btnHold.TabIndex = 1;
            this.btnHold.Text = "HOLD";
            this.btnHold.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(249, 22);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 39);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // lblDisplay
            // 
            this.lblDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDisplay.Font = new System.Drawing.Font("Courier New", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDisplay.Location = new System.Drawing.Point(6, 80);
            this.lblDisplay.Name = "lblDisplay";
            this.lblDisplay.Size = new System.Drawing.Size(318, 36);
            this.lblDisplay.TabIndex = 3;
            this.lblDisplay.Text = "00:00:00";
            this.lblDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 238);
            this.Controls.Add(this.gbInterface);
            this.Controls.Add(this.gbImpChoice);
            this.MinimumSize = new System.Drawing.Size(370, 275);
            this.Name = "MainForm";
            this.Text = "No Ifs Challenge 2 (New)";
            this.gbImpChoice.ResumeLayout(false);
            this.gbInterface.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbImpChoice;
        private System.Windows.Forms.Button btnImplRxFunc;
        private System.Windows.Forms.Button btnImplRxState;
        private System.Windows.Forms.Button btnImplWithState;
        private System.Windows.Forms.Button btnImplWithIfs;
        private System.Windows.Forms.GroupBox gbInterface;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnHold;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Label lblDisplay;
    }
}

