using System;
using System.Drawing;
using System.Windows.Forms;
using Challenge2.Library.Contracts;
using Challenge2.Library.Services;

namespace Challenge2
{
    public partial class MainForm : Form, UserInterface
    {
        public bool StartStopEnabled
        {
            set => this.UIChange(
                () =>
                {
                    btnStartStop.Enabled = value;
                    btnStartStop.BackColor = GetColor(value);
                });
        }

        public bool ResetEnabled
        {
            set => this.UIChange(
                () =>
                {
                    btnReset.Enabled = value;
                    btnReset.BackColor = GetColor(value);
                });
        }

        public bool HoldEnabled
        {
            set => this.UIChange(
                () =>
                {
                    btnHold.Enabled = value;
                    btnHold.BackColor = GetColor(value);
                });
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

        private static Color GetColor(bool value) => value ? Color.Lime : SystemColors.Control;

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