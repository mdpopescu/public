using System.Globalization;
using System.Threading.Channels;
using YouNews.Core.Model;

namespace YouNews.Core.Refresh;

/// <summary>
/// Joins the two sides of a run: combines their progress into one report, and wakes the consumer when the producer queues a video.
/// The videos themselves wait in the store, because the order they are taken in can change as more are discovered.
/// </summary>
internal sealed class RefreshActivity : IDiscoveryListener
{
    /// <summary>How many times either side of the run waited for YouTube's rate limit.</summary>
    public int RateLimitPauses { get; private set; }

    public RefreshActivity(Action<RefreshProgress> report, TimeProvider time)
    {
        this.report = report;
        this.time = time;
        lock (gate)
        {
            Publish();
        }
    }

    public void ChannelStarted(int position, int total, string channelName)
    {
        lock (gate)
        {
            channel = $"Checking channel {position} of {total} ({channelName})";
            Publish();
        }
    }

    public void VideoQueued()
    {
        lock (gate)
        {
            found++;
            Publish();
        }

        wakeUps.Writer.TryWrite(true);
    }

    public void DiscoveryFinished()
    {
        lock (gate)
        {
            channel = null;
            Publish();
        }

        wakeUps.Writer.TryComplete();
    }

    public void VideoStarted(string title, int done, int total)
    {
        lock (gate)
        {
            activity = title;
            videosDone = done;
            videosTotal = total;
            Publish();
        }
    }

    public async Task WaitOutChannelRateLimitAsync(CancellationToken cancellationToken)
    {
        var pause = StartPause(cancellationToken);
        lock (gate)
        {
            channel = $"YouTube's rate limit was reached, next channel check at {pause.ResumesAt}";
            Publish();
        }

        await pause.Delay.ConfigureAwait(false);
    }

    public async Task WaitOutVideoRateLimitAsync(CancellationToken cancellationToken)
    {
        var pause = StartPause(cancellationToken);
        lock (gate)
        {
            activity = $"YouTube's rate limit was reached, next video at {pause.ResumesAt}";
            Publish();
        }

        await pause.Delay.ConfigureAwait(false);
    }

    /// <summary>Call before looking in the store, so that a video queued after the look still wakes the next wait.</summary>
    public void ForgetEarlierWakeUps()
    {
        while (wakeUps.Reader.TryRead(out _))
        {
        }
    }

    /// <summary>Waits until another video may be waiting. Returns false when discovery has ended and none can arrive.</summary>
    public async Task<bool> WaitForMoreVideosAsync(CancellationToken cancellationToken) =>
        await wakeUps.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false);

    //

    private readonly Action<RefreshProgress> report;
    private readonly TimeProvider time;
    private readonly Lock gate = new();

    private readonly Channel<bool> wakeUps = System.Threading.Channels.Channel.CreateBounded<bool>(
        new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropWrite });

    private string activity = "Looking for new videos";
    private string? channel;
    private int found;
    private int videosDone;
    private int videosTotal;

    /// <summary>Starts the delay before announcing it, so that whoever hears the announcement sees a pause already under way.</summary>
    private Pause StartPause(CancellationToken cancellationToken)
    {
        var resumesAt = TimeZoneInfo.ConvertTime(time.GetUtcNow() + RefreshRunner.RATE_LIMIT_PAUSE, time.LocalTimeZone);
        lock (gate)
        {
            RateLimitPauses++;
        }

        return new Pause(Task.Delay(RefreshRunner.RATE_LIMIT_PAUSE, time, cancellationToken), resumesAt.ToString("HH:mm", CultureInfo.InvariantCulture));
    }

    private void Publish()
    {
        var discovery = channel is null ? null : $"{channel}, {found} new so far";
        report(new RefreshProgress(activity, videosDone, videosTotal, discovery));
    }

    private sealed record Pause(Task Delay, string ResumesAt);
}