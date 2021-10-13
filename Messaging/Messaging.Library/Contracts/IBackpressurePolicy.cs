namespace Messaging.Library.Contracts
{
    /// <summary>
    ///     An interface describing how to handle overflow in a bounded collection.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public interface IBackpressurePolicy<T>
    {
        /// <summary>
        ///     This method will decide what should happen when overflow is detected.
        /// </summary>
        /// <param name="item">The item causing the overflow.</param>
        /// <returns>
        ///     The method can return <c>null</c> to indicate that the item should be discarded, a value of type
        ///     <see cref="T" /> that should be added to the collection (ignoring the overflow condition), it can throw an
        ///     exception or it can block (in which case the <see cref="Release" /> method should be used to indicate the
        ///     end of the overflow condition).
        /// </returns>
        T? HandleOverflow(T item);

        /// <summary>
        ///     This indicates that the overflow condition no longer applies.
        /// </summary>
        /// <remarks>
        ///     It is recommended that this method should not be called immediately when one slot is available, but rather
        ///     after more space has been created (e.g., 10%).
        /// </remarks>
        void Release();
    }
}