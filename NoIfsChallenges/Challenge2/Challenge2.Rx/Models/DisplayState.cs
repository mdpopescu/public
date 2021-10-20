using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Challenge2.Rx.Helpers;

namespace Challenge2.Rx.Models
{
    public class DisplayState : IDisposable
    {
        public bool StartStopEnabled { get; private set; }
        public bool ResetEnabled { get; private set; }
        public bool HoldEnabled { get; private set; }

        public TimeSpan TimerValue { get; private set; }
        public bool IsFrozen { get; private set; }
        public string TimerDisplay { get; private set; }

        public DisplayState()
        {
            subscription = holdEnabled
                .SwitchMap(isRunning => isRunning ? CreateTimer() : ResetTimer())
                .Subscribe(_ => Tick(INCREMENT));

            Reset();
        }

        public void Dispose()
        {
            subscription.Dispose();
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

        private static readonly TimeSpan INCREMENT = TimeSpan.FromSeconds(1);

        private readonly ISubject<bool> holdEnabled = new Subject<bool>();

        private readonly IDisposable subscription;

        private static IObservable<long> CreateTimer() => Observable.Interval(INCREMENT);
        private static IObservable<long> ResetTimer() => Observable.Return(0L).Concat(Observable.Never<long>());
    }
}