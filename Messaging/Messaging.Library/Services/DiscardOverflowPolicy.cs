using Messaging.Library.Contracts;

namespace Messaging.Library.Services
{
    public class DiscardOverflowPolicy<T> : IBackpressurePolicy<T>
    {
        /// <inheritdoc />
        public T? HandleOverflow(T item) => default;

        /// <inheritdoc />
        public void Release()
        {
            // do nothing
        }
    }
}