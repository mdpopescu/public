using System;

namespace Messaging.Library.Models
{
    public abstract class MessageBase
    {
        /// <summary>
        ///     Id uniquely identifying this message.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        ///     Optionally, messages can be grouped into categories.
        /// </summary>
        public Guid? CategoryId { get; }

        /// <summary>
        ///     Optionally, messages can be related to other messages.
        /// </summary>
        public Guid? RefId { get; }

        /// <summary>
        ///     Messages can be versioned.
        /// </summary>
        public int Version { get; }

        //

        protected MessageBase(Guid id, Guid? categoryId, Guid? refId, int version)
        {
            Id = id;
            CategoryId = categoryId;
            RefId = refId;
            Version = version;
        }
    }
}