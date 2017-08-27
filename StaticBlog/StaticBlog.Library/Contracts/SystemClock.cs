using System;

namespace StaticBlog.Library.Contracts
{
    public interface SystemClock
    {
        DateTime Now { get; }

        void Sleep(TimeSpan duration);
    }
}