namespace Messaging.Library.Contracts
{
    public interface IMessageSerializer<TTransport>
    {
        /// <summary>
        ///     Deserializes the message from a given format (examples, <c>byte[]</c> or <c>string</c>).
        /// </summary>
        /// <param name="serialized">The serialized message.</param>
        void Read(TTransport serialized);

        /// <summary>
        ///     Serializes the message into a given format (examples, <c>byte[]</c> or <c>string</c>).
        /// </summary>
        /// <returns>The serialized message.</returns>
        TTransport Write();
    }
}