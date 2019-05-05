using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public sealed class WatchStopped : WatchState
    {
        public static WatchStopped Create(UserInterface ui)
        {
            var state = new WatchStopped(ui);

            ui.DisableStartStop();
            ui.EnableReset();
            ui.DisableHold();

            return state;
        }

        //

        public void ShowTime(TimeSpan ts)
        {
            // do nothing
        }

        public WatchState StartStop(Action<TimeSpan> showTime) => this;

        public WatchState Hold() => this;

        public WatchState Reset() => WatchZero.Create(ui);

        //

        private readonly UserInterface ui;

        private WatchStopped(UserInterface ui)
        {
            this.ui = ui;
        }
    }
}