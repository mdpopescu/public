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

            var startStopClicked = ObservableLog.Create("startStopClicked", () => btnStartStop.GetClicks()).Share();
            var resetClicked = ObservableLog.Create("resetClicked", () => btnReset.GetClicks()).Share();
            var holdClicked = ObservableLog.Create("holdClicked", () => btnHold.GetClicks()).Share();

            var ssEnabledFalse = ObservableLog.Create("ssEnabledFalse", () => startStopClicked.Buffer(2).AsConst(false)).Share();
            var ssEnabledTrue = ObservableLog.Create("ssEnabledTrue", () => resetClicked.AsConst(true)).Share();
            var ssEnabled = ObservableLog.Create("ssEnabled", () => ssEnabledFalse.Merge(ssEnabledTrue).StartWith(true)).Share();

            var resetEnabledTrue = ObservableLog.Create("resetEnabledTrue", () => startStopClicked.Buffer(2).AsConst(true)).Share();
            var resetEnabledFalse = ObservableLog.Create("resetEnabledFalse", () => resetClicked.AsConst(false)).Share();
            var resetEnabled = ObservableLog.Create("resetEnabled", () => resetEnabledTrue.Merge(resetEnabledFalse).StartWith(false)).Share();

            var holdEnabled = ObservableLog.Create("holdEnabled", () => startStopClicked.Toggle(false)).Share();
            var clearHold = ObservableLog.Create("clearHold", () => resetClicked.AsUnit().StartWith(Unit.Default)).Share();
            var shouldDisplay = ObservableLog.Create("shouldDisplay", () => clearHold.SwitchMap(_ => holdClicked.Toggle(true)).Share()).Share();

            var timer = ObservableLog.Create("timer", () => startStopClicked.Toggle(false).WhenTrue(StartTimer)).Share();
            var timerUpdate = ObservableLog.Create("timerUpdate", () => timer.CombineLatest(shouldDisplay).Where(it => it.Item2).Select(it => it.Item1)).Share();
            var timerReset = ObservableLog.Create("timerReset", () => resetClicked.Select(_ => TimeSpan.Zero)).Share();
            var timerDisplay = ObservableLog.Create("timerDisplay", () => timerUpdate.Merge(timerReset).Select(value => value.ToString())).Share();

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