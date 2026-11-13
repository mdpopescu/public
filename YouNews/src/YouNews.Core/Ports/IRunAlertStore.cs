using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Keeps the one alert about a run that the user has not dismissed yet. Later runs do not clear it.</summary>
public interface IRunAlertStore
{
    /// <summary>Stores the alert, replacing any alert already waiting.</summary>
    Task RaiseAsync(string message, DateTimeOffset raisedAt, CancellationToken cancellationToken);

    /// <summary>Gets the alert waiting to be dismissed, or null if there is none.</summary>
    Task<RunAlert?> GetAsync(CancellationToken cancellationToken);

    /// <summary>Removes the alert, if any.</summary>
    Task DismissAsync(CancellationToken cancellationToken);
}