using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public class WatchRunning : WatchState
    {
        public WatchRunning(UserInterface ui)
        {
            this.ui = ui;

            ui.StartStopEnabled = true;
            ui.ResetEnabled = false;
            ui.HoldEnabled = true;
        }

        public void ShowTime(TimeSpan ts)
        {
            ui.Text = ts.ToString();
        }

        public WatchState StartStop()
        {
            throw new NotImplementedException();
        }

        public WatchState Hold()
        {
            return new WatchPaused(ui);
        }

        //

        private readonly UserInterface ui;
    }
}