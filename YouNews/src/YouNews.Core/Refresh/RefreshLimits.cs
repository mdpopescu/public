namespace YouNews.Core.Refresh;

/// <param name="MaximumVideosPerRun">Stops a run after this many videos. The rest stay pending for the next run. Null means no limit.</param>
public sealed record RefreshLimits(int? MaximumVideosPerRun);