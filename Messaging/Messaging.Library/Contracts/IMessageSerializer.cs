namespace Messaging.Library.Contracts
{
    public interface IMessageSerializer<TTransport>
    {
        /// <summary>
        ///     Deserializes the message from a given format (examples, <c>byte[]</c> or <c>string</c>).
        /// </summary>
        /// <param name="serialized">The serialized message.</param>
        /// <returns>The deserialized message.</returns>
        /// <remarks>If deserialization fails, whether the method throws or returns <c>null</c> is left to the implementation.</remarks>
        IMessage? Deserialize(TTransport serialized);

        /// <summary>
        ///     Serializes the message into a given format (examples, <c>byte[]</c> or <c>string</c>).
        /// </summary>
        /// <param name="message">The message to be serialized.</param>
        /// <returns>The serialized message.</returns>
        TTransport Serialize(IMessage message);
    }
}