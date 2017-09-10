using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Pomodoro
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //

        private TimeSpan remaining;

        private bool IsRunning => timer.Enabled;

        private TimeSpan SelectedDuration =>
            rb10min.Checked
                ? TimeSpan.FromMinutes(10)
                : rb20min.Checked
                    ? TimeSpan.FromMinutes(20)
                    : TimeSpan.FromMinutes(30);

        private void Minimize()
        {
            notifyIcon.Visible = true;
            Hide();
        }

        private void Restore()
        {
            notifyIcon.Visible = false;
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void Start()
        {
            remaining = SelectedDuration;

            rb10min.Enabled = rb20min.Enabled = rb30min.Enabled = false;

            btnStart.Text = "Stop";
            btnStart.ForeColor = Color.Red;
            timer.Enabled = true;
        }

        private void Stop()
        {
            if (WindowState == FormWindowState.Minimized)
                Restore();

            rb10min.Enabled = rb20min.Enabled = rb30min.Enabled = true;

            timer.Enabled = false;
            btnStart.Text = "Start";
            btnStart.ForeColor = SystemColors.ControlText;

            lblRemaining.Text = "00:00";
        }

        private static void SoundAlarm()
        {
            using (var player = new SoundPlayer())
            {
                player.SoundLocation = @"Media\ring.wav";
                player.Play();
            }
        }

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            //
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                Minimize();
            else
                Restore();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblRemaining.Text = remaining.ToString(@"mm\:ss");
            notifyIcon.Text = remaining.ToString(@"mm\:ss");

            remaining = remaining.Subtract(TimeSpan.FromSeconds(1));
            if (remaining > TimeSpan.Zero)
                return;

            SoundAlarm();
            Stop();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "Start")
                Start();
            else
                Stop();
        }

        private void mnuRestore_Click(object sender, EventArgs e)
        {
            Restore();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (IsRunning && MessageBox.Show("Are you sure you want to exit?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Close();
        }
    }
}

// Using image from https://pixabay.com/en/alarm-clock-clock-time-wake-up-155187/
// Using sound file from http://www.orangefreesounds.com/short-ringtone-for-notification/