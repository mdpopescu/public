using System;

namespace Challenge2New.Library.Contracts
{
    public interface ITimer
    {
        IDisposable Start(TimeSpan interval, Action<TimeSpan> callback);
    }
}