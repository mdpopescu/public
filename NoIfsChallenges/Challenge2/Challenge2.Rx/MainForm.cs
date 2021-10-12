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

            var startStopClicked = btnStartStop.GetClicks();
            var resetClicked = btnReset.GetClicks();
            var holdClicked = btnHold.GetClicks();

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

            var clearHold = resetClicked
                .AsUnit()
                .StartWith(Unit.Default);
            var shouldDisplay = clearHold
                .SelectSwitch(_ => holdClicked.Toggle(true));

            var timer = startStopClicked
                .Toggle(false)
                .WhenTrue(StartTimer)
                .Select(value => value.ToString("hh\\:mm\\:ss"))
                .StartWith("00:00:00");

            var timerUpdate = timer
                .CombineLatest(shouldDisplay, (value, enabled) => (value, enabled))
                .Where(it => it.enabled)
                .Select(it => it.value);
            var timerReset = resetClicked
                .Select(_ => "00:00:00");
            var timerDisplay = timerUpdate.Merge(timerReset);

            btnStartStop.SetEnabled(ssEnabled);
            btnReset.SetEnabled(resetEnabled);
            btnHold.SetEnabled(holdEnabled);

            lblClock.SetText(timerDisplay);
        }

        //

        private static readonly TimeSpan SECOND = TimeSpan.FromSeconds(1);

        private static IObservable<TimeSpan> StartTimer() =>
            Observable.Interval(SECOND).Select(value => TimeSpan.FromSeconds(value + 1));
    }
}