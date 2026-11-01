using System.Globalization;
using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

public sealed class StatusPresenter : IStatusPresenter
{
    public StatusPresenter(TimeProvider time)
    {
        this.time = time;
    }

    public StatusView Present(RefreshRun? lastRun, IReadOnlyList<TrackedVideo> waiting, IReadOnlyList<string> toolProblems) =>
        new(lastRun is null ? null : DescribeRun(lastRun), lastRun?.Problem, waiting.Select(PresentWaiting).ToList(), toolProblems);

    //

    private readonly TimeProvider time;

    private static WaitingVideoView PresentWaiting(TrackedVideo video)
    {
        var detail = video.State == VideoState.GaveUp ? "gave up" : video.Attempts == 1 ? "tried once" : $"tried {video.Attempts} times";
        return new WaitingVideoView(
            video.Title,
            $"https://www.youtube.com/watch?v={Uri.EscapeDataString(video.YouTubeId)}",
            string.IsNullOrWhiteSpace(video.LastReason) ? "no reason recorded" : video.LastReason,
            detail);
    }

    private string DescribeRun(RefreshRun run)
    {
        var local = TimeZoneInfo.ConvertTime(run.FinishedAt, time.LocalTimeZone).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        return $"Last refresh {local}: {run.Completed} completed, {run.Failed} failed.";
    }
}