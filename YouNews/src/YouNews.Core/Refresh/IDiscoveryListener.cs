namespace YouNews.Core.Refresh;

/// <summary>Hears what the search for new videos is doing while it runs.</summary>
public interface IDiscoveryListener
{
    /// <summary>The search moved on to a channel. Position counts from one.</summary>
    void ChannelStarted(int position, int total, string channelName);

    /// <summary>A video is now waiting in the store to be processed.</summary>
    void VideoQueued();
}