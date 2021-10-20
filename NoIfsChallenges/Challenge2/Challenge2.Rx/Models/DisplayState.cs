using System;
using System.Reactive.Disposables;
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
            var s1 = holdEnabled
                .SwitchMap(isRunning => isRunning ? CreateTimer() : ResetTimer())
                .Subscribe();
            var s2 = holdEnabled
                .Subscribe(value => HoldEnabled = value);
            var s3 = holdEnabled
                .Subscribe(value => startStopHandler = value ? (Action)Stop : Start);
            subscription = new CompositeDisposable(s1, s2, s3);

            Reset();
        }

        public void Dispose() =>
            subscription.Dispose();

        public void StartStop() =>
            startStopHandler.Invoke();

        public void Reset()
        {
            StartStopEnabled = true;
            ResetEnabled = false;
            holdEnabled.OnNext(false);

            TimerValue = TimeSpan.Zero;
            IsFrozen = false;
            TimerDisplay = TimerValue.ToString();
        }

        public void Hold()
        {
            IsFrozen = !IsFrozen;
        }

        //

        private static readonly TimeSpan INCREMENT = TimeSpan.FromSeconds(1);

        private readonly ISubject<bool> holdEnabled = new Subject<bool>();

        private readonly IDisposable subscription;

        private Action startStopHandler;

        private IObservable<long> CreateTimer() =>
            Observable.Interval(INCREMENT).Do(_ => Tick());

        private static IObservable<long> ResetTimer() =>
            Observable.Return(0L).Concat(Observable.Never<long>());

        private void Start()
        {
            holdEnabled.OnNext(true);

            IsFrozen = false;
        }

        private void Stop()
        {
            StartStopEnabled = false;
            ResetEnabled = true;
            holdEnabled.OnNext(false);

            IsFrozen = false;
        }

        private void Tick()
        {
            TimerValue += INCREMENT;

            if (!IsFrozen)
                TimerDisplay = TimerValue.ToString();
        }
    }
}