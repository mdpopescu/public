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

            ui.Text = "00:00:00";
        }

        public void ShowTime(TimeSpan ts)
        {
            //
        }

        public WatchState StartStop()
        {
            return new WatchRunning(ui);
        }

        public WatchState Hold()
        {
            return this;
        }

        //

        private readonly UserInterface ui;
    }
}