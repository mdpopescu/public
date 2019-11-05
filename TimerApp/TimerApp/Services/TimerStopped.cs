using System;
using TimerApp.Contracts;

namespace TimerApp.Services
{
    public class TimerStopped : TimerState
    {
        public TimerStopped(UserInterface ui)
        {
            this.ui = ui;
        }

        public TimerState StartStop()
        {
            ui.SetButtonText("Stop");
            return new TimerRunning(ui, DateTimeOffset.Now);
        }

        public void DisplayTime()
        {
            // do nothing
        }

        //

        private readonly UserInterface ui;
    }
}