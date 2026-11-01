namespace YouNews.Core.Model;

public sealed record RefreshRun(DateTimeOffset StartedAt, DateTimeOffset FinishedAt, int Completed, int Failed, string? Problem);

/// <summary>Where a running refresh is up to.</summary>
public sealed record RefreshProgress(string Activity, int VideosDone, int VideosTotal);