namespace YouNews.Core.Ports;

/// <summary>Keeps the user's editable list of interests.</summary>
public interface IInterestStore
{
    /// <summary>Gets the interests, in the order the user gave them.</summary>
    Task<IReadOnlyList<string>> GetAsync(CancellationToken cancellationToken);

    /// <summary>Replaces the interests.</summary>
    Task SaveAsync(IReadOnlyList<string> interests, CancellationToken cancellationToken);
}