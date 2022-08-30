namespace WinRng
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIntervalMin = new System.Windows.Forms.TextBox();
            this.txtIntervalMax = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Interval";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(92, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "[";
            // 
            // txtIntervalMin
            // 
            this.txtIntervalMin.Location = new System.Drawing.Point(113, 6);
            this.txtIntervalMin.Name = "txtIntervalMin";
            this.txtIntervalMin.Size = new System.Drawing.Size(148, 27);
            this.txtIntervalMin.TabIndex = 2;
            this.txtIntervalMin.Text = "0";
            this.txtIntervalMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtIntervalMax
            // 
            this.txtIntervalMax.Location = new System.Drawing.Point(288, 6);
            this.txtIntervalMax.Name = "txtIntervalMax";
            this.txtIntervalMax.Size = new System.Drawing.Size(148, 27);
            this.txtIntervalMax.TabIndex = 4;
            this.txtIntervalMax.Text = "100";
            this.txtIntervalMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(267, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = ",";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(442, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 18);
            this.label4.TabIndex = 5;
            this.label4.Text = ")";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(463, 6);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(121, 27);
            this.btnGenerate.TabIndex = 6;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(622, 10);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(139, 18);
            this.lblResult.TabIndex = 7;
            this.lblResult.Text = "Result: 99.9999";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 41);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtIntervalMax);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtIntervalMin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "WinRng";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox txtIntervalMin;
        private TextBox txtIntervalMax;
        private Label label3;
        private Label label4;
        private Button btnGenerate;
        private Label lblResult;
    }
}