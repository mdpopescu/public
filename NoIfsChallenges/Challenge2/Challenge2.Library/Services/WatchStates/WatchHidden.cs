using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public sealed class WatchHidden : WatchState
    {
        public static WatchHidden Create(UserInterface ui, IDisposable timer) => new WatchHidden(ui, timer);

        //

        public void ShowTime(TimeSpan ts)
        {
            // do nothing
        }

        public WatchState StartStop(Action<TimeSpan> showTime)
        {
            timer.Dispose();
            return WatchStopped.Create(ui);
        }

        public WatchState Hold() => WatchRunning.Create(ui, timer);

        public WatchState Reset() => this;

        //

        private readonly UserInterface ui;
        private readonly IDisposable timer;

        private WatchHidden(UserInterface ui, IDisposable timer)
        {
            this.ui = ui;
            this.timer = timer;
        }
    }
}