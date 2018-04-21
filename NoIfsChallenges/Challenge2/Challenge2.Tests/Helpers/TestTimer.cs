using System;
using Challenge2.Library.Contracts;
using Challenge2.Library.Services;

namespace Challenge2.Tests.Helpers
{
    public class TestTimer : SystemTimer
    {
        // ReSharper disable once ParameterHidesMember
        public IDisposable Start(TimeSpan interval, Action<TimeSpan> callback)
        {
            this.callback = callback;
            currentTime = TimeSpan.Zero;
            return new EmptyDisposable();
        }

        public void Advance(int count)
        {
            for (var i = 1; i <= count; i++)
            {
                currentTime = currentTime.Add(TimeSpan.FromSeconds(1));
                callback(currentTime);
            }
        }

        //

        private Action<TimeSpan> callback;

        private TimeSpan currentTime;
    }
}