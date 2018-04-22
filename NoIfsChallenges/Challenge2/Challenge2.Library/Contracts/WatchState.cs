using System;

namespace Challenge2.Library.Contracts
{
    public interface WatchState
    {
        void ShowTime(TimeSpan ts);

        WatchState StartStop(Action<TimeSpan> showTime);
        WatchState Hold();
        WatchState Reset();
    }
}