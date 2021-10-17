using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Disposables;
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

            var startStopClicked = btnStartStop.GetClicks().CreateLog("startStopClicked").Share();
            var resetClicked = btnReset.GetClicks().CreateLog("resetClicked").Share();
            var holdClicked = btnHold.GetClicks().CreateLog("holdClicked").Share();

            var stopClicked = startStopClicked.Buffer(2).AsUnit().CreateLog("stopClicked");

            var ssEnabledFalse = stopClicked.AsConst(false).CreateLog("ssEnabledFalse");
            var ssEnabledTrue = resetClicked.AsConst(true).CreateLog("ssEnabledTrue");
            var ssEnabled = ssEnabledFalse.Merge(ssEnabledTrue).StartWith(true).CreateLog("ssEnabled");

            var resetEnabledTrue = stopClicked.AsConst(true).CreateLog("resetEnabledTrue");
            var resetEnabledFalse = resetClicked.AsConst(false).CreateLog("resetEnabledFalse");
            var resetEnabled = resetEnabledTrue.Merge(resetEnabledFalse).StartWith(false).CreateLog("resetEnabled");

            var holdEnabled = startStopClicked.Toggle(false).CreateLog("holdEnabled");
            var clearHold = resetClicked.AsUnit().StartWith(Unit.Default).CreateLog("clearHold");
            var shouldDisplay = clearHold.SwitchMap(_ => holdClicked.Toggle(true)).Share().CreateLog("shouldDisplay");

            var timer = startStopClicked.Toggle(false).WhenTrue(() => StartTimer().CreateLog("startTimer").Share()).CreateLog("timer");

            var timerUpdate = timer.CombineLatest(shouldDisplay).Where(it => it.Item2).Select(it => it.Item1).CreateLog("timerUpdate");
            var timerReset = resetClicked.Select(_ => TimeSpan.Zero).CreateLog("timerReset");
            var timerDisplay = timerUpdate.Merge(timerReset).Select(value => value.ToString()).CreateLog("timerDisplay");

            var s1 = btnStartStop.HandleChanges(ssEnabled, InternalSetEnabled);
            var s2 = btnReset.HandleChanges(resetEnabled, InternalSetEnabled);
            var s3 = btnHold.HandleChanges(holdEnabled, InternalSetEnabled);

            var s4 = lblClock.HandleChanges(timerDisplay, InternalSetText);

            subscription = new CompositeDisposable(s1, s2, s3, s4);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            subscription.Dispose();

            var timeline = string.Join(Environment.NewLine, ObservableLog.GetTimeline());
            Console.WriteLine(timeline);

            base.OnFormClosed(e);
        }

        //

        private static readonly TimeSpan SECOND = TimeSpan.FromSeconds(1);

        private IDisposable subscription;

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