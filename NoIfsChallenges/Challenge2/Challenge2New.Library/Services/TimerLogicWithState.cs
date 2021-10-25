using System;
using Challenge2New.Library.Contracts;
using Challenge2New.Library.Models.States;

namespace Challenge2New.Library.Services
{
    public class TimerLogicWithState : ITimerLogic
    {
        public TimerLogicWithState(IUserInterface ui)
        {
            this.ui = ui;

            state = new InitialState();
        }

        public void Dispose()
        {
            ui.TearDown(OnStartStop, OnHold, OnReset);
        }

        public void Initialize()
        {
            ui.SetUp(OnStartStop, OnHold, OnReset);

            state = new InitialState();
        }

        //

        private readonly IUserInterface ui;

        private IState state;

        //

        private void OnStartStop(object? sender, EventArgs e)
        {
            state = state.StartStop(ts => ui.SetDisplay(ts.ToString()));
        }

        private void OnHold(object? sender, EventArgs e)
        {
            state = state.Hold();
        }

        private void OnReset(object? sender, EventArgs e)
        {
            state = state.Reset();
        }
    }
}