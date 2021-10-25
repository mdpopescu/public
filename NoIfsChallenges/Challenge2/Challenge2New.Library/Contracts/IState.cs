using System;

namespace Challenge2New.Library.Contracts
{
    public interface IState
    {
        IState StartStop(Action<TimeSpan> showTime);
        IState Hold();
        IState Reset();
    }
}