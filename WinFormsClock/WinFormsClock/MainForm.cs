using System;
using System.Windows.Forms;

namespace WinFormsClock
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //

        //

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}