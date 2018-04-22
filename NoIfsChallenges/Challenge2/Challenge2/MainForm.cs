using System;
using System.Windows.Forms;
using Challenge2.Library.Contracts;
using Challenge2.Library.Services;

namespace Challenge2
{
    public partial class MainForm : Form, UserInterface
    {
        public bool StartStopEnabled
        {
            set => btnStartStop.Enabled = value;
        }

        public bool ResetEnabled
        {
            set => btnReset.Enabled = value;
        }

        public bool HoldEnabled
        {
            set => btnHold.Enabled = value;
        }

        public string Display
        {
            set => lblClock.Text = value;
        }

        public MainForm()
        {
            InitializeComponent();

            watch = new Watch(this);
        }

        //

        private readonly Watch watch;

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            watch.Initialize();
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            watch.StartStop();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            watch.Reset();
        }

        private void btnHold_Click(object sender, EventArgs e)
        {
            watch.Hold();
        }
    }
}