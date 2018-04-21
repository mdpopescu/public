using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services
{
    public class RealTimer : SystemTimer
    {
        public IDisposable Start(TimeSpan interval, Action<TimeSpan> callback)
        {
            throw new NotImplementedException();
        }
    }
}