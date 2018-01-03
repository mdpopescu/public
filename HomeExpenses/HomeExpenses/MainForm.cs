using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace HomeExpenses
{
    public partial class MainForm : RadForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //

        private readonly HomeEntities db = new HomeEntities();

        private void SetupGrid()
        {
            rgvExpenses.AutoGenerateColumns = false;

            AddDateColumn("Date", "Data", 100);
            AddTextColumn("Amount", "Suma", 80);
            AddTextColumn("Description", "Scopul cheltuielii", 400);
            AddComboColumn("Category", "Categoria", 150, db.Categories.Local.ToBindingList());
        }

        private void AddDateColumn(string name, string header, int width)
        {
            rgvExpenses.Columns.Add(new GridViewDateTimeColumn(name) { HeaderText = header, TextAlignment = ContentAlignment.MiddleCenter, MinWidth = width });
        }

        private void AddTextColumn(string name, string header, int width)
        {
            rgvExpenses.Columns.Add(new GridViewTextBoxColumn(name) { HeaderText = header, TextAlignment = ContentAlignment.MiddleRight, MinWidth = width });
        }

        private void AddComboColumn(string name, string header, int width, BindingList<Categories> values)
        {
            rgvExpenses.Columns.Add(
                new GridViewComboBoxColumn(name)
                {
                    HeaderText = header,
                    TextAlignment = ContentAlignment.MiddleCenter,
                    MinWidth = width,
                    DropDownStyle = RadDropDownStyle.DropDown,
                    DataSource = values,
                    DisplayMember = "Category",
                    ValueMember = "Category",
                });
        }

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupGrid();
            rgvExpenses.DataSource = db.Expenses.Local.ToBindingList();
        }

        private void rgvExpenses_RowsChanged(object sender, GridViewCollectionChangedEventArgs e)
        {
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(ex.Message);
            }
        }
    }
}