using System;

namespace StaticBlog.Library.Contracts
{
    public interface SystemClock
    {
        void Sleep(TimeSpan duration);
    }
}