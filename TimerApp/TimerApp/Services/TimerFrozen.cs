using System;
using TimerApp.Contracts;

namespace TimerApp.Services
{
    public class TimerFrozen : TimerState
    {
        public TimerFrozen(UserInterface ui)
        {
            this.ui = ui;
        }

        public TimerState StartStop()
        {
            ui.SetButtonText("Start");
            ui.ShowTime(TimeSpan.Zero);

            return new TimerStopped(ui);
        }

        public void DisplayTime()
        {
            // do nothing
        }

        //

        private readonly UserInterface ui;
    }
}