using System;
using Challenge2New.Library.Contracts;

namespace Challenge2New.Library.Models.States
{
    public class FrozenState : IState
    {
        public IState StartStop(Action<TimeSpan> showTime) => throw new NotImplementedException();

        public IState Hold() => new RunningState();

        public IState Reset() => this;
    }
}