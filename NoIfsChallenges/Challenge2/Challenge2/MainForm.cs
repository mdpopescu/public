using System;
using System.Drawing;
using System.Windows.Forms;
using Challenge2.Helpers;
using Challenge2.Library.Contracts;
using Challenge2.Library.Services;
using Challenge2.Services;

namespace Challenge2
{
    public partial class MainForm : Form, UserInterface
    {
        public MainForm()
        {
            InitializeComponent();

            watch = new Watch(new SafeUI(this, this.UIChange));
        }

        public void EnableStartStop() => Enable(btnStartStop);
        public void DisableStartStop() => Disable(btnStartStop);

        public void EnableReset() => Enable(btnReset);
        public void DisableReset() => Disable(btnReset);

        public void EnableHold() => Enable(btnHold);
        public void DisableHold() => Disable(btnHold);

        public void Display(string text) => lblClock.Text = text;

        //

        private readonly Watch watch;

        private static void Enable(Control control)
        {
            control.Enabled = true;
            control.BackColor = Color.Lime;
        }

        private static void Disable(Control control)
        {
            control.Enabled = false;
            control.BackColor = SystemColors.Control;
        }

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