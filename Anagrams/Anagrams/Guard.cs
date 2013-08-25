using System;

namespace Renfield.Anagrams
{
    public class Guard : IDisposable
    {
        public Guard(Action action, Action undo)
        {
            this.undo = undo;

            action();
        }

        public void Dispose()
        {
            undo();
        }

        //

        private readonly Action undo;
    }
}