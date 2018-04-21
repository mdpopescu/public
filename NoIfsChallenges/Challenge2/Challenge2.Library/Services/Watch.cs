using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services
{
    public class Watch
    {
        public Watch(UserInterface ui)
        {
            this.ui = ui;
        }

        public void Initialize()
        {
            ui.StartStopEnabled = true;
            ui.ResetEnabled = false;
            ui.HoldEnabled = false;

            ui.Text = "00:00:00";
        }

        public void StartStop()
        {
            ui.StartStopEnabled = true;
            ui.ResetEnabled = false;
            ui.HoldEnabled = true;

            ui.Text = "00:00:00";

            GlobalSettings.Timer.Start(TimeSpan.FromSeconds(1), ts => ui.Text = ts.ToString());
        }

        //

        private readonly UserInterface ui;
    }
}