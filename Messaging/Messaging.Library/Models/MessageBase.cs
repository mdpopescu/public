using System;
using Messaging.Library.Contracts;

namespace Messaging.Library.Models
{
    public abstract class MessageBase : IMessage
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public Guid? CategoryId { get; }

        /// <inheritdoc />
        public Guid? RefId { get; }

        /// <inheritdoc />
        public int Version { get; }

        //

        protected MessageBase(Guid id, Guid? categoryId, Guid? inReplyTo, int version)
        {
            Id = id;
            CategoryId = categoryId;
            RefId = inReplyTo;
            Version = version;
        }
    }
}