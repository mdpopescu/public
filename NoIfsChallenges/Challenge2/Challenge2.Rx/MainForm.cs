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

        //

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var startStopClicked = btnStartStop.GetClicks();
            var resetClicked = btnReset.GetClicks();
            var holdClicked = btnHold.GetClicks();

            var ssEnabledFalse = startStopClicked.Buffer(2).AsConst(false);
            var ssEnabledTrue = resetClicked.AsConst(true);
            var ssEnabled = ssEnabledFalse.Merge(ssEnabledTrue).StartWith(true);

            var resetEnabledTrue = startStopClicked.Buffer(2).AsConst(true);
            var resetEnabledFalse = resetClicked.AsConst(false);
            var resetEnabled = resetEnabledTrue.Merge(resetEnabledFalse).StartWith(false);

            var holdEnabled = startStopClicked.Toggle(false);

            var timer = startStopClicked.Toggle(false).WhenTrue(StartTimer);

            var clearHold = resetClicked.AsUnit().StartWith(Unit.Default);
            var shouldDisplay = clearHold.SwitchMap(_ => holdClicked.Toggle(true)).Share();

            var timerUpdate = timer
                .CombineLatest(shouldDisplay)
                .Where(it => it.Item2)
                .Select(it => it.Item1);
            var timerReset = resetClicked
                .Select(_ => TimeSpan.Zero);
            var timerDisplay = timerUpdate
                .Merge(timerReset)
                .Select(value => value.ToString("hh\\:mm\\:ss"))
                .StartWith("00:00:00");

            btnStartStop.HandleChanges(ssEnabled, InternalSetEnabled);
            btnReset.HandleChanges(resetEnabled, InternalSetEnabled);
            btnHold.HandleChanges(holdEnabled, InternalSetEnabled);

            lblClock.HandleChanges(timerDisplay, InternalSetText);
        }

        //

        private static readonly TimeSpan SECOND = TimeSpan.FromSeconds(1);

        private static IObservable<TimeSpan> StartTimer() =>
            Observable.Interval(SECOND).Select(value => TimeSpan.FromSeconds(value + 1));

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