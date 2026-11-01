using YouNews.Core.Model;
using YouNews.Infrastructure.Storage;

namespace YouNews.Infrastructure.Tests.Storage;

[TestClass]
public class VideoStoreTests
{
    [TestInitialize]
    public async Task Initialize()
    {
        await fixture.InitializeAsync();
        sut = new SqliteVideoStore(fixture.Factory);
        channels = new SqliteChannelStore(fixture.Factory);
        channelId = (await channels.ListAsync(CancellationToken.None)).Single().Id;
    }

    [TestCleanup]
    public void Cleanup() => fixture.Dispose();

    [TestMethod]
    public async Task AddAsync_NewVideo_RoundTripsAllFieldsIncludingOffset()
    {
        var seen = new DateTimeOffset(2026, 10, 30, 21, 15, 7, TimeSpan.FromHours(2));

        await sut.AddAsync(new NewVideo(channelId, "abc", "A title", VideoState.Skipped, "members only", seen), CancellationToken.None);

        var video = (await sut.ListByStateAsync(VideoState.Skipped, CancellationToken.None)).Single();
        Assert.AreEqual(new TrackedVideo(video.Id, channelId, "abc", "A title", VideoState.Skipped, 0, "members only", seen), video);
        Assert.AreEqual(seen.Offset, video.FirstSeenAt.Offset, "offset should round-trip");
    }

    [TestMethod]
    public async Task AddAsync_KnownVideo_LeavesItUnchanged()
    {
        var seen = new DateTimeOffset(2026, 10, 30, 8, 0, 0, TimeSpan.Zero);
        await sut.AddAsync(new NewVideo(channelId, "abc", "Original", VideoState.Pending, null, seen), CancellationToken.None);

        await sut.AddAsync(new NewVideo(channelId, "abc", "Changed", VideoState.Skipped, "x", seen.AddDays(1)), CancellationToken.None);

        var video = (await sut.ListByStateAsync(VideoState.Pending, CancellationToken.None)).Single();
        Assert.AreEqual("Original", video.Title);
        Assert.IsEmpty((await sut.ListByStateAsync(VideoState.Skipped, CancellationToken.None)));
    }

    [TestMethod]
    public async Task GetKnownIdsAsync_VideosInAnyState_ReturnsOnlyThatChannelsIds()
    {
        var other = await fixture.AddChannelWithVideoAsync("2");
        var seen = new DateTimeOffset(2026, 10, 30, 8, 0, 0, TimeSpan.Zero);
        await sut.AddAsync(new NewVideo(channelId, "p", "P", VideoState.Pending, null, seen), CancellationToken.None);
        await sut.AddAsync(new NewVideo(channelId, "s", "S", VideoState.Skipped, "private", seen), CancellationToken.None);

        var known = await sut.GetKnownIdsAsync(channelId, CancellationToken.None);

        CollectionAssert.AreEquivalent(new[] { "p", "s" }, known.ToArray());
        CollectionAssert.AreEquivalent(new[] { "vid2" }, (await sut.GetKnownIdsAsync(other.Channel.Id, CancellationToken.None)).ToArray());
    }

    [TestMethod]
    public async Task ListByStateAsync_Videos_NewestFirstSeenFirstTiesByIdDescending()
    {
        var baseTime = new DateTimeOffset(2026, 10, 30, 8, 0, 0, TimeSpan.Zero);
        await sut.AddAsync(new NewVideo(channelId, "old", "Old", VideoState.Pending, null, baseTime), CancellationToken.None);
        await sut.AddAsync(new NewVideo(channelId, "tieA", "Tie A", VideoState.Pending, null, baseTime.AddHours(5)), CancellationToken.None);
        await sut.AddAsync(new NewVideo(channelId, "tieB", "Tie B", VideoState.Pending, null, baseTime.AddHours(5)), CancellationToken.None);
        await sut.AddAsync(new NewVideo(channelId, "viaOffset", "Later in another zone", VideoState.Pending, null, new DateTimeOffset(2026, 10, 30, 20, 0, 0, TimeSpan.FromHours(5))), CancellationToken.None);

        var order = (await sut.ListByStateAsync(VideoState.Pending, CancellationToken.None)).Select(v => v.YouTubeId).ToArray();

        CollectionAssert.AreEqual(new[] { "viaOffset", "tieB", "tieA", "old" }, order);
    }

