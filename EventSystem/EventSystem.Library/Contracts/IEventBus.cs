using System;
using System.Threading.Tasks;

namespace EventSystem.Library.Contracts;

public interface IEventBus
{
    /// <summary>
    ///     Publish a message asynchronously, without waiting for the eventual handler(s) to process it.
    /// </summary>
    /// <param name="msg">The message to publish.</param>
    /// <remarks>If there are no handlers for the message, it will be lost.</remarks>
    Task PublishAsync(IMessage msg);

    /// <summary>
    ///     Invoke the given handler asynchronously when a new message of the given type is published.
    /// </summary>
    /// <param name="handler">The message handler.</param>
    IDisposable Subscribe<T>(Func<T, Task> handler)
        where T : IMessage;
}