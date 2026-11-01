using YouNews.Core.Model;

namespace YouNews.Core.Setup;

/// <summary>Provides the description of the user's AI server, refreshing it automatically once a day.</summary>
public interface ISetupMonitor
{
    /// <summary>
    /// Gets the current snapshot, inspecting the server first if the stored one is older than today or carries a warning.
    /// When the inspection fails, the last snapshot is kept and its warning is set. Returns null if the server was
    /// never inspected successfully. Throws only on cancellation.
    /// </summary>
    Task<SetupSnapshot?> GetCurrentAsync(CancellationToken cancellationToken);
}