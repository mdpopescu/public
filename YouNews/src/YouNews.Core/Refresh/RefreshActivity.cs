using System.Globalization;
using System.Threading.Channels;
using YouNews.Core.Model;

namespace YouNews.Core.Refresh;

/// <summary>
/// Joins the two sides of a run: combines their progress into one report, and wakes the consumer when the producer queues a video.
/// The videos themselves wait in the store, because the order they are taken in can change as more are discovered.
/// </summary>
internal sealed class RefreshActivity : IDiscoveryListener, IDisposable
{
    /// <summary>How many times either side of the run waited for YouTube's rate limit.</summary>
    public int RateLimitPauses { get; private set; }

    /// <summary>Cancelled once YouTube has refused <see cref="RefreshRunner.MAXIMUM_RATE_LIMIT_PAUSES_IN_A_ROW" /> requests in a row, so that whatever is running can stop.</summary>
    public CancellationToken RateLimitStop => stoppedByRateLimit.Token;

    /// <summary>True once the run has to end because YouTube kept refusing it. Stays true even if a request is accepted afterwards.</summary>
    public bool StoppedByRateLimit => stoppedByRateLimit.IsCancellationRequested;

    /// <summary>What YouTube answered the last time its rate limit refused either side of the run, or null if it never did.</summary>
    public string? LastRefusal
    {
        get
        {
            lock (gate)
            {
                return lastRefusal;
            }
        }
    }

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

    public void ChannelChecked() => EndRefusals();

    /// <summary>A video was processed without YouTube's rate limit refusing it. Ends the run of refusals counted so far.</summary>
    public void VideoProcessed() => EndRefusals();

    /// <summary>
    /// Counts the refusal and waits it out. The refusal that reaches the limit is not waited out: the run stops instead,
    /// and the search, which holds the token this is called with, is cancelled.
    /// </summary>
    public async Task WaitOutChannelRateLimitAsync(string refusal, CancellationToken cancellationToken)
    {
        if (await CountRefusalAsync(refusal).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return;
        }

        var pause = StartPause(cancellationToken);
        lock (gate)
        {
            channel = $"YouTube's rate limit was reached, next channel check at {pause.ResumesAt}";
            Publish();
        }

        await pause.Delay.WaitAsync(stoppedByRateLimit.Token).ConfigureAwait(false);
    }

    /// <summary>Counts the refusal and waits it out, unless it reaches the limit: then it returns at once and <see cref="StoppedByRateLimit" /> is true.</summary>
    public async Task WaitOutVideoRateLimitAsync(string refusal, CancellationToken cancellationToken)
    {
        if (await CountRefusalAsync(refusal).ConfigureAwait(false))
            return;

        var pause = StartPause(cancellationToken);
        lock (gate)
        {
            activity = $"YouTube's rate limit was reached, next video at {pause.ResumesAt}";
            Publish();
        }

        await WaitOutAsync(pause, cancellationToken).ConfigureAwait(false);
    }

    public void Dispose() => stoppedByRateLimit.Dispose();

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

    private readonly CancellationTokenSource stoppedByRateLimit = new();
    private int refusalsInARow;
    private string? lastRefusal;
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

    /// <summary>Ends early, without throwing, when the other side stops the run during the pause.</summary>
    private async Task WaitOutAsync(Pause pause, CancellationToken cancellationToken)
    {
        try
        {
            await pause.Delay.WaitAsync(stoppedByRateLimit.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (StoppedByRateLimit && !cancellationToken.IsCancellationRequested)
        {
        }
    }

    /// <summary>Returns true when this refusal, or an earlier one, has stopped the run.</summary>
    private async Task<bool> CountRefusalAsync(string refusal)
    {
        bool reached;
        lock (gate)
        {
            lastRefusal = refusal;
            refusalsInARow++;
            reached = refusalsInARow >= RefreshRunner.MAXIMUM_RATE_LIMIT_PAUSES_IN_A_ROW;
        }

        if (reached)
            await stoppedByRateLimit.CancelAsync().ConfigureAwait(false);

        return StoppedByRateLimit;
    }

    private void EndRefusals()
    {
        lock (gate)
        {
            refusalsInARow = 0;
        }
    }

    private void Publish()
    {
        var discovery = channel is null ? null : $"{channel}, {found} new so far";
        report(new RefreshProgress(activity, videosDone, videosTotal, discovery));
    }

    private sealed record Pause(Task Delay, string ResumesAt);
}