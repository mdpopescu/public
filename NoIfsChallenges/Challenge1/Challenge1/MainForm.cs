using System;
using System.Windows.Forms;
using Challenge1.Library.Contracts;
using Challenge1.Library.Shell;

namespace Challenge1
{
    public partial class MainForm : Form, MainUI
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void Display(string s)
        {
            txtDisplay.Text = s;
        }

        //

        private MainLogic mainLogic;

        private void MainForm_Load(object sender, EventArgs e)
        {
            mainLogic = new MainLogic(this);
        }

        private void EnterDigit(object sender, EventArgs e)
        {
            mainLogic.EnterDigit(((Button) sender).Text[0]);
        }

        private void EnterOperator(object sender, EventArgs e)
        {
            mainLogic.EnterOperator(((Button) sender).Text[0]);
        }

        private void EnterEqual(object sender, EventArgs e)
        {
            mainLogic.EnterEqual();
        }
    }
}