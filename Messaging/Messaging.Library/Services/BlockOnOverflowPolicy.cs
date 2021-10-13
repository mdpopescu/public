using System;
using System.Threading;
using Messaging.Library.Contracts;

namespace Messaging.Library.Services
{
    public class BlockOnOverflowPolicy<T> : IBackpressurePolicy<T>, IDisposable
    {
        /// <inheritdoc />
        public void Dispose() => gate.Dispose();

        /// <inheritdoc />
        public T? HandleOverflow(T item)
        {
            gate.WaitOne(); // block until the Release method is called
            return item;
        }

        /// <inheritdoc />
        public void Release() => gate.Set();

        //

        private readonly AutoResetEvent gate = new(false);
    }
}