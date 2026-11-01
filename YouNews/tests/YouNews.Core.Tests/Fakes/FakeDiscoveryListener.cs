using YouNews.Core.Refresh;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeDiscoveryListener : IDiscoveryListener
{
    public List<string> Channels { get; } = [];

    public int VideosQueued { get; private set; }

    public void ChannelStarted(int position, int total, string channelName) => Channels.Add($"{position}/{total} {channelName}");

    public void VideoQueued() => VideosQueued++;
}