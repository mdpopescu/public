using System;

namespace Messaging.Library.Models
{
    public class ErrorMessage : MessageBase
    {
        /// <summary>
        ///     This is the error message.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Optionally, we can attach additional info (like a stack trace) to the error message.
        /// </summary>
        public string? AdditionalInfo { get; }

        public ErrorMessage(Guid id, Guid? categoryId, Guid? inReplyTo, int version, string description, string? additionalInfo)
            : base(id, categoryId, inReplyTo, version)
        {
            Description = description;
            AdditionalInfo = additionalInfo;
        }
    }
}