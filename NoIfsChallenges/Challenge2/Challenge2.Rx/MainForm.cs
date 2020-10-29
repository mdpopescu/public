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

            StartStopClicked = GetClicks(btnStartStop).AsUnit();
            ResetClicked = GetClicks(btnReset).AsUnit();
            HoldClicked = GetClicks(btnHold).AsUnit();

            TimerTicked = GetTimer().AsUnit();
        }

        //

        private IObservable<Unit> StartStopClicked { get; }
        private IObservable<Unit> ResetClicked { get; }
        private IObservable<Unit> HoldClicked { get; }

        private IObservable<Unit> TimerTicked { get; }

        private static IObservable<EventPattern<EventArgs>> GetClicks(Control control) =>
            Observable.FromEventPattern<EventHandler, EventArgs>(h => control.Click += h, h => control.Click -= h);

        private static IObservable<long> GetTimer() =>
            Observable.Interval(TimeSpan.FromSeconds(1));
    }
}