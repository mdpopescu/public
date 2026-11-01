using Microsoft.Data.Sqlite;
using YouNews.Core.Model;
using YouNews.Infrastructure.Storage;

namespace YouNews.Infrastructure.Tests.Storage;

[TestClass]
public class EntryStoreTests
{
    private readonly StorageFixture fixture = new();
    private SqliteEntryStore sut = null!;
    private SqliteEntryReader reader = null!;
    private SqliteVideoStore videos = null!;

    [TestInitialize]
    public async Task Initialize()
    {
        await fixture.InitializeAsync();
        sut = new SqliteEntryStore(fixture.Factory);
        reader = new SqliteEntryReader(fixture.Factory);
        videos = new SqliteVideoStore(fixture.Factory);
    }

    [TestCleanup]
    public void Cleanup() => fixture.Dispose();

    [TestMethod]
    public async Task AddAsync_FullEntry_RoundTripsThroughReader()
    {
        var (_, video) = await fixture.AddChannelWithVideoAsync();
        var first = await sut.AddAsync(Entry(video.Id, new DateOnly(2026, 10, 30), "First"), CancellationToken.None);
        var (_, second) = await fixture.AddChannelWithVideoAsync("2");
        var newEntry = Entry(second.Id, new DateOnly(2026, 10, 31), "Second", related: [first]);

        var id = await sut.AddAsync(newEntry, CancellationToken.None);

        var entry = (await reader.ListForDayAsync(new DateOnly(2026, 10, 31), CancellationToken.None)).Single();
        Assert.AreEqual(id, entry.Id);
        Assert.AreEqual("Channel 2", entry.ChannelName);
        Assert.AreEqual("vid2", entry.VideoYouTubeId);
        Assert.AreEqual("Second", entry.Headline);
        CollectionAssert.AreEqual(new[] { "local models", "research" }, entry.Tags.ToArray());
        Assert.AreEqual("Summary of Second", entry.Summary);
        Assert.AreEqual("Opinion on Second", entry.Opinion);
        CollectionAssert.AreEqual(new[] { new WebSource("Source A", "https://a.example"), new WebSource("Source B", "https://b.example") }, entry.WebSources.ToArray());
        CollectionAssert.AreEqual(new[] { new EntryImage("vid/thumb.jpg", ImageKind.Thumbnail), new EntryImage("vid/f1.jpg", ImageKind.Frame) }, entry.Images.ToArray());
        CollectionAssert.AreEqual(new[] { new RelatedLink(first, new DateOnly(2026, 10, 30), "First") }, entry.Related.ToArray());
    }

    [TestMethod]
    public async Task AddAsync_Entry_MarksVideoDoneAndClearsLastReason()
    {
        var (_, video) = await fixture.AddChannelWithVideoAsync();
        await videos.RecordFailureAsync(video.Id, "first try failed", CancellationToken.None);

        await sut.AddAsync(Entry(video.Id, new DateOnly(2026, 10, 30), "Head"), CancellationToken.None);

        Assert.IsEmpty((await videos.ListByStateAsync(VideoState.Pending, CancellationToken.None)));
        var done = (await videos.ListByStateAsync(VideoState.Done, CancellationToken.None)).Single();
        Assert.IsNull(done.LastReason);
        Assert.AreEqual(1, done.Attempts, "attempt count is history and stays");
    }

    [TestMethod]
    public async Task AddAsync_UnknownRelatedId_RollsBackEntryAndVideoState()
    {
        var (_, video) = await fixture.AddChannelWithVideoAsync();

        await Assert.ThrowsAsync<SqliteException>(() => sut.AddAsync(Entry(video.Id, new DateOnly(2026, 10, 30), "Head", related: [9999]), CancellationToken.None));

        Assert.IsEmpty((await reader.ListDaysAsync(CancellationToken.None)), "no entry should remain");
        Assert.HasCount(1, (await videos.ListByStateAsync(VideoState.Pending, CancellationToken.None)), "video should still be pending");
    }

