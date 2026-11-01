using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Remembers finished refresh runs.</summary>
public interface IRefreshLog
{
    /// <summary>Records a finished run.</summary>
    Task RecordAsync(RefreshRun run, CancellationToken cancellationToken);

    /// <summary>Gets the most recent run, or null if none has finished.</summary>
    Task<RefreshRun?> GetLatestAsync(CancellationToken cancellationToken);
}