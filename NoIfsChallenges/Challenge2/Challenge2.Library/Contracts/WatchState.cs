using System;

namespace Challenge2.Library.Contracts
{
    public interface WatchState
    {
        void ShowTime(TimeSpan ts);

        WatchState StartStop();
        WatchState Hold();
        WatchState Reset();
    }
}