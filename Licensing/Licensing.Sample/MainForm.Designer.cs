namespace Renfield.Licensing.Sample
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
      this.cbIsLicensed = new System.Windows.Forms.CheckBox();
      this.cbIsTrial = new System.Windows.Forms.CheckBox();
      this.cbShouldRun = new System.Windows.Forms.CheckBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.txtCreatedOn = new System.Windows.Forms.TextBox();
      this.txtLimitsDays = new System.Windows.Forms.TextBox();
      this.txtLimitsRuns = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.txtKey = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.txtName = new System.Windows.Forms.TextBox();
      this.txtContact = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.txtProcessorId = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.txtExpiration = new System.Windows.Forms.TextBox();
      this.label8 = new System.Windows.Forms.Label();
      this.btnLoad = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // cbIsLicensed
      // 
      this.cbIsLicensed.AutoSize = true;
      this.cbIsLicensed.Enabled = false;
      this.cbIsLicensed.Location = new System.Drawing.Point(13, 13);
      this.cbIsLicensed.Name = "cbIsLicensed";
      this.cbIsLicensed.Size = new System.Drawing.Size(80, 17);
      this.cbIsLicensed.TabIndex = 0;
      this.cbIsLicensed.Text = "Is Licensed";
      this.cbIsLicensed.UseVisualStyleBackColor = true;
      // 
      // cbIsTrial
      // 
      this.cbIsTrial.AutoSize = true;
      this.cbIsTrial.Enabled = false;
      this.cbIsTrial.Location = new System.Drawing.Point(118, 13);
      this.cbIsTrial.Name = "cbIsTrial";
      this.cbIsTrial.Size = new System.Drawing.Size(57, 17);
      this.cbIsTrial.TabIndex = 1;
      this.cbIsTrial.Text = "Is Trial";
      this.cbIsTrial.UseVisualStyleBackColor = true;
      // 
      // cbShouldRun
      // 
      this.cbShouldRun.AutoSize = true;
      this.cbShouldRun.Enabled = false;
      this.cbShouldRun.Location = new System.Drawing.Point(205, 13);
      this.cbShouldRun.Name = "cbShouldRun";
      this.cbShouldRun.Size = new System.Drawing.Size(82, 17);
      this.cbShouldRun.TabIndex = 2;
      this.cbShouldRun.Text = "Should Run";
      this.cbShouldRun.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.btnSave);
      this.groupBox1.Controls.Add(this.btnLoad);
      this.groupBox1.Controls.Add(this.txtExpiration);
      this.groupBox1.Controls.Add(this.label8);
      this.groupBox1.Controls.Add(this.txtProcessorId);
      this.groupBox1.Controls.Add(this.label7);
      this.groupBox1.Controls.Add(this.txtContact);
      this.groupBox1.Controls.Add(this.label6);
      this.groupBox1.Controls.Add(this.txtName);
      this.groupBox1.Controls.Add(this.label5);
      this.groupBox1.Controls.Add(this.txtKey);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.txtLimitsRuns);
      this.groupBox1.Controls.Add(this.txtLimitsDays);
      this.groupBox1.Controls.Add(this.txtCreatedOn);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Location = new System.Drawing.Point(-2, 36);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(616, 213);
      this.groupBox1.TabIndex = 4;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = " License Details ";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 28);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(61, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Created On";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(248, 28);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(63, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Limits: Days";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(398, 28);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(64, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Limits: Runs";
      // 
      // txtCreatedOn
      // 
      this.txtCreatedOn.Location = new System.Drawing.Point(73, 25);
      this.txtCreatedOn.Name = "txtCreatedOn";
      this.txtCreatedOn.Size = new System.Drawing.Size(154, 20);
      this.txtCreatedOn.TabIndex = 3;
      // 
      // txtLimitsDays
      // 
      this.txtLimitsDays.Location = new System.Drawing.Point(317, 25);
      this.txtLimitsDays.Name = "txtLimitsDays";
      this.txtLimitsDays.Size = new System.Drawing.Size(60, 20);
      this.txtLimitsDays.TabIndex = 4;
      // 
      // txtLimitsRuns
      // 
      this.txtLimitsRuns.Location = new System.Drawing.Point(468, 25);
      this.txtLimitsRuns.Name = "txtLimitsRuns";
      this.txtLimitsRuns.Size = new System.Drawing.Size(60, 20);
      this.txtLimitsRuns.TabIndex = 5;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(12, 57);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(25, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Key";
      // 
      // txtKey
      // 
      this.txtKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtKey.Location = new System.Drawing.Point(72, 54);
      this.txtKey.Multiline = true;
      this.txtKey.Name = "txtKey";
      this.txtKey.Size = new System.Drawing.Size(531, 83);
      this.txtKey.TabIndex = 7;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(14, 155);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(35, 13);
      this.label5.TabIndex = 8;
      this.label5.Text = "Name";
      // 
      // txtName
      // 
      this.txtName.Location = new System.Drawing.Point(71, 152);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(155, 20);
      this.txtName.TabIndex = 9;
      // 
      // txtContact
      // 
      this.txtContact.Location = new System.Drawing.Point(71, 180);
      this.txtContact.Name = "txtContact";
      this.txtContact.Size = new System.Drawing.Size(155, 20);
      this.txtContact.TabIndex = 11;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(14, 183);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(44, 13);
      this.label6.TabIndex = 10;
      this.label6.Text = "Contact";
      // 
      // txtProcessorId
      // 
      this.txtProcessorId.Location = new System.Drawing.Point(322, 149);
      this.txtProcessorId.Name = "txtProcessorId";
      this.txtProcessorId.Size = new System.Drawing.Size(155, 20);
      this.txtProcessorId.TabIndex = 13;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(253, 152);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(63, 13);
      this.label7.TabIndex = 12;
      this.label7.Text = "ProcessorId";
      // 
      // txtExpiration
      // 
      this.txtExpiration.Location = new System.Drawing.Point(322, 180);
      this.txtExpiration.Name = "txtExpiration";
      this.txtExpiration.Size = new System.Drawing.Size(155, 20);
      this.txtExpiration.TabIndex = 15;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(253, 183);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(53, 13);
      this.label8.TabIndex = 14;
      this.label8.Text = "Expiration";
      // 
      // btnLoad
      // 
      this.btnLoad.Location = new System.Drawing.Point(528, 150);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(75, 23);
      this.btnLoad.TabIndex = 16;
      this.btnLoad.Text = "Load";
      this.btnLoad.UseVisualStyleBackColor = true;
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
      // 
      // btnSave
      // 
      this.btnSave.Location = new System.Drawing.Point(528, 178);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 17;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(613, 248);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.cbShouldRun);
      this.Controls.Add(this.cbIsTrial);
      this.Controls.Add(this.cbIsLicensed);
      this.Name = "MainForm";
      this.Text = "Licensing Sample";
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox cbIsLicensed;
    private System.Windows.Forms.CheckBox cbIsTrial;
    private System.Windows.Forms.CheckBox cbShouldRun;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtLimitsRuns;
    private System.Windows.Forms.TextBox txtLimitsDays;
    private System.Windows.Forms.TextBox txtCreatedOn;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtContact;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox txtKey;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txtProcessorId;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox txtExpiration;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnLoad;
  }
}

