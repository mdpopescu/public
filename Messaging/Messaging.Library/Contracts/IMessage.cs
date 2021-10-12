using System;

namespace Messaging.Library.Contracts
{
    public interface IMessage
    {
        /// <summary>
        ///     Id uniquely identifying this message.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        ///     Optionally, messages can be grouped into categories.
        /// </summary>
        Guid? CategoryId { get; }

        /// <summary>
        ///     Optionally, messages can be replies to other messages.
        /// </summary>
        Guid? InReplyTo { get; }

        /// <summary>
        ///     Messages can be versioned.
        /// </summary>
        int Version { get; }
    }
}