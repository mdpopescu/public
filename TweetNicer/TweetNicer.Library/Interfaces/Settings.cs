using System.Collections.Generic;

namespace TweetNicer.Library.Interfaces
{
    /// <summary>
    ///     Key-Value store
    /// </summary>
    public interface Settings
    {
        /// <summary>
        ///     Sets or returns the value stored for a given key; <c>null</c> means there's no value for the given key.
        /// </summary>
        /// <param name="key">The (case-insensitive) key.</param>
        /// <returns>The value for the given key.</returns>
        /// <remarks>The keys are case-insensitive.</remarks>
        string this[string key] { get; set; }

        /// <summary>
        ///     Returns all the keys that have matching values.
        /// </summary>
        /// <returns>The list of known keys.</returns>
        IEnumerable<string> GetKeys();
    }
}