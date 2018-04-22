using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public class WatchHidden : WatchState
    {
        public WatchHidden(UserInterface ui, IDisposable timer)
        {
            this.ui = ui;
            this.timer = timer;
        }

        public void ShowTime(TimeSpan ts)
        {
            // do nothing
        }

        public WatchState StartStop(Action<TimeSpan> showTime)
        {
            timer.Dispose();
            return new WatchStopped(ui);
        }

        public WatchState Hold() => new WatchRunning(ui, timer);

        public WatchState Reset() => this;

        //

        private readonly UserInterface ui;
        private readonly IDisposable timer;
    }
}