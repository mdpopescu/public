using System;

namespace Messaging.Library.Contracts
{
    public interface IMessageBusFacade
    {
        /// <summary>
        ///     Publishes the given message to all listeners.
        /// </summary>
        /// <param name="message">The message being published.</param>
        void Publish(IMessage message);

        /// <summary>
        ///     Returns the (hot) stream of all messages.
        /// </summary>
        /// <returns>The stream of all messages.</returns>
        /// <remarks>This will include error messages, since they inherit from <see cref="IMessage" />.</remarks>
        IObservable<IMessage> GetMessages();

        /// <summary>
        ///     Returns the (hot) stream of all errors.
        /// </summary>
        /// <returns>The stream of all errors.</returns>
        IObservable<IErrorMessage> GetErrors();
    }
}