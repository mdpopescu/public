using System;
using Messaging.Library.Contracts;

namespace Messaging.Library.Services
{
    public class ThrowOnOverflowPolicy<T> : IBackpressurePolicy<T>
    {
        /// <inheritdoc />
        public T? HandleOverflow(T item) => throw new InvalidOperationException("Overflow.");

        /// <inheritdoc />
        public void Release()
        {
            // do nothing
        }
    }
}