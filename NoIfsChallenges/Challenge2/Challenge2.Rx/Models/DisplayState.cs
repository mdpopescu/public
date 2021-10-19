using System;

namespace Challenge2.Rx.Models
{
    public class DisplayState
    {
        public bool StartStopEnabled { get; private set; }
        public bool ResetEnabled { get; private set; }
        public bool HoldEnabled { get; private set; }

        public TimeSpan TimerValue { get; private set; }
        public bool IsRunning { get; private set; }

        public bool IsFrozen { get; private set; }
        public string TimerDisplay { get; private set; }

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

                // TimerValue
                IsRunning = false;

                IsFrozen = false;
                // TimerDisplay
            }
            else
            {
                // StartStopEnabled
                // ResetEnabled
                HoldEnabled = true;

                // TimerValue
                IsRunning = true;

                IsFrozen = false;
                // TimerDisplay
            }
        }

        public void Reset()
        {
            StartStopEnabled = true;
            ResetEnabled = false;
            HoldEnabled = false;

            TimerValue = TimeSpan.Zero;
            IsRunning = false;

            IsFrozen = false;
            TimerDisplay = TimerValue.ToString();
        }

        public void Hold()
        {
            IsFrozen = !IsFrozen;
        }

        public void Tick(TimeSpan interval)
        {
            // ReSharper disable once InvertIf
            if (IsRunning)
            {
                TimerValue += interval;

                if (!IsFrozen)
                    TimerDisplay = TimerValue.ToString();
            }
        }
    }
}