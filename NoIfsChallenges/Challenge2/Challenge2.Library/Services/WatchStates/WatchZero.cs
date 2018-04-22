using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public class WatchZero : WatchState
    {
        public WatchZero(UserInterface ui)
        {
            this.ui = ui;

            ui.StartStopEnabled = true;
            ui.ResetEnabled = false;
            ui.HoldEnabled = false;

            ui.Display = "00:00:00";
        }

        public void ShowTime(TimeSpan ts)
        {
            // do nothing
        }

        public WatchState StartStop(Action<TimeSpan> showTime)
        {
            var timer = GlobalSettings.Timer.Start(TimeSpan.FromSeconds(1), showTime);
            return new WatchRunning(ui, timer);
        }

        public WatchState Hold()
        {
            return this;
        }

        public WatchState Reset()
        {
            return this;
        }

        //

        private readonly UserInterface ui;
    }
}