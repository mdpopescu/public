namespace YouNews.Core.Refresh;

/// <summary>Starts refresh runs.</summary>
public interface IRefreshTrigger
{
    /// <summary>Starts a run in the background. Returns false, changing nothing, when one is already running.</summary>
    bool TryStart();
}