using System;
using System.Threading.Tasks;

namespace EventSystem.Library.Contracts;

public interface IResultsCache
{
    /// <summary>
    ///     Returns the cached value associated with the given id.
    /// </summary>
    /// <param name="id">The id of the cached value.</param>
    /// <param name="timeout">The maximum amount of time to wait for the result.</param>
    /// <returns>The cached value, or <c>default</c> if no result has been encountered before the timeout expired.</returns>
    Task<T?> GetAsync<T>(Guid id, TimeSpan timeout);
}