    [TestMethod]
    public async Task AddAsync_SecondEntryForSameVideo_ThrowsAndKeepsFirstIntact()
    {
        var (_, video) = await fixture.AddChannelWithVideoAsync();
        await sut.AddAsync(Entry(video.Id, new DateOnly(2026, 10, 30), "First"), CancellationToken.None);

        await Assert.ThrowsAsync<SqliteException>(() => sut.AddAsync(Entry(video.Id, new DateOnly(2026, 10, 30), "Duplicate"), CancellationToken.None));

        var entries = await reader.ListForDayAsync(new DateOnly(2026, 10, 30), CancellationToken.None);
        Assert.AreEqual("First", entries.Single().Headline);
        Assert.HasCount(2, entries.Single().Tags, "tags of the first entry must not be doubled or lost");
    }

    [TestMethod]
    public async Task ListDaysAsync_EntriesOnSeveralDays_NewestDayFirstWithCounts()
    {
        await AddEntryAsync("1", new DateOnly(2026, 10, 29));
        await AddEntryAsync("2", new DateOnly(2026, 10, 31));
        await AddEntryAsync("3", new DateOnly(2026, 10, 31));

        var days = await reader.ListDaysAsync(CancellationToken.None);

        CollectionAssert.AreEqual(new[] { new DayCount(new DateOnly(2026, 10, 31), 2), new DayCount(new DateOnly(2026, 10, 29), 1) }, days.ToArray());
    }

    [TestMethod]
    public async Task ListForDayAsync_SeveralEntries_MostRecentlyAddedFirst()
    {
        var day = new DateOnly(2026, 10, 31);
        await AddEntryAsync("1", day);
        await AddEntryAsync("2", day);
        await AddEntryAsync("3", new DateOnly(2026, 10, 30));

        var headlines = (await reader.ListForDayAsync(day, CancellationToken.None)).Select(e => e.Headline).ToArray();

        CollectionAssert.AreEqual(new[] { "Headline 2", "Headline 1" }, headlines);
    }

    [TestMethod]
    public async Task ListForDayAsync_ChannelRemoved_EntryStillShownWithChannelName()
    {
        var day = new DateOnly(2026, 10, 31);
        var (channel, video) = await fixture.AddChannelWithVideoAsync();
        await sut.AddAsync(Entry(video.Id, day, "Kept"), CancellationToken.None);

        await new SqliteChannelStore(fixture.Factory).RemoveAsync(channel.Id, CancellationToken.None);

        var entry = (await reader.ListForDayAsync(day, CancellationToken.None)).Single();
        Assert.AreEqual("Channel 1", entry.ChannelName);
        Assert.AreEqual(1, (await reader.ListDaysAsync(CancellationToken.None)).Single().EntryCount);
    }

    [TestMethod]
    public async Task ListForDayAsync_NoEntries_ReturnsEmpty()
    {
        var entries = await reader.ListForDayAsync(new DateOnly(2026, 1, 1), CancellationToken.None);

        Assert.IsEmpty(entries);
    }

    [TestMethod]
    public async Task ListRecentDigestsAsync_ManyEntries_NewestFirstLimitedWithTags()
    {
        await AddEntryAsync("1", new DateOnly(2026, 10, 29));
        await AddEntryAsync("2", new DateOnly(2026, 10, 31));
        await AddEntryAsync("3", new DateOnly(2026, 10, 30));

        var digests = await sut.ListRecentDigestsAsync(2, CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "Headline 2", "Headline 3" }, digests.Select(d => d.Headline).ToArray());
        CollectionAssert.AreEqual(new[] { "local models", "research" }, digests[0].Tags.ToArray());
        Assert.AreEqual("Summary of Headline 2", digests[0].Summary);
        Assert.AreEqual(new DateOnly(2026, 10, 31), digests[0].Day);
    }

    //

    private static NewEntry Entry(long videoId, DateOnly day, string headline, IReadOnlyList<long>? related = null) => new(
        videoId,
        day,
        headline,
        ["local models", "research"],
        $"Summary of {headline}",
        $"Opinion on {headline}",
        [new WebSource("Source A", "https://a.example"), new WebSource("Source B", "https://b.example")],
        [new EntryImage("vid/thumb.jpg", ImageKind.Thumbnail), new EntryImage("vid/f1.jpg", ImageKind.Frame)],
        related ?? []);

    private async Task AddEntryAsync(string suffix, DateOnly day)
    {
        var (_, video) = await fixture.AddChannelWithVideoAsync(suffix);
        await sut.AddAsync(Entry(video.Id, day, $"Headline {suffix}"), CancellationToken.None);
    }
}