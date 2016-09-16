using System.Collections.Generic;

namespace Elomen.Storage.Contracts
{
    /// <summary>
    ///     Key-Value store acting as a pseudo-dictionary.
    /// </summary>
    public interface CompositeSettings
    {
        /// <summary>
        ///     Sets or returns the value stored for a given key; <c>null</c> means there's no value for the given key.
        /// </summary>
        /// <remarks>The keys are case-insensitive.</remarks>
        string this[string key] { get; set; }

        /// <summary>
        ///     Returns all the keys that have matching values.
        /// </summary>
        IEnumerable<string> GetKeys();
    }
}