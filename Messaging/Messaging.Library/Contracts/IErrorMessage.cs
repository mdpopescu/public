namespace Messaging.Library.Contracts
{
    public interface IErrorMessage : IMessage
    {
        /// <summary>
        ///     This is the error message.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Optionally, we can attach additional info (like a stack trace) to the error message.
        /// </summary>
        string? AdditionalInfo { get; }
    }
}