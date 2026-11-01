using YouNews.Core.Model;

namespace YouNews.Core.Refresh;

/// <summary>Tells how the current refresh run is getting on.</summary>
public interface IRefreshStatus
{
    /// <summary>Gets the progress of the running refresh, or null when none is running.</summary>
    RefreshProgress? Current { get; }
}