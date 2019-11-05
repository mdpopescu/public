using System;
using TimerApp.Contracts;

namespace TimerApp.Services
{
    public class TimerRunning : TimerState
    {
        public TimerRunning(UserInterface ui, DateTimeOffset startTime)
        {
            this.ui = ui;
            this.startTime = startTime;
        }

        public TimerState StartStop()
        {
            ui.SetButtonText("Start");
            return new TimerStopped(ui);
        }

        public void DisplayTime()
        {
            ui.ShowTime(DateTimeOffset.Now - startTime);
        }

        //

        private readonly UserInterface ui;
        private readonly DateTimeOffset startTime;
    }
}