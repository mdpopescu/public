using YouNews.Core.Model;
using YouNews.Infrastructure.Storage;

namespace YouNews.Infrastructure.Tests.Storage;

[TestClass]
public class ReprocessQueueTests
{
    [TestInitialize]
    public async Task Initialize()
    {
        await fixture.InitializeAsync();
        sut = new SqliteReprocessQueue(fixture.Factory);
        entries = new SqliteEntryStore(fixture.Factory);
    }

    [TestCleanup]
    public void Cleanup() => fixture.Dispose();

    [TestMethod]
    public async Task GetNextAsync_NothingRequested_ReturnsNull()
    {
        Assert.IsNull(await sut.GetNextAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task GetNextAsync_RequestedEntry_ReturnsVideoTitleAndChannelName()
    {
        var entryId = await AddEntryAsync("1");

        await sut.RequestAsync(entryId, CancellationToken.None);

        var job = await sut.GetNextAsync(CancellationToken.None);
        Assert.AreEqual(new ReprocessJob(entryId, "vid1", "Video 1", "Channel 1"), job);
    }

    [TestMethod]
    public async Task GetNextAsync_ChannelRemoved_StillReturnsTheJob()
    {
        var (channel, video) = await fixture.AddChannelWithVideoAsync();
        var entryId = await entries.AddAsync(NewEntry(video.Id), CancellationToken.None);
        await sut.RequestAsync(entryId, CancellationToken.None);
        await new SqliteChannelStore(fixture.Factory).RemoveAsync(channel.Id, CancellationToken.None);

        var job = await sut.GetNextAsync(CancellationToken.None);

        Assert.AreEqual("Channel 1", job!.ChannelName);
    }

    [TestMethod]
    public async Task RequestAsync_UnknownEntry_DoesNothing()
    {
        await sut.RequestAsync(9999, CancellationToken.None);

        Assert.IsNull(await sut.GetNextAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task RequestAsync_AlreadyQueued_StaysASingleRequest()
    {
        var entryId = await AddEntryAsync("1");
        await sut.RequestAsync(entryId, CancellationToken.None);

        await sut.RequestAsync(entryId, CancellationToken.None);

        Assert.AreEqual(entryId, (await sut.GetNextAsync(CancellationToken.None))!.EntryId);
        await sut.RecordFailureAsync(entryId, "failed", CancellationToken.None);
        Assert.IsNull(await sut.GetNextAsync(CancellationToken.None), "a second request row would still be queued");
    }

    [TestMethod]
    public async Task GetNextAsync_SeveralRequests_ReturnsTheOldestFirst()
    {
        var first = await AddEntryAsync("1");
        var second = await AddEntryAsync("2");
        await sut.RequestAsync(second, CancellationToken.None);
        await sut.RequestAsync(first, CancellationToken.None);

        var job = await sut.GetNextAsync(CancellationToken.None);

        Assert.AreEqual(second, job!.EntryId);
    }

    [TestMethod]
    public async Task GetNextAsync_FailedRequest_IsSkipped()
    {
        var first = await AddEntryAsync("1");
        var second = await AddEntryAsync("2");
        await sut.RequestAsync(first, CancellationToken.None);
        await sut.RequestAsync(second, CancellationToken.None);

        await sut.RecordFailureAsync(first, "model is down", CancellationToken.None);

        Assert.AreEqual(second, (await sut.GetNextAsync(CancellationToken.None))!.EntryId);
    }

    [TestMethod]
    public async Task RequestAsync_FailedRequest_IsQueuedAgainKeepingItsPlace()
    {
        var first = await AddEntryAsync("1");
        var second = await AddEntryAsync("2");
        await sut.RequestAsync(first, CancellationToken.None);
        await sut.RequestAsync(second, CancellationToken.None);
        await sut.RecordFailureAsync(first, "model is down", CancellationToken.None);

        await sut.RequestAsync(first, CancellationToken.None);

        Assert.AreEqual(first, (await sut.GetNextAsync(CancellationToken.None))!.EntryId, "the request keeps its id, so it is still the oldest");
    }

    //

    private readonly StorageFixture fixture = new();
    private SqliteReprocessQueue sut = null!;
    private SqliteEntryStore entries = null!;

    private async Task<long> AddEntryAsync(string suffix)
    {
        var (_, video) = await fixture.AddChannelWithVideoAsync(suffix);
        return await entries.AddAsync(NewEntry(video.Id), CancellationToken.None);
    }

    private static NewEntry NewEntry(long videoId) =>
        new(videoId, new DateOnly(2026, 10, 30), "Headline", ["research"], "Summary", "Opinion", [], [], []);
}