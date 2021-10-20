using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Forms;
using Challenge2.Rx.Helpers;
using Challenge2.Rx.Models;

namespace Challenge2.Rx
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var state = new DisplayState();

            var startStopClicks = btnStartStop
                .GetClicks()
                .Do(_ => state.StartStop());
            var resetClicks = btnReset
                .GetClicks()
                .Do(_ => state.Reset());
            var holdClicks = btnHold
                .GetClicks()
                .Do(_ => state.Hold());
            var updates = Observable
                .Interval(REFRESH_RATE)
                .AsUnit();

            var events = Observable.Merge(startStopClicks, resetClicks, holdClicks, updates);

            subscription = events
                .Select(_ => state)
                .StartWith(state)
                .ObserveOn(this)
                .Subscribe(UpdateDisplay);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            subscription.Dispose();

            var timeline = string.Join(Environment.NewLine, ObservableLog.GetTimeline());
            Console.WriteLine(timeline);

            base.OnFormClosed(e);
        }

        //

        private static readonly TimeSpan REFRESH_RATE = TimeSpan.FromSeconds(0.25);

        private IDisposable subscription;

        private void UpdateDisplay(DisplayState state)
        {
            InternalSetEnabled(btnStartStop, state.StartStopEnabled);
            InternalSetEnabled(btnReset, state.ResetEnabled);
            InternalSetEnabled(btnHold, state.HoldEnabled);

            InternalSetText(lblClock, state.TimerDisplay);
        }

        private static void InternalSetEnabled(Control control, bool value)
        {
            if (value)
            {
                control.Enabled = true;
                control.BackColor = Color.Lime;
            }
            else
            {
                control.Enabled = false;
                control.BackColor = SystemColors.Control;
            }
        }

        private static void InternalSetText(Control control, string text) => control.Text = text;
    }
}