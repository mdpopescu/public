namespace Challenge4
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnCallTo3rd = new System.Windows.Forms.Button();
            this.txt3rdScreen = new System.Windows.Forms.TextBox();
            this.btnGoFrom3rdTo1st = new System.Windows.Forms.Button();
            this.btnGoFrom3rdTo2nd = new System.Windows.Forms.Button();
            this.txt2ndScreen = new System.Windows.Forms.TextBox();
            this.btnGoDown = new System.Windows.Forms.Button();
            this.btnGoUp = new System.Windows.Forms.Button();
            this.btnCallTo2nd = new System.Windows.Forms.Button();
            this.txt1stScreen = new System.Windows.Forms.TextBox();
            this.btnGoFrom1stTo2nd = new System.Windows.Forms.Button();
            this.btnGoFrom1stTo3rd = new System.Windows.Forms.Button();
            this.btnCallTo1st = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(327, 359);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt3rdScreen);
            this.groupBox1.Controls.Add(this.btnGoFrom3rdTo2nd);
            this.groupBox1.Controls.Add(this.btnGoFrom3rdTo1st);
            this.groupBox1.Controls.Add(this.btnCallTo3rd);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 113);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " 3rd Floor ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txt2ndScreen);
            this.groupBox2.Controls.Add(this.btnGoDown);
            this.groupBox2.Controls.Add(this.btnGoUp);
            this.groupBox2.Controls.Add(this.btnCallTo2nd);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 122);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(321, 113);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " 2nd Floor";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txt1stScreen);
            this.groupBox3.Controls.Add(this.btnGoFrom1stTo2nd);
            this.groupBox3.Controls.Add(this.btnGoFrom1stTo3rd);
            this.groupBox3.Controls.Add(this.btnCallTo1st);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 241);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(321, 115);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = " 1st Floor ";
            // 
            // btnCallTo3rd
            // 
            this.btnCallTo3rd.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCallTo3rd.Location = new System.Drawing.Point(3, 16);
            this.btnCallTo3rd.Name = "btnCallTo3rd";
            this.btnCallTo3rd.Size = new System.Drawing.Size(315, 23);
            this.btnCallTo3rd.TabIndex = 0;
            this.btnCallTo3rd.Text = "Call Elevator";
            this.btnCallTo3rd.UseVisualStyleBackColor = true;
            this.btnCallTo3rd.Click += new System.EventHandler(this.btnCallTo3rd_Click);
            // 
            // txt3rdScreen
            // 
            this.txt3rdScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt3rdScreen.Enabled = false;
            this.txt3rdScreen.Location = new System.Drawing.Point(3, 85);
            this.txt3rdScreen.Name = "txt3rdScreen";
            this.txt3rdScreen.Size = new System.Drawing.Size(315, 20);
            this.txt3rdScreen.TabIndex = 1;
            // 
            // btnGoFrom3rdTo1st
            // 
            this.btnGoFrom3rdTo1st.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGoFrom3rdTo1st.Location = new System.Drawing.Point(3, 39);
            this.btnGoFrom3rdTo1st.Name = "btnGoFrom3rdTo1st";
            this.btnGoFrom3rdTo1st.Size = new System.Drawing.Size(315, 23);
            this.btnGoFrom3rdTo1st.TabIndex = 2;
            this.btnGoFrom3rdTo1st.Text = "Go to 1st Floor";
            this.btnGoFrom3rdTo1st.UseVisualStyleBackColor = true;
            this.btnGoFrom3rdTo1st.Click += new System.EventHandler(this.btnGoFrom3rdTo1st_Click);
            // 
            // btnGoFrom3rdTo2nd
            // 
            this.btnGoFrom3rdTo2nd.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGoFrom3rdTo2nd.Location = new System.Drawing.Point(3, 62);
            this.btnGoFrom3rdTo2nd.Name = "btnGoFrom3rdTo2nd";
            this.btnGoFrom3rdTo2nd.Size = new System.Drawing.Size(315, 23);
            this.btnGoFrom3rdTo2nd.TabIndex = 3;
            this.btnGoFrom3rdTo2nd.Text = "Go to 2nd Floor";
            this.btnGoFrom3rdTo2nd.UseVisualStyleBackColor = true;
            this.btnGoFrom3rdTo2nd.Click += new System.EventHandler(this.btnGoFrom3rdTo2nd_Click);
            // 
            // txt2ndScreen
            // 
            this.txt2ndScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt2ndScreen.Enabled = false;
            this.txt2ndScreen.Location = new System.Drawing.Point(3, 85);
            this.txt2ndScreen.Name = "txt2ndScreen";
            this.txt2ndScreen.Size = new System.Drawing.Size(315, 20);
            this.txt2ndScreen.TabIndex = 5;
            // 
            // btnGoDown
            // 
            this.btnGoDown.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGoDown.Location = new System.Drawing.Point(3, 62);
            this.btnGoDown.Name = "btnGoDown";
            this.btnGoDown.Size = new System.Drawing.Size(315, 23);
            this.btnGoDown.TabIndex = 7;
            this.btnGoDown.Text = "Go Down";
            this.btnGoDown.UseVisualStyleBackColor = true;
            this.btnGoDown.Click += new System.EventHandler(this.btnGoDown_Click);
            // 
            // btnGoUp
            // 
            this.btnGoUp.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGoUp.Location = new System.Drawing.Point(3, 39);
            this.btnGoUp.Name = "btnGoUp";
            this.btnGoUp.Size = new System.Drawing.Size(315, 23);
            this.btnGoUp.TabIndex = 6;
            this.btnGoUp.Text = "Go Up";
            this.btnGoUp.UseVisualStyleBackColor = true;
            this.btnGoUp.Click += new System.EventHandler(this.btnGoUp_Click);
            // 
            // btnCallTo2nd
            // 
            this.btnCallTo2nd.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCallTo2nd.Location = new System.Drawing.Point(3, 16);
            this.btnCallTo2nd.Name = "btnCallTo2nd";
            this.btnCallTo2nd.Size = new System.Drawing.Size(315, 23);
            this.btnCallTo2nd.TabIndex = 4;
            this.btnCallTo2nd.Text = "Call Elevator";
            this.btnCallTo2nd.UseVisualStyleBackColor = true;
            this.btnCallTo2nd.Click += new System.EventHandler(this.btnCallTo2nd_Click);
            // 
            // txt1stScreen
            // 
            this.txt1stScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt1stScreen.Enabled = false;
            this.txt1stScreen.Location = new System.Drawing.Point(3, 85);
            this.txt1stScreen.Name = "txt1stScreen";
            this.txt1stScreen.Size = new System.Drawing.Size(315, 20);
            this.txt1stScreen.TabIndex = 5;
            // 
            // btnGoFrom1stTo2nd
            // 
            this.btnGoFrom1stTo2nd.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGoFrom1stTo2nd.Location = new System.Drawing.Point(3, 62);
            this.btnGoFrom1stTo2nd.Name = "btnGoFrom1stTo2nd";
            this.btnGoFrom1stTo2nd.Size = new System.Drawing.Size(315, 23);
            this.btnGoFrom1stTo2nd.TabIndex = 7;
            this.btnGoFrom1stTo2nd.Text = "Go to 2nd Floor";
            this.btnGoFrom1stTo2nd.UseVisualStyleBackColor = true;
            this.btnGoFrom1stTo2nd.Click += new System.EventHandler(this.btnGoFrom1stTo2nd_Click);
            // 
            // btnGoFrom1stTo3rd
            // 
            this.btnGoFrom1stTo3rd.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGoFrom1stTo3rd.Location = new System.Drawing.Point(3, 39);
            this.btnGoFrom1stTo3rd.Name = "btnGoFrom1stTo3rd";
            this.btnGoFrom1stTo3rd.Size = new System.Drawing.Size(315, 23);
            this.btnGoFrom1stTo3rd.TabIndex = 6;
            this.btnGoFrom1stTo3rd.Text = "Go to 3rd Floor";
            this.btnGoFrom1stTo3rd.UseVisualStyleBackColor = true;
            this.btnGoFrom1stTo3rd.Click += new System.EventHandler(this.btnGoFrom1stTo3rd_Click);
            // 
            // btnCallTo1st
            // 
            this.btnCallTo1st.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCallTo1st.Location = new System.Drawing.Point(3, 16);
            this.btnCallTo1st.Name = "btnCallTo1st";
            this.btnCallTo1st.Size = new System.Drawing.Size(315, 23);
            this.btnCallTo1st.TabIndex = 4;
            this.btnCallTo1st.Text = "Call Elevator";
            this.btnCallTo1st.UseVisualStyleBackColor = true;
            this.btnCallTo1st.Click += new System.EventHandler(this.btnCallTo1st_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 359);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Elevator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt1stScreen;
        private System.Windows.Forms.Button btnGoFrom1stTo2nd;
        private System.Windows.Forms.Button btnGoFrom1stTo3rd;
        private System.Windows.Forms.Button btnCallTo1st;
        private System.Windows.Forms.TextBox txt2ndScreen;
        private System.Windows.Forms.Button btnGoDown;
        private System.Windows.Forms.Button btnGoUp;
        private System.Windows.Forms.Button btnCallTo2nd;
        private System.Windows.Forms.TextBox txt3rdScreen;
        private System.Windows.Forms.Button btnGoFrom3rdTo2nd;
        private System.Windows.Forms.Button btnGoFrom3rdTo1st;
        private System.Windows.Forms.Button btnCallTo3rd;
    }
}

