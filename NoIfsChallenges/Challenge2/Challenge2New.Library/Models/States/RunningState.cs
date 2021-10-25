using System;
using System.Threading;
using Challenge2New.Library.Contracts;

namespace Challenge2New.Library.Models.States
{
    public class RunningState : IState
    {
        public RunningState()
        {
            timer = new Timer()
        }

        public IState StartStop(Action<TimeSpan> showTime) => throw new NotImplementedException();

        public IState Hold()
        {
            timer.Change()
            return new FrozenState();
        }

        public IState Reset() => throw new NotImplementedException();

        //

        private Timer timer;
    }
}