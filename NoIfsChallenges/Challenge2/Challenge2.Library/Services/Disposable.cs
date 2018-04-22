using System;

namespace Challenge2.Library.Services
{
    public class Disposable : IDisposable
    {
        public Disposable(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }

        //

        private readonly Action action;
    }
}