using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Concurrency;
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

            var scheduler = new ControlScheduler(this);

            var startStopClicked = GetClicks(btnStartStop).AsUnit();
            var resetClicked = GetClicks(btnReset).AsUnit();
            var holdClicked = GetClicks(btnHold).AsUnit();

            holdEnabled = startStopClicked.Toggle(false);

            var timer = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Publish()
                .RefCount();
            var timerValue = startStopClicked
                .Toggle(false)
                .Do(enabled => Console.WriteLine($"Enabled: {enabled}"))
                .Scan(0, (oldValue, enabled) => enabled ? oldValue + 1 : oldValue)
                .StartWith(0)
                .Do(value => Console.WriteLine($"Current value: {value}"))
                .Select(value => TimeSpan.FromSeconds(value).ToString("hh\\:mm\\:ss"));
            var timerShouldUpdate = holdClicked.Toggle(true);
            timerDisplay = from _ in timer
                           from value in timerValue
                           from active in timerShouldUpdate
                           where active
                           select value;
        }

        //

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            holdEnabled
                .ObserveOn(this)
                .Subscribe(value => SetEnabled(btnHold, value));

            timerDisplay
                .ObserveOn(this)
                .Subscribe(value => lblClock.Text = value);

            //// right now, this starts a new timer every time the Start/Stop button is clicked
            //startStopClicked
            //    .SelectSwitch(_ => GetTimer())
            //    .StartWith(0)
            //    .Select(it => TimeSpan.FromSeconds(it))
            //    .ObserveOn(this)
            //    .Subscribe(ts => lblClock.Text = ts.ToString(@"hh\:mm\:ss"));
        }

        //

        private readonly IObservable<bool> holdEnabled;

        private readonly IObservable<string> timerDisplay;

        private static IObservable<EventPattern<EventArgs>> GetClicks(Control control) =>
            Observable.FromEventPattern<EventHandler, EventArgs>(h => control.Click += h, h => control.Click -= h);

        private static void SetEnabled(Control btn, bool value)
        {
            if (value)
            {
                btn.Enabled = true;
                btn.BackColor = Color.Lime;
            }
            else
            {
                btn.Enabled = false;
                btn.BackColor = SystemColors.Control;
            }
        }
    }
}