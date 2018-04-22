using System;
using Challenge2.Library.Contracts;
using Challenge2.Library.Services.WatchStates;

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
            state = new WatchZero(ui);
        }

        public void StartStop()
        {
            state = state.StartStop();

            GlobalSettings.Timer.Start(TimeSpan.FromSeconds(1), ShowTime);
        }

        public void Hold()
        {
            state = state.Hold();
        }

        public void Reset()
        {
            state = state.Reset();
        }

        //

        private readonly UserInterface ui;

        private WatchState state;

        private void ShowTime(TimeSpan ts)
        {
            state.ShowTime(ts);
        }
    }
}