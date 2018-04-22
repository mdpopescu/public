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
            set => this.UIChange(() => btnStartStop.Enabled = value);
        }

        public bool ResetEnabled
        {
            set => this.UIChange(() => btnReset.Enabled = value);
        }

        public bool HoldEnabled
        {
            set => this.UIChange(() => btnHold.Enabled = value);
        }

        public string Display
        {
            set => this.UIChange(() => lblClock.Text = value);
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