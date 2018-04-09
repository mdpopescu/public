namespace Challenge1
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
            this.btnDigit2 = new System.Windows.Forms.Button();
            this.txtDisplay = new System.Windows.Forms.TextBox();
            this.btnDigit3 = new System.Windows.Forms.Button();
            this.btnDigit5 = new System.Windows.Forms.Button();
            this.btnOpPlus = new System.Windows.Forms.Button();
            this.btnOpMinus = new System.Windows.Forms.Button();
            this.btnResult = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDigit2
            // 
            this.btnDigit2.Location = new System.Drawing.Point(12, 39);
            this.btnDigit2.Name = "btnDigit2";
            this.btnDigit2.Size = new System.Drawing.Size(33, 23);
            this.btnDigit2.TabIndex = 0;
            this.btnDigit2.Text = "2";
            this.btnDigit2.UseVisualStyleBackColor = true;
            this.btnDigit2.Click += new System.EventHandler(this.EnterDigit);
            // 
            // txtDisplay
            // 
            this.txtDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDisplay.Location = new System.Drawing.Point(13, 13);
            this.txtDisplay.Name = "txtDisplay";
            this.txtDisplay.Size = new System.Drawing.Size(420, 20);
            this.txtDisplay.TabIndex = 1;
            // 
            // btnDigit3
            // 
            this.btnDigit3.Location = new System.Drawing.Point(51, 39);
            this.btnDigit3.Name = "btnDigit3";
            this.btnDigit3.Size = new System.Drawing.Size(33, 23);
            this.btnDigit3.TabIndex = 2;
            this.btnDigit3.Text = "3";
            this.btnDigit3.UseVisualStyleBackColor = true;
            this.btnDigit3.Click += new System.EventHandler(this.EnterDigit);
            // 
            // btnDigit5
            // 
            this.btnDigit5.Location = new System.Drawing.Point(90, 39);
            this.btnDigit5.Name = "btnDigit5";
            this.btnDigit5.Size = new System.Drawing.Size(33, 23);
            this.btnDigit5.TabIndex = 3;
            this.btnDigit5.Text = "5";
            this.btnDigit5.UseVisualStyleBackColor = true;
            this.btnDigit5.Click += new System.EventHandler(this.EnterDigit);
            // 
            // btnOpPlus
            // 
            this.btnOpPlus.Location = new System.Drawing.Point(129, 39);
            this.btnOpPlus.Name = "btnOpPlus";
            this.btnOpPlus.Size = new System.Drawing.Size(33, 23);
            this.btnOpPlus.TabIndex = 4;
            this.btnOpPlus.Text = "+";
            this.btnOpPlus.UseVisualStyleBackColor = true;
            this.btnOpPlus.Click += new System.EventHandler(this.EnterPlus);
            // 
            // btnOpMinus
            // 
            this.btnOpMinus.Location = new System.Drawing.Point(168, 39);
            this.btnOpMinus.Name = "btnOpMinus";
            this.btnOpMinus.Size = new System.Drawing.Size(33, 23);
            this.btnOpMinus.TabIndex = 5;
            this.btnOpMinus.Text = "-";
            this.btnOpMinus.UseVisualStyleBackColor = true;
            this.btnOpMinus.Click += new System.EventHandler(this.EnterMinus);
            // 
            // btnResult
            // 
            this.btnResult.Location = new System.Drawing.Point(207, 39);
            this.btnResult.Name = "btnResult";
            this.btnResult.Size = new System.Drawing.Size(33, 23);
            this.btnResult.TabIndex = 6;
            this.btnResult.Text = "=";
            this.btnResult.UseVisualStyleBackColor = true;
            this.btnResult.Click += new System.EventHandler(this.EnterEqual);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 89);
            this.Controls.Add(this.btnResult);
            this.Controls.Add(this.btnOpMinus);
            this.Controls.Add(this.btnOpPlus);
            this.Controls.Add(this.btnDigit5);
            this.Controls.Add(this.btnDigit3);
            this.Controls.Add(this.txtDisplay);
            this.Controls.Add(this.btnDigit2);
            this.Name = "MainForm";
            this.Text = "Challenge 1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDigit2;
        private System.Windows.Forms.TextBox txtDisplay;
        private System.Windows.Forms.Button btnDigit3;
        private System.Windows.Forms.Button btnDigit5;
        private System.Windows.Forms.Button btnOpPlus;
        private System.Windows.Forms.Button btnOpMinus;
        private System.Windows.Forms.Button btnResult;
    }
}

