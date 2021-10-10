using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using Challenge2.Rx.Helpers;

namespace Challenge2.Rx
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void EnableStartStop() => Enable(btnStartStop);
        public void DisableStartStop() => Disable(btnStartStop);

        public void EnableReset() => Enable(btnReset);
        public void DisableReset() => Disable(btnReset);

        public void EnableHold() => Enable(btnHold);
        public void DisableHold() => Disable(btnHold);

        public void Display(string text) => lblClock.Text = text;

        //

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            EnableStartStop();
            DisableReset();
            DisableHold();
            Display("00:00:00");

            startStopClicked = btnStartStop.GetClicks();
            resetClicked = btnReset.GetClicks();
            holdClicked = btnHold.GetClicks();

            var btnHoldEnabled = startStopClicked
                .Toggle(false);

            // timer
            startStopClicked
                .Toggle(false)
                .SelectSwitch(running => running ? Observable.Interval(TimeSpan.FromSeconds(1)) : Observable.Never<long>())
                .Select(value => TimeSpan.FromSeconds(value + 1))
                .Select(value => value.ToString("hh\\:mm\\:ss"))
                .SetText(lblClock);

            btnHoldEnabled.SetEnabled(btnHold);
        }

        //

        private IObservable<Unit> startStopClicked, resetClicked, holdClicked;

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
    }
}