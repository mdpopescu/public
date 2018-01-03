using System;
using System.Windows.Forms;
using CQRS3.Contracts;
using CQRS3.Implementations;

namespace CQRS3
{
    public partial class MainForm : Form, MainUI
    {
        public MainForm()
        {
            InitializeComponent();

            logic = new MainLogic(this);
        }

        public void ShowValue(int value)
        {
            lblCurrentValue.Text = $"Current value: {value}";
        }

        public void ShowStatus(string status)
        {
            lblStatus.Text = status;
        }

        //

        private readonly MainLogic logic;

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            //
        }

        private void btnIncrement_Click(object sender, EventArgs e)
        {
            logic.Increment();
        }

        private void btnDecrement_Click(object sender, EventArgs e)
        {
            logic.Decrement();
        }
    }
}