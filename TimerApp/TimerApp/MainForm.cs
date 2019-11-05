using System;
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

        public void SetButtonText(string text)
        {
            btnStartStop.Text = text;
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