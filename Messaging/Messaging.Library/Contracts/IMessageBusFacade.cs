using System;
using Messaging.Library.Models;

namespace Messaging.Library.Contracts
{
    public interface IMessageBusFacade
    {
        /// <summary>
        ///     The (hot) stream of all messages.
        /// </summary>
        /// <value>The (hot) stream of all messages.</value>
        /// <remarks>This will include error messages, since they inherit from <see cref="MessageBase" />.</remarks>
        IObservable<MessageBase> Messages { get; }

        /// <summary>
        ///     The (hot) stream of all errors.
        /// </summary>
        /// <value>The (hot) stream of all errors.</value>
        IObservable<ErrorMessage> Errors { get; }

        /// <summary>
        ///     Publishes the given message to all listeners.
        /// </summary>
        /// <param name="message">The message being published.</param>
        void Publish(MessageBase message);
    }
}