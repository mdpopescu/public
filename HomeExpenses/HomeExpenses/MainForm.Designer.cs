namespace HomeExpenses
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
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.rgvExpenses = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgvExpenses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvExpenses.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.rgvExpenses);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.radPanel1.Location = new System.Drawing.Point(0, 97);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(1170, 394);
            this.radPanel1.TabIndex = 7;
            // 
            // rgvExpenses
            // 
            this.rgvExpenses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rgvExpenses.Location = new System.Drawing.Point(0, 0);
            // 
            // 
            // 
            this.rgvExpenses.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.rgvExpenses.Name = "rgvExpenses";
            this.rgvExpenses.Size = new System.Drawing.Size(1170, 394);
            this.rgvExpenses.TabIndex = 0;
            this.rgvExpenses.Text = "radGridView1";
            this.rgvExpenses.RowsChanged += new Telerik.WinControls.UI.GridViewCollectionChangedEventHandler(this.rgvExpenses_RowsChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 491);
            this.Controls.Add(this.radPanel1);
            this.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MainForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "Home Expenses";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rgvExpenses.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgvExpenses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadGridView rgvExpenses;
    }
}

