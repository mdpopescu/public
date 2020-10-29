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

            startStopClicked = GetClicks(btnStartStop).AsUnit();
            resetClicked = GetClicks(btnReset).AsUnit();
            holdClicked = GetClicks(btnHold).AsUnit();

            timerTicked = GetTimer().AsUnit();
        }

        //

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // right now, this starts a new timer every time the Start/Stop button is clicked
            startStopClicked
                .SelectSwitch(_ => GetTimer())
                .StartWith(0)
                .Select(it => TimeSpan.FromSeconds(it))
                .ObserveOn(this)
                .Subscribe(ts => lblClock.Text = ts.ToString(@"hh\:mm\:ss"));
        }

        //

        private readonly IObservable<Unit> startStopClicked;
        private readonly IObservable<Unit> resetClicked;
        private readonly IObservable<Unit> holdClicked;

        private readonly IObservable<Unit> timerTicked;

        private static IObservable<EventPattern<EventArgs>> GetClicks(Control control) =>
            Observable.FromEventPattern<EventHandler, EventArgs>(h => control.Click += h, h => control.Click -= h);

        private static IObservable<long> GetTimer() =>
            Observable.Interval(TimeSpan.FromSeconds(1));

        private IObservable<int> CreateStoppedClock(int value) =>
            Observable.Repeat(value, 1);
    }
}