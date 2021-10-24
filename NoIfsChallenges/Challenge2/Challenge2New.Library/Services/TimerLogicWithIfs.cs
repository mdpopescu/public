using System;
using System.Threading;
using Challenge2New.Library.Contracts;
using Challenge2New.Library.Models;

namespace Challenge2New.Library.Services
{
    public class TimerLogicWithIfs : ITimerLogic
    {
        public TimerLogicWithIfs(IUserInterface ui)
        {
            this.ui = ui;
        }

        public void Dispose()
        {
            ui.TearDown(OnStartStop, OnHold, OnReset);
        }

        public void Initialize()
        {
            ui.SetUp(OnStartStop, OnHold, OnReset);

            ui.SetEnabled(OperableActionName.START_STOP, true);
            ui.SetEnabled(OperableActionName.HOLD, false);
            ui.SetEnabled(OperableActionName.RESET, false);
            ui.SetDisplay("00:00:00");
        }

        //

        private readonly IUserInterface ui;

        private Timer? timer;
        private TimeSpan currentTime;

        private bool isRunning;
        private bool isFrozen;

        //

        private void OnStartStop(object? sender, EventArgs e)
        {
            if (isRunning)
                Stop();
            else
                Start();
        }

        private void Start()
        {
            timer = new Timer(OnTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            currentTime = TimeSpan.Zero;

            isRunning = true;
            isFrozen = false;

            ui.SetEnabled(OperableActionName.HOLD, true);
        }

        private void Stop()
        {
            timer?.Dispose();
            timer = null;

            isRunning = false;
            isFrozen = false;

            ui.SetEnabled(OperableActionName.START_STOP, false);
            ui.SetEnabled(OperableActionName.HOLD, false);
            ui.SetEnabled(OperableActionName.RESET, true);
        }

        private void OnTick(object? state)
        {
            currentTime += TimeSpan.FromSeconds(1);

            if (!isFrozen)
                ui.SetDisplay(currentTime.ToString());
        }

        private void OnHold(object? sender, EventArgs e)
        {
            isFrozen = !isFrozen;
        }

        private void OnReset(object? sender, EventArgs e)
        {
            ui.SetEnabled(OperableActionName.START_STOP, true);
            ui.SetEnabled(OperableActionName.HOLD, false);
            ui.SetEnabled(OperableActionName.RESET, false);
            ui.SetDisplay("00:00:00");
        }
    }
}