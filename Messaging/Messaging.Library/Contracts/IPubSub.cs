using System;

namespace Messaging.Library.Contracts
{
    public interface IPubSub<TValue>
    {
        /// <summary>
        ///     Returns the stream of values.
        /// </summary>
        IObservable<TValue> Messages { get; }

        /// <summary>
        ///     Adds a value to the stream.
        /// </summary>
        /// <param name="value">The value.</param>
        void Publish(TValue value);
    }
}