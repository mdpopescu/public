using System;
using Messaging.Library.Contracts;

namespace Messaging.Library.Models
{
    public class ErrorMessage : MessageBase, IErrorMessage
    {
        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public string? AdditionalInfo { get; }

        public ErrorMessage(Guid id, Guid? categoryId, Guid? inReplyTo, int version, string description, string? additionalInfo)
            : base(id, categoryId, inReplyTo, version)
        {
            Description = description;
            AdditionalInfo = additionalInfo;
        }
    }
}