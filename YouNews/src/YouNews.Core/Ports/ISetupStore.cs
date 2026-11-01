using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Keeps the latest known description of the user's AI server.</summary>
public interface ISetupStore
{
    /// <summary>Gets the latest snapshot, or null if there has never been one.</summary>
    Task<SetupSnapshot?> GetLatestAsync(CancellationToken cancellationToken);

    /// <summary>Replaces the latest snapshot.</summary>
    Task SaveAsync(SetupSnapshot snapshot, CancellationToken cancellationToken);
}