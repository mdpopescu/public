﻿using System;
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

            GlobalSettings.Timer.Start(TimeSpan.FromSeconds(1), ts => state.ShowTime(ts));
        }

        public void Hold()
        {
            state = state.Hold();
        }

        //

        private readonly UserInterface ui;

        private WatchState state;
    }
}