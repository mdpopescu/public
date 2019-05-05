using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public sealed class WatchRunning : WatchState
    {
        public static WatchRunning Create(UserInterface ui, IDisposable timer)
        {
            var state = new WatchRunning(ui, timer);

            ui.EnableStartStop();
            ui.DisableReset();
            ui.EnableHold();

            return state;
        }

        //

        public void ShowTime(TimeSpan ts)
        {
            ui.Display(ts.ToString(@"hh\:mm\:ss"));
        }

        public WatchState StartStop(Action<TimeSpan> showTime)
        {
            timer.Dispose();
            return WatchStopped.Create(ui);
        }

        public WatchState Hold() => WatchHidden.Create(ui, timer);

        public WatchState Reset() => this;

        //

        private readonly UserInterface ui;
        private readonly IDisposable timer;

        private WatchRunning(UserInterface ui, IDisposable timer)
        {
            this.ui = ui;
            this.timer = timer;
        }
    }
}