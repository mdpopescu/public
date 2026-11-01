using YouNews.Core.Model;
using YouNews.Infrastructure.Storage;

namespace YouNews.Infrastructure.Tests.Storage;

[TestClass]
public class ChannelStoreTests
{
    private readonly StorageFixture fixture = new();
    private SqliteChannelStore sut = null!;

    [TestInitialize]
    public async Task Initialize()
    {
        await fixture.InitializeAsync();
        sut = new SqliteChannelStore(fixture.Factory);
    }

    [TestCleanup]
    public void Cleanup() => fixture.Dispose();

    [TestMethod]
    public async Task AddAsync_NewChannel_ReturnsItWithIdAndListsItByName()
    {
        var added = await sut.AddAsync(new ChannelDetails("UCaaa", "https://www.youtube.com/@aaa/", "AAA Channel"), CancellationToken.None);

        Assert.IsNotNull(added);
        Assert.AreEqual("AAA Channel", added.Name);
        var names = (await sut.ListAsync(CancellationToken.None)).Select(channel => channel.Name).ToArray();
        CollectionAssert.AreEqual(new[] { "AAA Channel", "Matthew Berman" }, names);
    }

    [TestMethod]
    public async Task AddAsync_ActivelyFollowedChannel_ReturnsNull()
    {
        var duplicate = await sut.AddAsync(new ChannelDetails("UCawZsQWqfGSbCI5yjkdVkTA", "https://www.youtube.com/@matthew_berman/", "Matthew Berman"), CancellationToken.None);

        Assert.IsNull(duplicate);
        Assert.HasCount(1, (await sut.ListAsync(CancellationToken.None)));
    }

    [TestMethod]
    public async Task RemoveAsync_Channel_NoLongerListed()
    {
        var channel = (await sut.ListAsync(CancellationToken.None)).Single();

        await sut.RemoveAsync(channel.Id, CancellationToken.None);

        Assert.IsEmpty((await sut.ListAsync(CancellationToken.None)));
    }

    [TestMethod]
    public async Task AddAsync_RemovedChannel_ReactivatesItKeepingIdAndVideoStates()
    {
        var (channel, video) = await fixture.AddChannelWithVideoAsync();
        var videos = new SqliteVideoStore(fixture.Factory);
        await videos.SetStateAsync(video.Id, VideoState.Skipped, "members only", CancellationToken.None);
        await sut.RemoveAsync(channel.Id, CancellationToken.None);

        var readded = await sut.AddAsync(new ChannelDetails("UC1", channel.Url, "Renamed"), CancellationToken.None);

        Assert.IsNotNull(readded);
        Assert.AreEqual(channel.Id, readded.Id, "same row should be reactivated");
        Assert.AreEqual("Renamed", readded.Name);
        Assert.IsTrue((await sut.ListAsync(CancellationToken.None)).Any(c => c.Id == channel.Id), "channel should be listed again");
        var skipped = await videos.ListByStateAsync(VideoState.Skipped, CancellationToken.None);
        Assert.AreEqual("members only", skipped.Single().LastReason, "old video state must survive");
    }

    [TestMethod]
    public async Task AddAsync_RemovedThenReaddedTwice_SecondAddReturnsNull()
    {
        var details = new ChannelDetails("UCxyz", "https://www.youtube.com/@xyz/", "XYZ");
        var first = await sut.AddAsync(details, CancellationToken.None);
        await sut.RemoveAsync(first!.Id, CancellationToken.None);
        await sut.AddAsync(details, CancellationToken.None);

        var third = await sut.AddAsync(details, CancellationToken.None);

        Assert.IsNull(third);
    }
}