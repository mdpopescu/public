using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public class WatchZero : WatchState
    {
        public WatchZero(UserInterface ui)
        {
            this.ui = ui;

            ui.EnableStartStop();
            ui.DisableReset();
            ui.DisableHold();

            ui.Display("00:00:00");
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

        public WatchState Hold() => this;

        public WatchState Reset() => this;

        //

        private readonly UserInterface ui;
    }
}