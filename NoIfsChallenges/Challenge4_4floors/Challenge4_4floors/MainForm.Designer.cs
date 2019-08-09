namespace Challenge4_4floors
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
            this.grpFloors = new System.Windows.Forms.GroupBox();
            this.grpElevator = new System.Windows.Forms.GroupBox();
            this.rb4thFloor = new System.Windows.Forms.RadioButton();
            this.rb3rdFloor = new System.Windows.Forms.RadioButton();
            this.rb2ndFloor = new System.Windows.Forms.RadioButton();
            this.rb1stFloor = new System.Windows.Forms.RadioButton();
            this.txt4thFloorDisplay = new System.Windows.Forms.TextBox();
            this.txt3rdFloorDisplay = new System.Windows.Forms.TextBox();
            this.txt2ndFloorDisplay = new System.Windows.Forms.TextBox();
            this.txt1stFloorDisplay = new System.Windows.Forms.TextBox();
            this.btn4thFloorCallDown = new System.Windows.Forms.Button();
            this.btn3rdFloorCallDown = new System.Windows.Forms.Button();
            this.btn3rdFloorCallUp = new System.Windows.Forms.Button();
            this.btn2ndFloorCallDown = new System.Windows.Forms.Button();
            this.btn2ndFloorCallUp = new System.Windows.Forms.Button();
            this.btn1stFloorCallUp = new System.Windows.Forms.Button();
            this.btnGoTo1stFloor = new System.Windows.Forms.Button();
            this.btnGoTo2ndFloor = new System.Windows.Forms.Button();
            this.btnGoTo3rdFloor = new System.Windows.Forms.Button();
            this.btnGoTo4thFloor = new System.Windows.Forms.Button();
            this.txtElevatorDisplay = new System.Windows.Forms.TextBox();
            this.grpFloors.SuspendLayout();
            this.grpElevator.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpFloors
            // 
            this.grpFloors.Controls.Add(this.btn1stFloorCallUp);
            this.grpFloors.Controls.Add(this.btn2ndFloorCallDown);
            this.grpFloors.Controls.Add(this.btn2ndFloorCallUp);
            this.grpFloors.Controls.Add(this.btn3rdFloorCallDown);
            this.grpFloors.Controls.Add(this.btn3rdFloorCallUp);
            this.grpFloors.Controls.Add(this.btn4thFloorCallDown);
            this.grpFloors.Controls.Add(this.txt1stFloorDisplay);
            this.grpFloors.Controls.Add(this.txt2ndFloorDisplay);
            this.grpFloors.Controls.Add(this.txt3rdFloorDisplay);
            this.grpFloors.Controls.Add(this.txt4thFloorDisplay);
            this.grpFloors.Controls.Add(this.rb1stFloor);
            this.grpFloors.Controls.Add(this.rb2ndFloor);
            this.grpFloors.Controls.Add(this.rb3rdFloor);
            this.grpFloors.Controls.Add(this.rb4thFloor);
            this.grpFloors.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpFloors.Location = new System.Drawing.Point(0, 0);
            this.grpFloors.Name = "grpFloors";
            this.grpFloors.Size = new System.Drawing.Size(397, 129);
            this.grpFloors.TabIndex = 0;
            this.grpFloors.TabStop = false;
            this.grpFloors.Text = " Floors ";
            // 
            // grpElevator
            // 
            this.grpElevator.Controls.Add(this.txtElevatorDisplay);
            this.grpElevator.Controls.Add(this.btnGoTo4thFloor);
            this.grpElevator.Controls.Add(this.btnGoTo3rdFloor);
            this.grpElevator.Controls.Add(this.btnGoTo2ndFloor);
            this.grpElevator.Controls.Add(this.btnGoTo1stFloor);
            this.grpElevator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpElevator.Location = new System.Drawing.Point(0, 129);
            this.grpElevator.Name = "grpElevator";
            this.grpElevator.Size = new System.Drawing.Size(397, 51);
            this.grpElevator.TabIndex = 1;
            this.grpElevator.TabStop = false;
            this.grpElevator.Text = " Elevator ";
            // 
            // rb4thFloor
            // 
            this.rb4thFloor.AutoSize = true;
            this.rb4thFloor.Enabled = false;
            this.rb4thFloor.Location = new System.Drawing.Point(13, 20);
            this.rb4thFloor.Name = "rb4thFloor";
            this.rb4thFloor.Size = new System.Drawing.Size(66, 17);
            this.rb4thFloor.TabIndex = 0;
            this.rb4thFloor.TabStop = true;
            this.rb4thFloor.Text = "4th Floor";
            this.rb4thFloor.UseVisualStyleBackColor = true;
            // 
            // rb3rdFloor
            // 
            this.rb3rdFloor.AutoSize = true;
            this.rb3rdFloor.Enabled = false;
            this.rb3rdFloor.Location = new System.Drawing.Point(13, 43);
            this.rb3rdFloor.Name = "rb3rdFloor";
            this.rb3rdFloor.Size = new System.Drawing.Size(66, 17);
            this.rb3rdFloor.TabIndex = 1;
            this.rb3rdFloor.TabStop = true;
            this.rb3rdFloor.Text = "3rd Floor";
            this.rb3rdFloor.UseVisualStyleBackColor = true;
            // 
            // rb2ndFloor
            // 
            this.rb2ndFloor.AutoSize = true;
            this.rb2ndFloor.Enabled = false;
            this.rb2ndFloor.Location = new System.Drawing.Point(13, 66);
            this.rb2ndFloor.Name = "rb2ndFloor";
            this.rb2ndFloor.Size = new System.Drawing.Size(69, 17);
            this.rb2ndFloor.TabIndex = 2;
            this.rb2ndFloor.TabStop = true;
            this.rb2ndFloor.Text = "2nd Floor";
            this.rb2ndFloor.UseVisualStyleBackColor = true;
            // 
            // rb1stFloor
            // 
            this.rb1stFloor.AutoSize = true;
            this.rb1stFloor.Enabled = false;
            this.rb1stFloor.Location = new System.Drawing.Point(13, 89);
            this.rb1stFloor.Name = "rb1stFloor";
            this.rb1stFloor.Size = new System.Drawing.Size(65, 17);
            this.rb1stFloor.TabIndex = 3;
            this.rb1stFloor.TabStop = true;
            this.rb1stFloor.Text = "1st Floor";
            this.rb1stFloor.UseVisualStyleBackColor = true;
            // 
            // txt4thFloorDisplay
            // 
            this.txt4thFloorDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt4thFloorDisplay.Location = new System.Drawing.Point(166, 19);
            this.txt4thFloorDisplay.Name = "txt4thFloorDisplay";
            this.txt4thFloorDisplay.ReadOnly = true;
            this.txt4thFloorDisplay.Size = new System.Drawing.Size(225, 20);
            this.txt4thFloorDisplay.TabIndex = 4;
            // 
            // txt3rdFloorDisplay
            // 
            this.txt3rdFloorDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt3rdFloorDisplay.Location = new System.Drawing.Point(166, 42);
            this.txt3rdFloorDisplay.Name = "txt3rdFloorDisplay";
            this.txt3rdFloorDisplay.ReadOnly = true;
            this.txt3rdFloorDisplay.Size = new System.Drawing.Size(225, 20);
            this.txt3rdFloorDisplay.TabIndex = 5;
            // 
            // txt2ndFloorDisplay
            // 
            this.txt2ndFloorDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt2ndFloorDisplay.Location = new System.Drawing.Point(166, 65);
            this.txt2ndFloorDisplay.Name = "txt2ndFloorDisplay";
            this.txt2ndFloorDisplay.ReadOnly = true;
            this.txt2ndFloorDisplay.Size = new System.Drawing.Size(225, 20);
            this.txt2ndFloorDisplay.TabIndex = 6;
            // 
            // txt1stFloorDisplay
            // 
            this.txt1stFloorDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt1stFloorDisplay.Location = new System.Drawing.Point(166, 88);
            this.txt1stFloorDisplay.Name = "txt1stFloorDisplay";
            this.txt1stFloorDisplay.ReadOnly = true;
            this.txt1stFloorDisplay.Size = new System.Drawing.Size(225, 20);
            this.txt1stFloorDisplay.TabIndex = 7;
            // 
            // btn4thFloorCallDown
            // 
            this.btn4thFloorCallDown.Location = new System.Drawing.Point(131, 17);
            this.btn4thFloorCallDown.Name = "btn4thFloorCallDown";
            this.btn4thFloorCallDown.Size = new System.Drawing.Size(28, 23);
            this.btn4thFloorCallDown.TabIndex = 9;
            this.btn4thFloorCallDown.Text = "▼";
            this.btn4thFloorCallDown.UseVisualStyleBackColor = true;
            // 
            // btn3rdFloorCallDown
            // 
            this.btn3rdFloorCallDown.Location = new System.Drawing.Point(131, 40);
            this.btn3rdFloorCallDown.Name = "btn3rdFloorCallDown";
            this.btn3rdFloorCallDown.Size = new System.Drawing.Size(28, 23);
            this.btn3rdFloorCallDown.TabIndex = 11;
            this.btn3rdFloorCallDown.Text = "▼";
            this.btn3rdFloorCallDown.UseVisualStyleBackColor = true;
            // 
            // btn3rdFloorCallUp
            // 
            this.btn3rdFloorCallUp.Location = new System.Drawing.Point(97, 40);
            this.btn3rdFloorCallUp.Name = "btn3rdFloorCallUp";
            this.btn3rdFloorCallUp.Size = new System.Drawing.Size(28, 23);
            this.btn3rdFloorCallUp.TabIndex = 10;
            this.btn3rdFloorCallUp.Text = "▲";
            this.btn3rdFloorCallUp.UseVisualStyleBackColor = true;
            // 
            // btn2ndFloorCallDown
            // 
            this.btn2ndFloorCallDown.Location = new System.Drawing.Point(131, 63);
            this.btn2ndFloorCallDown.Name = "btn2ndFloorCallDown";
            this.btn2ndFloorCallDown.Size = new System.Drawing.Size(28, 23);
            this.btn2ndFloorCallDown.TabIndex = 13;
            this.btn2ndFloorCallDown.Text = "▼";
            this.btn2ndFloorCallDown.UseVisualStyleBackColor = true;
            // 
            // btn2ndFloorCallUp
            // 
            this.btn2ndFloorCallUp.Location = new System.Drawing.Point(97, 63);
            this.btn2ndFloorCallUp.Name = "btn2ndFloorCallUp";
            this.btn2ndFloorCallUp.Size = new System.Drawing.Size(28, 23);
            this.btn2ndFloorCallUp.TabIndex = 12;
            this.btn2ndFloorCallUp.Text = "▲";
            this.btn2ndFloorCallUp.UseVisualStyleBackColor = true;
            // 
            // btn1stFloorCallUp
            // 
            this.btn1stFloorCallUp.Location = new System.Drawing.Point(97, 86);
            this.btn1stFloorCallUp.Name = "btn1stFloorCallUp";
            this.btn1stFloorCallUp.Size = new System.Drawing.Size(28, 23);
            this.btn1stFloorCallUp.TabIndex = 14;
            this.btn1stFloorCallUp.Text = "▲";
            this.btn1stFloorCallUp.UseVisualStyleBackColor = true;
            // 
            // btnGoTo1stFloor
            // 
            this.btnGoTo1stFloor.Location = new System.Drawing.Point(6, 19);
            this.btnGoTo1stFloor.Name = "btnGoTo1stFloor";
            this.btnGoTo1stFloor.Size = new System.Drawing.Size(43, 23);
            this.btnGoTo1stFloor.TabIndex = 0;
            this.btnGoTo1stFloor.Text = "[ 1 ]";
            this.btnGoTo1stFloor.UseVisualStyleBackColor = true;
            // 
            // btnGoTo2ndFloor
            // 
            this.btnGoTo2ndFloor.Location = new System.Drawing.Point(55, 19);
            this.btnGoTo2ndFloor.Name = "btnGoTo2ndFloor";
            this.btnGoTo2ndFloor.Size = new System.Drawing.Size(43, 23);
            this.btnGoTo2ndFloor.TabIndex = 1;
            this.btnGoTo2ndFloor.Text = "[ 2 ]";
            this.btnGoTo2ndFloor.UseVisualStyleBackColor = true;
            // 
            // btnGoTo3rdFloor
            // 
            this.btnGoTo3rdFloor.Location = new System.Drawing.Point(104, 19);
            this.btnGoTo3rdFloor.Name = "btnGoTo3rdFloor";
            this.btnGoTo3rdFloor.Size = new System.Drawing.Size(43, 23);
            this.btnGoTo3rdFloor.TabIndex = 2;
            this.btnGoTo3rdFloor.Text = "[ 3 ]";
            this.btnGoTo3rdFloor.UseVisualStyleBackColor = true;
            // 
            // btnGoTo4thFloor
            // 
            this.btnGoTo4thFloor.Location = new System.Drawing.Point(153, 19);
            this.btnGoTo4thFloor.Name = "btnGoTo4thFloor";
            this.btnGoTo4thFloor.Size = new System.Drawing.Size(43, 23);
            this.btnGoTo4thFloor.TabIndex = 3;
            this.btnGoTo4thFloor.Text = "[ 4 ]";
            this.btnGoTo4thFloor.UseVisualStyleBackColor = true;
            // 
            // txtElevatorDisplay
            // 
            this.txtElevatorDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtElevatorDisplay.Location = new System.Drawing.Point(202, 21);
            this.txtElevatorDisplay.Name = "txtElevatorDisplay";
            this.txtElevatorDisplay.ReadOnly = true;
            this.txtElevatorDisplay.Size = new System.Drawing.Size(189, 20);
            this.txtElevatorDisplay.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 180);
            this.Controls.Add(this.grpElevator);
            this.Controls.Add(this.grpFloors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Challenge 4: Elevator (4 floors)";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.grpFloors.ResumeLayout(false);
            this.grpFloors.PerformLayout();
            this.grpElevator.ResumeLayout(false);
            this.grpElevator.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpFloors;
        private System.Windows.Forms.GroupBox grpElevator;
        private System.Windows.Forms.RadioButton rb1stFloor;
        private System.Windows.Forms.RadioButton rb2ndFloor;
        private System.Windows.Forms.RadioButton rb3rdFloor;
        private System.Windows.Forms.RadioButton rb4thFloor;
        private System.Windows.Forms.TextBox txt1stFloorDisplay;
        private System.Windows.Forms.TextBox txt2ndFloorDisplay;
        private System.Windows.Forms.TextBox txt3rdFloorDisplay;
        private System.Windows.Forms.TextBox txt4thFloorDisplay;
        private System.Windows.Forms.Button btn4thFloorCallDown;
        private System.Windows.Forms.Button btn1stFloorCallUp;
        private System.Windows.Forms.Button btn2ndFloorCallDown;
        private System.Windows.Forms.Button btn2ndFloorCallUp;
        private System.Windows.Forms.Button btn3rdFloorCallDown;
        private System.Windows.Forms.Button btn3rdFloorCallUp;
        private System.Windows.Forms.TextBox txtElevatorDisplay;
        private System.Windows.Forms.Button btnGoTo4thFloor;
        private System.Windows.Forms.Button btnGoTo3rdFloor;
        private System.Windows.Forms.Button btnGoTo2ndFloor;
        private System.Windows.Forms.Button btnGoTo1stFloor;
    }
}

