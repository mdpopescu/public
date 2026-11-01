using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Keeps the channels being followed.</summary>
public interface IChannelStore
{
    /// <summary>Lists the followed channels, ordered by name.</summary>
    Task<IReadOnlyList<Channel>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Follows a channel. Returns null when it is already followed.</summary>
    Task<Channel?> AddAsync(ChannelDetails details, CancellationToken cancellationToken);

    /// <summary>Stops following a channel. Its existing entries remain.</summary>
    Task RemoveAsync(long channelId, CancellationToken cancellationToken);
}