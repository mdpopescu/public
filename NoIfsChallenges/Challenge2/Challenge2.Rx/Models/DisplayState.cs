using System;
using System.Reactive.Subjects;

namespace Challenge2.Rx.Models
{
    public class DisplayState
    {
        public bool StartStopEnabled { get; private set; }
        public bool ResetEnabled { get; private set; }
        public bool HoldEnabled { get; private set; }

        public TimeSpan TimerValue { get; private set; }
        public bool IsFrozen { get; private set; }
        public string TimerDisplay { get; private set; }

        //public IObservable<TimeSpan> TimerValues { get; }

        public DisplayState()
        {
            //TimerValues = holdEnabled.SwitchMap(isRunning => isRunning ? CreateTimer() : ResetTimer());
            //TimerValues.Subscribe(ts => TimerDisplay = ts.ToString());

            Reset();
        }

        public void StartStop()
        {
            if (HoldEnabled)
            {
                StartStopEnabled = false;
                ResetEnabled = true;
                HoldEnabled = false;
                holdEnabled.OnNext(false);

                IsFrozen = false;
            }
            else
            {
                HoldEnabled = true;
                holdEnabled.OnNext(true);

                IsFrozen = false;
            }
        }

        public void Reset()
        {
            StartStopEnabled = true;
            ResetEnabled = false;
            HoldEnabled = false;
            holdEnabled.OnNext(false);

            TimerValue = TimeSpan.Zero;
            IsFrozen = false;
            TimerDisplay = TimerValue.ToString();
        }

        public void Hold()
        {
            IsFrozen = !IsFrozen;
        }

        public void Tick(TimeSpan interval)
        {
            if (!HoldEnabled)
                return;

            TimerValue += interval;

            if (!IsFrozen)
                TimerDisplay = TimerValue.ToString();
        }

        //

        private readonly ISubject<bool> holdEnabled = new Subject<bool>();

        //private static IObservable<TimeSpan> CreateTimer() => Observable.Interval(TimeSpan.FromSeconds(1)).Select(value => TimeSpan.FromSeconds(value));
        //private static IObservable<TimeSpan> ResetTimer() => Observable.Return(TimeSpan.Zero).Concat(Observable.Never<TimeSpan>());
    }
}