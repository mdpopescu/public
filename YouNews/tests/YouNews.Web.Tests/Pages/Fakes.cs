using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Core.Refresh;

namespace YouNews.Web.Tests.Pages;

public sealed class FakeEntryReader : IEntryReader
{
    public List<Entry> Entries { get; } = [];

    public Task<IReadOnlyList<DayCount>> ListDaysAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<DayCount>>(Entries.GroupBy(entry => entry.Day).OrderByDescending(group => group.Key).Select(group => new DayCount(group.Key, group.Count())).ToList());

    public Task<IReadOnlyList<Entry>> ListForDayAsync(DateOnly day, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Entry>>(Entries.Where(entry => entry.Day == day).ToList());
}

public sealed class FakeChannelStore : IChannelStore
{
    public List<Channel> Channels { get; } = [];

    public Task<IReadOnlyList<Channel>> ListAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Channel>>(Channels.ToList());

    public Task<Channel?> AddAsync(ChannelDetails details, CancellationToken cancellationToken)
    {
        if (Channels.Any(channel => channel.Url == details.Url))
            return Task.FromResult<Channel?>(null);

        var channel = new Channel(Channels.Count + 1, details.Url, details.Name);
        Channels.Add(channel);
        return Task.FromResult<Channel?>(channel);
    }

    public Task RemoveAsync(long channelId, CancellationToken cancellationToken)
    {
        Channels.RemoveAll(channel => channel.Id == channelId);
        return Task.CompletedTask;
    }
}

public sealed class FakeInterestStore : IInterestStore
{
    public List<string> Interests { get; private set; } = [];

    public Task<IReadOnlyList<string>> GetAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<string>>(Interests.ToList());

    public Task SaveAsync(IReadOnlyList<string> interests, CancellationToken cancellationToken)
    {
        Interests = interests.ToList();
        return Task.CompletedTask;
    }
}

public sealed class FakeRefreshLog : IRefreshLog
{
    public RefreshRun? Latest { get; set; }

    public Task RecordAsync(RefreshRun run, CancellationToken cancellationToken)
    {
        Latest = run;
        return Task.CompletedTask;
    }

    public Task<RefreshRun?> GetLatestAsync(CancellationToken cancellationToken) => Task.FromResult(Latest);
}

public sealed class FakeRunAlertStore : IRunAlertStore
{
    public RunAlert? Current { get; set; }

    public Task RaiseAsync(string message, DateTimeOffset raisedAt, CancellationToken cancellationToken)
    {
        Current = new RunAlert(message, raisedAt);
        return Task.CompletedTask;
    }

    public Task<RunAlert?> GetAsync(CancellationToken cancellationToken) => Task.FromResult(Current);

    public Task DismissAsync(CancellationToken cancellationToken)
    {
        Current = null;
        return Task.CompletedTask;
    }
}

public sealed class FakeModelSelection : ICurrentModel, IModelSelector
{
    public string Model { get; set; } = "gemma4-26b";

    public Task<string> GetAsync(CancellationToken cancellationToken) => Task.FromResult(Model);

    public Task SelectAsync(string model, CancellationToken cancellationToken)
    {
        Model = model;
        return Task.CompletedTask;
    }
}

public sealed class FakeSetupStore : ISetupStore
{
    public SetupSnapshot? Snapshot { get; set; }

    public Task<SetupSnapshot?> GetLatestAsync(CancellationToken cancellationToken) => Task.FromResult(Snapshot);

    public Task SaveAsync(SetupSnapshot snapshot, CancellationToken cancellationToken)
    {
        Snapshot = snapshot;
        return Task.CompletedTask;
    }
}

public sealed class FakeToolHealth : IToolHealth
{
    public List<string> Problems { get; } = [];

    public Task<IReadOnlyList<string>> FindProblemsAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<string>>(Problems.ToList());
}

public sealed class FakeVideoCatalog : IVideoCatalog
{
    public Dictionary<string, ChannelDetails> Known { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Exception? Failure { get; set; }

    public Task<ChannelDetails?> ResolveChannelAsync(string urlOrHandle, CancellationToken cancellationToken)
    {
        if (Failure is not null)
            throw Failure;

        return Task.FromResult(Known.GetValueOrDefault(urlOrHandle));
    }

    public Task<IReadOnlyList<IReadOnlyList<FeedVideo>>> ListVideoFeedsAsync(Channel channel, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<VideoDetails> GetDetailsAsync(string youTubeId, CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}

public sealed class FakeVideoStore : IVideoStore
{
    public List<TrackedVideo> Videos { get; } = [];

    public Task<IReadOnlySet<string>> GetKnownIdsAsync(long channelId, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task AddAsync(NewVideo video, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task SetPublishedOnIfMissingAsync(string youTubeId, DateOnly publishedOn, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<TrackedVideo>> ListByStateAsync(VideoState state, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<TrackedVideo>>(Videos.Where(video => video.State == state).ToList());

    public Task DeferAsync(long videoId, DateTimeOffset deferredAt, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task SetStateAsync(long videoId, VideoState state, string? reason, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task RecordFailureAsync(long videoId, string reason, CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}

public sealed class FakeRefresh : IRefreshTrigger, IRefreshStatus
{
    public int StartCount { get; private set; }

    public RefreshProgress? Current { get; set; }

    public bool TryStart()
    {
        StartCount++;
        return true;
    }
}

public sealed class FakeReprocessTrigger : IReprocessTrigger
{
    public List<long> Requested { get; } = [];

    public Task RequestAsync(long entryId, CancellationToken cancellationToken)
    {
        Requested.Add(entryId);
        return Task.CompletedTask;
    }
}

public sealed class IdleReprocessWorker : IReprocessWorker
{
    public async Task RunAsync(CancellationToken stopping)
    {
        try
        {
            await Task.Delay(Timeout.Infinite, stopping);
        }
        catch (OperationCanceledException)
        {
        }
    }
}