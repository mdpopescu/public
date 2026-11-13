namespace YouNews.Core.Refresh;

/// <summary>Hears what the search for new videos is doing while it runs.</summary>
public interface IDiscoveryListener
{
    /// <summary>The search moved on to a channel. Position counts from one.</summary>
    void ChannelStarted(int position, int total, string channelName);

    /// <summary>A video is now waiting in the store to be processed.</summary>
    void VideoQueued();

    /// <summary>YouTube accepted a channel check, whatever else went wrong with it. Ends the run of rate-limit refusals counted so far.</summary>
    void ChannelChecked();

    /// <summary>YouTube refused a channel check because of its rate limit. Returns once the search may ask YouTube again.</summary>
    /// <param name="refusal">What YouTube answered, so the user can see why a run that keeps being refused was stopped.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    Task WaitOutChannelRateLimitAsync(string refusal, CancellationToken cancellationToken);
}