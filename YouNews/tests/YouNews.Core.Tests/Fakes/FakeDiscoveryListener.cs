using YouNews.Core.Refresh;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeDiscoveryListener : IDiscoveryListener
{
    public List<string> Channels { get; } = [];

    public int VideosQueued { get; private set; }

    public int RateLimitWaits { get; private set; }

    /// <summary>Runs during each rate-limit wait, standing in for what changes on YouTube while the search waits.</summary>
    public Action? DuringRateLimitWait { get; set; }

    public void ChannelStarted(int position, int total, string channelName) => Channels.Add($"{position}/{total} {channelName}");

    public void VideoQueued() => VideosQueued++;

    public Task WaitOutChannelRateLimitAsync(CancellationToken cancellationToken)
    {
        RateLimitWaits++;
        DuringRateLimitWait?.Invoke();
        return Task.CompletedTask;
    }
}