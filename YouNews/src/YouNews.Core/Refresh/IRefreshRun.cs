using YouNews.Core.Model;

namespace YouNews.Core.Refresh;

/// <summary>Performs one complete refresh.</summary>
public interface IRefreshRun
{
    /// <summary>Discovers and processes videos, reporting progress as it goes. A failing channel or video does not stop the run.</summary>
    Task ExecuteAsync(Action<RefreshProgress> reportProgress, CancellationToken cancellationToken);
}