    [TestMethod]
    public async Task ListByStateAsync_SameDay_MostRecentlyPublishedFirstAndUnknownDatesLast()
    {
        var seen = new DateTimeOffset(2026, 10, 30, 12, 0, 0, TimeSpan.Zero);
        await AddPendingAsync("unknown", seen.AddMinutes(3), null);
        await AddPendingAsync("older", seen.AddMinutes(2), new DateOnly(2026, 10, 1));
        await AddPendingAsync("newest", seen, new DateOnly(2026, 10, 29));
        await AddPendingAsync("middle", seen.AddMinutes(1), new DateOnly(2026, 10, 15));

        var order = (await sut.ListByStateAsync(VideoState.Pending, CancellationToken.None)).Select(v => v.YouTubeId).ToArray();

        CollectionAssert.AreEqual(new[] { "newest", "middle", "older", "unknown" }, order);
    }

    [TestMethod]
    public async Task ListByStateAsync_DifferentDays_LaterDiscoveryWinsOverNewerPublication()
    {
        var seen = new DateTimeOffset(2026, 10, 30, 12, 0, 0, TimeSpan.Zero);
        await AddPendingAsync("found-yesterday", seen, new DateOnly(2026, 10, 30));
        await AddPendingAsync("found-today", seen.AddDays(1), new DateOnly(2026, 10, 2));

        var order = (await sut.ListByStateAsync(VideoState.Pending, CancellationToken.None)).Select(v => v.YouTubeId).ToArray();

        CollectionAssert.AreEqual(new[] { "found-today", "found-yesterday" }, order);
    }

    [TestMethod]
    public async Task SetPublishedOnIfMissingAsync_FillsAGapButKeepsAKnownDate()
    {
        var seen = new DateTimeOffset(2026, 10, 30, 12, 0, 0, TimeSpan.Zero);
        await AddPendingAsync("gap", seen, null);
        await AddPendingAsync("dated", seen, new DateOnly(2026, 10, 10));

        await sut.SetPublishedOnIfMissingAsync("gap", new DateOnly(2026, 10, 20), CancellationToken.None);
        await sut.SetPublishedOnIfMissingAsync("dated", new DateOnly(2026, 10, 25), CancellationToken.None);

        var order = (await sut.ListByStateAsync(VideoState.Pending, CancellationToken.None)).Select(v => v.YouTubeId).ToArray();
        CollectionAssert.AreEqual(new[] { "gap", "dated" }, order, "gap is now 20 October; dated must have stayed at 10 October, not moved to the 25th ahead of it.");
    }

    [TestMethod]
    public async Task ListByStateAsync_VideosOfRemovedChannel_AreLeftOut()
    {
        var (channel, _) = await fixture.AddChannelWithVideoAsync("2");
        await channels.RemoveAsync(channel.Id, CancellationToken.None);

        var pending = await sut.ListByStateAsync(VideoState.Pending, CancellationToken.None);

        Assert.IsEmpty(pending, "videos of a removed channel should not be listed");
    }

    [TestMethod]
    public async Task RecordFailureAsync_Video_IncrementsAttemptsSetsReasonAndStaysPending()
    {
        var (_, video) = await fixture.AddChannelWithVideoAsync();

        await sut.RecordFailureAsync(video.Id, "no captions", CancellationToken.None);
        await sut.RecordFailureAsync(video.Id, "LLM timed out", CancellationToken.None);

        var updated = (await sut.ListByStateAsync(VideoState.Pending, CancellationToken.None)).Single();
        Assert.AreEqual(2, updated.Attempts);
        Assert.AreEqual("LLM timed out", updated.LastReason);
    }

    [TestMethod]
    public async Task SetStateAsync_Video_ChangesStateAndReasonWithoutCountingAnAttempt()
    {
        var (_, video) = await fixture.AddChannelWithVideoAsync();

        await sut.SetStateAsync(video.Id, VideoState.Recheck, "live stream not finished", CancellationToken.None);

        Assert.IsEmpty((await sut.ListByStateAsync(VideoState.Pending, CancellationToken.None)));
        var rechecked = (await sut.ListByStateAsync(VideoState.Recheck, CancellationToken.None)).Single();
        Assert.AreEqual("live stream not finished", rechecked.LastReason);
        Assert.AreEqual(0, rechecked.Attempts);
    }

    [TestMethod]
    public async Task SetStateAsync_NullReason_ClearsEarlierReason()
    {
        var (_, video) = await fixture.AddChannelWithVideoAsync();
        await sut.RecordFailureAsync(video.Id, "boom", CancellationToken.None);

        await sut.SetStateAsync(video.Id, VideoState.Pending, null, CancellationToken.None);

        Assert.IsNull((await sut.ListByStateAsync(VideoState.Pending, CancellationToken.None)).Single().LastReason);
    }

    //

    private readonly StorageFixture fixture = new();
    private SqliteVideoStore sut = null!;
    private SqliteChannelStore channels = null!;
    private long channelId;

    private Task AddPendingAsync(string id, DateTimeOffset seen, DateOnly? publishedOn) =>
        sut.AddAsync(new NewVideo(channelId, id, id, VideoState.Pending, null, seen, publishedOn), CancellationToken.None);
}