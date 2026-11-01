using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeChannelStore : IChannelStore
{
    public List<Channel> Channels { get; } = [];

    public Task<IReadOnlyList<Channel>> ListAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Channel>>([.. Channels]);

    public Task<Channel?> AddAsync(ChannelDetails details, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task RemoveAsync(long channelId, CancellationToken cancellationToken) => throw new NotSupportedException();
}