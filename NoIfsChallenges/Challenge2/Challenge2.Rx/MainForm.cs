using System;
using System.Diagnostics.CodeAnalysis;
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

            // external events

            var startStopClicked = btnStartStop.GetClicks().CreateLog("startStopClicked").Share();
            var resetClicked = btnReset.GetClicks().CreateLog("resetClicked").Share();
            var holdClicked = btnHold.GetClicks().CreateLog("holdClicked").Share();
            IObservable<TimeSpan> CreateTimer() => Observable.Interval(SECOND).StartWith(0).Select(value => TimeSpan.FromSeconds(value)).CreateLog("timerValue").Share();

            // derived events

            var startClicked = startStopClicked.Toggle(false).Where(it => it).AsUnit().CreateLog("startClicked");
            var stopClicked = startStopClicked.Buffer(2).AsUnit().CreateLog("stopClicked");

            var ssEnabledFalse = stopClicked.AsConst(false).CreateLog("ssEnabledFalse");
            var ssEnabledTrue = resetClicked.AsConst(true).CreateLog("ssEnabledTrue");
            var ssEnabled = ssEnabledFalse.Merge(ssEnabledTrue).StartWith(true).CreateLog("ssEnabled");

            var resetEnabledTrue = stopClicked.AsConst(true).CreateLog("resetEnabledTrue");
            var resetEnabledFalse = resetClicked.AsConst(false).CreateLog("resetEnabledFalse");
            var resetEnabled = resetEnabledTrue.Merge(resetEnabledFalse).StartWith(false).CreateLog("resetEnabled");

            var holdEnabled = startClicked.CreateLog("holdEnabled");

            var unfreeze = holdClicked.Buffer(2).AsUnit().CreateLog("unfreeze");
            var displayEnabled = startClicked.Merge(unfreeze).CreateLog("displayEnabled");

            var timer = startClicked.SwitchMap(_ => CreateTimer().TakeUntil(stopClicked).CreateLog("CreateTimer")).CreateLog("timer");
            var display = timer.Select(it => it.ToString()).CreateLog("display");

            //var clearHold = resetClicked.StartWith(Unit.Default).CreateLog("clearHold");
            //var shouldDisplay = clearHold.SwitchMap(_ => holdClicked.Toggle(true)).Share().CreateLog("shouldDisplay");

            //var timerRunning = timer.CombineLatest(shouldDisplay).Where(it => it.Item2).Select(it => it.Item1).CreateLog("timerRunning");
            //var timerReset = resetClicked.Select(_ => TimeSpan.Zero).CreateLog("timerReset");
            //var displayActive = timerRunning.Merge(timerReset).Select(value => value.ToString()).CreateLog("displayActive");

            var s1 = btnStartStop.HandleChanges(ssEnabled, InternalSetEnabled);
            var s2 = btnReset.HandleChanges(resetEnabled, InternalSetEnabled);
            var s3 = btnHold.HandleChanges(holdEnabled, InternalSetEnabled);

            var s4 = lblClock.HandleChanges(display, InternalSetText);

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