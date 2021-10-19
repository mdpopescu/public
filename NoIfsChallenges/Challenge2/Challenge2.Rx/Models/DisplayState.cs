using System;
using System.Reactive.Linq;

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

        //public IObservable<TimeSpan> TimerValues { get; private set; }

        public DisplayState()
        {
            Reset();
        }

        public void StartStop()
        {
            if (HoldEnabled)
            {
                StartStopEnabled = false;
                ResetEnabled = true;
                HoldEnabled = false;

                IsFrozen = false;
            }
            else
            {
                HoldEnabled = true;

                IsFrozen = false;

                //TimerValues = Observable.Interval(TimeSpan.FromSeconds(1)).Select(value => TimeSpan.FromSeconds(value));
            }
        }

        public void Reset()
        {
            StartStopEnabled = true;
            ResetEnabled = false;
            HoldEnabled = false;

            TimerValue = TimeSpan.Zero;
            IsFrozen = false;
            TimerDisplay = TimerValue.ToString();

            //TimerValues = Observable.Repeat(TimeSpan.Zero, 1).Concat(Observable.Never<TimeSpan>());
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
    }
}