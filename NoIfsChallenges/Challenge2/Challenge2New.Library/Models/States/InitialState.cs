using System;
using Challenge2New.Library.Contracts;

namespace Challenge2New.Library.Models.States
{
    public class InitialState : IState
    {
        public IState StartStop(Action<TimeSpan> showTime) => throw new NotImplementedException();

        public IState Hold() => this;

        public IState Reset() => this;
    }
}