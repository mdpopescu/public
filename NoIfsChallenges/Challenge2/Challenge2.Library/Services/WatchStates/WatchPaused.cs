using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public class WatchPaused : WatchState
    {
        public WatchPaused(UserInterface ui)
        {
            this.ui = ui;
        }

        public void ShowTime(TimeSpan ts)
        {
            // do nothing
        }

        public WatchState StartStop()
        {
            return this;
        }

        public WatchState Hold()
        {
            return new WatchRunning(ui);
        }

        //

        private readonly UserInterface ui;
    }
}