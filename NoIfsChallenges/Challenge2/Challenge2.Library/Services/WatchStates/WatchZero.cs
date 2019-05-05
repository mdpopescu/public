using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public sealed class WatchZero : WatchState
    {
        public static WatchZero Create(UserInterface ui)
        {
            var state = new WatchZero(ui);

            ui.EnableStartStop();
            ui.DisableReset();
            ui.DisableHold();

            ui.Display("00:00:00");

            return state;
        }

        //

        public void ShowTime(TimeSpan ts)
        {
            // do nothing
        }

        public WatchState StartStop(Action<TimeSpan> showTime)
        {
            var timer = GlobalSettings.Timer.Start(TimeSpan.FromSeconds(1), showTime);
            return WatchRunning.Create(ui, timer);
        }

        public WatchState Hold() => this;

        public WatchState Reset() => this;

        //

        private readonly UserInterface ui;

        private WatchZero(UserInterface ui)
        {
            this.ui = ui;
        }
    }
}