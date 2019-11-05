using System;
using System.Drawing;
using System.Windows.Forms;
using TimerApp.Contracts;
using TimerApp.Services;

namespace TimerApp
{
    public partial class MainForm : Form, UserInterface
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void ChangeButtonToStart()
        {
            btnStartStop.Text = "Start";
            btnStartStop.BackColor = Color.Lime;
        }

        public void ChangeButtonToStop()
        {
            btnStartStop.Text = "Stop";
            btnStartStop.BackColor = Color.Red;
        }

        public void ChangeButtonToReset()
        {
            btnStartStop.Text = "Reset";
            btnStartStop.BackColor = Color.Yellow;
        }

        public void ShowTime(TimeSpan ts)
        {
            lblElapsedTime.Text = ts.ToString(@"hh\:mm\:ss\.fff");
        }

        //

        private TimerState state;

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            state = new TimerStopped(this);
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            state = state.StartStop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            state.DisplayTime();
        }
    }
}