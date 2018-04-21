using System;

namespace Challenge2.Library.Contracts
{
    public interface SystemTimer
    {
        IDisposable Start(TimeSpan interval, Action<TimeSpan> callback);
    }
}