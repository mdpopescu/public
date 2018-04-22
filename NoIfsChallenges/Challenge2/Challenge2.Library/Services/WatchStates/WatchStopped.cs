using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public class WatchStopped : WatchState
    {
        public WatchStopped(UserInterface ui)
        {
            this.ui = ui;

            ui.StartStopEnabled = false;
            ui.ResetEnabled = true;
            ui.HoldEnabled = false;
        }

        public void ShowTime(TimeSpan ts)
        {
            // do nothing
        }

        public WatchState StartStop(Action<TimeSpan> showTime) => this;

        public WatchState Hold() => this;

        public WatchState Reset() => new WatchZero(ui);

        //

        private readonly UserInterface ui;
    }
}