namespace WindowsFormsApp3
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
            this.lblSavedHeight = new System.Windows.Forms.Label();
            this.lblSavedWeight = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.lblBMI = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.tbHeight = new System.Windows.Forms.TrackBar();
            this.lblWeight = new System.Windows.Forms.Label();
            this.tbWeight = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.tbHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSavedHeight
            // 
            this.lblSavedHeight.AutoSize = true;
            this.lblSavedHeight.Location = new System.Drawing.Point(12, 247);
            this.lblSavedHeight.Name = "lblSavedHeight";
            this.lblSavedHeight.Size = new System.Drawing.Size(94, 13);
            this.lblSavedHeight.TabIndex = 17;
            this.lblSavedHeight.Text = "Saved height: 000";
            // 
            // lblSavedWeight
            // 
            this.lblSavedWeight.AutoSize = true;
            this.lblSavedWeight.Location = new System.Drawing.Point(12, 223);
            this.lblSavedWeight.Name = "lblSavedWeight";
            this.lblSavedWeight.Size = new System.Drawing.Size(96, 13);
            this.lblSavedWeight.TabIndex = 16;
            this.lblSavedWeight.Text = "Saved weight: 000";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(316, 151);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(316, 180);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(75, 23);
            this.btnRestore.TabIndex = 14;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = true;
            // 
            // lblBMI
            // 
            this.lblBMI.AutoSize = true;
            this.lblBMI.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBMI.Location = new System.Drawing.Point(15, 151);
            this.lblBMI.Name = "lblBMI";
            this.lblBMI.Size = new System.Drawing.Size(152, 37);
            this.lblBMI.TabIndex = 13;
            this.lblBMI.Text = "BMI: 000";
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(12, 69);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(85, 13);
            this.lblHeight.TabIndex = 12;
            this.lblHeight.Text = "Height (cm): 000";
            // 
            // tbHeight
            // 
            this.tbHeight.Location = new System.Drawing.Point(124, 59);
            this.tbHeight.Maximum = 220;
            this.tbHeight.Minimum = 150;
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.Size = new System.Drawing.Size(267, 45);
            this.tbHeight.TabIndex = 11;
            this.tbHeight.Value = 150;
            // 
            // lblWeight
            // 
            this.lblWeight.AutoSize = true;
            this.lblWeight.Location = new System.Drawing.Point(12, 9);
            this.lblWeight.Name = "lblWeight";
            this.lblWeight.Size = new System.Drawing.Size(86, 13);
            this.lblWeight.TabIndex = 10;
            this.lblWeight.Text = "Weight (kg): 000";
            // 
            // tbWeight
            // 
            this.tbWeight.Location = new System.Drawing.Point(124, -1);
            this.tbWeight.Maximum = 150;
            this.tbWeight.Minimum = 40;
            this.tbWeight.Name = "tbWeight";
            this.tbWeight.Size = new System.Drawing.Size(267, 45);
            this.tbWeight.TabIndex = 9;
            this.tbWeight.Value = 40;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 290);
            this.Controls.Add(this.lblSavedHeight);
            this.Controls.Add(this.lblSavedWeight);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.lblBMI);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.tbHeight);
            this.Controls.Add(this.lblWeight);
            this.Controls.Add(this.tbWeight);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSavedHeight;
        private System.Windows.Forms.Label lblSavedWeight;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Label lblBMI;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.TrackBar tbHeight;
        private System.Windows.Forms.Label lblWeight;
        private System.Windows.Forms.TrackBar tbWeight;
    }
}

