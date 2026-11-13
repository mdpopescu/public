using System.Globalization;
using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

public sealed class StatusPresenter : IStatusPresenter
{
    public StatusPresenter(TimeProvider time)
    {
        this.time = time;
    }

    public StatusView Present(RefreshRun? lastRun, IReadOnlyList<TrackedVideo> waiting, IReadOnlyList<string> toolProblems, PendingQueue queue) =>
        new(
            lastRun is null ? null : DescribeRun(lastRun),
            lastRun?.Problem,
            waiting.Select(PresentWaiting).ToList(),
            toolProblems,
            DescribeQueue(queue.Videos.Count),
            queue.Videos.Take(QUEUE_PREVIEW_SIZE).Select(video => PresentQueued(video, queue.ChannelNames)).ToList());

    public StatusView PresentBanner(IReadOnlyList<string> toolProblems, RunAlert? alert) =>
        new(null, null, [], toolProblems, null, [], alert is null ? null : new RunAlertView(alert.Message, $"Raised {FormatLocal(alert.RaisedAt)}."));

    //

    private const int QUEUE_PREVIEW_SIZE = 10;

    private readonly TimeProvider time;

    private static WaitingVideoView PresentWaiting(TrackedVideo video)
    {
        var detail = video.State == VideoState.GaveUp ? "gave up" : video.Attempts == 1 ? "tried once" : $"tried {video.Attempts} times";
        return new WaitingVideoView(
            video.Title,
            WatchUrl(video),
            string.IsNullOrWhiteSpace(video.LastReason) ? "no reason recorded" : video.LastReason,
            detail);
    }

    private static string? DescribeQueue(int count) => count switch
    {
        0 => null,
        1 => "1 video waiting to be processed",
        _ => $"{count} videos waiting to be processed",
    };

    private static QueuedVideoView PresentQueued(TrackedVideo video, IReadOnlyDictionary<long, string> channelNames)
    {
        var parts = new List<string>();
        if (channelNames.TryGetValue(video.ChannelId, out var channel))
            parts.Add(channel);
        if (video.PublishedOn is { } publishedOn)
            parts.Add(publishedOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        return new QueuedVideoView(video.Title, WatchUrl(video), string.Join(", ", parts));
    }

    private static string WatchUrl(TrackedVideo video) => $"https://www.youtube.com/watch?v={Uri.EscapeDataString(video.YouTubeId)}";

    private string DescribeRun(RefreshRun run)
    {
        return $"Last refresh {FormatLocal(run.FinishedAt)}: {run.Completed} completed, {run.Failed} failed.";
    }

    private string FormatLocal(DateTimeOffset moment) =>
        TimeZoneInfo.ConvertTime(moment, time.LocalTimeZone).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
}