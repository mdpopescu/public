using System;
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

            startStopClicked = btnStartStop.GetClicks();
            resetClicked = btnReset.GetClicks();
            holdClicked = btnHold.GetClicks();

            var ssEnabledFalse = startStopClicked
                .Buffer(2)
                .Select(_ => false);
            var ssEnabledTrue = resetClicked
                .Select(_ => true);
            var ssEnabled = ssEnabledFalse
                .Merge(ssEnabledTrue)
                .StartWith(true);

            var resetEnabledTrue = startStopClicked
                .Buffer(2)
                .Select(_ => true);
            var resetEnabledFalse = resetClicked
                .Select(_ => false);
            var resetEnabled = resetEnabledTrue
                .Merge(resetEnabledFalse)
                .StartWith(false);

            var holdEnabled = startStopClicked
                .Toggle(false);

            // TODO: if I hit STOP when HOLD is in effect, HOLD remains "stuck" until clicked again; it needs to restart
            var shouldDisplay = holdClicked
                .Toggle(true);

            // timer
            var timer = startStopClicked
                .Toggle(false)
                .WhenTrue(StartTimer)
                .Select(value => value.ToString("hh\\:mm\\:ss"))
                .StartWith("00:00:00");

            var timerDisplay = timer
                .CombineLatest(shouldDisplay, Tuple.Create)
                .Where(it => it.Item2)
                .Select(it => it.Item1);

            btnStartStop.SetEnabled(ssEnabled);
            btnReset.SetEnabled(resetEnabled);
            btnHold.SetEnabled(holdEnabled);

            // TODO: reset the timer when RESET is clicked
            lblClock.SetText(timerDisplay);
        }

        //

        private static readonly TimeSpan SECOND = TimeSpan.FromSeconds(1);

        private IObservable<Unit> startStopClicked, resetClicked, holdClicked;

        private static IObservable<TimeSpan> StartTimer() =>
            Observable.Interval(SECOND).Select(value => TimeSpan.FromSeconds(value + 1));
    }
}