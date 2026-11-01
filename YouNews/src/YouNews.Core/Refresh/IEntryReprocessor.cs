using YouNews.Core.Model;

namespace YouNews.Core.Refresh;

/// <summary>Writes the texts of an existing entry again with the current model.</summary>
public interface IEntryReprocessor
{
    /// <summary>
    /// Gets the transcript again, reruns the analysis and the search, and replaces the summary, opinion, web sources and related
    /// entries of the entry. Throws, leaving the entry as it was, when any step fails or a text comes back blank.
    /// </summary>
    /// <param name="job">The entry and its video.</param>
    /// <param name="setup">The description of the user's AI server, or null when it was never detected.</param>
    /// <param name="interests">The user's interests.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    Task ReprocessAsync(ReprocessJob job, SetupSnapshot? setup, IReadOnlyList<string> interests, CancellationToken cancellationToken);
}