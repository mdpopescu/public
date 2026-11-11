using YouNews.Core.Model;
using YouNews.Web.Presentation;

namespace YouNews.Web.Tests.Presentation;

[TestClass]
public sealed class StatusPresenterTests
{
    [TestMethod]
    public void Present_NoPendingVideos_ShowsNoQueue()
    {
        var sut = new StatusPresenter(TimeProvider.System);

        var view = sut.Present(null, [], [], PendingQueue.EMPTY);

        Assert.IsNull(view.QueueText, "nothing should be said when no video is pending");
        Assert.IsEmpty(view.Queue, "no queued videos expected");
    }

    [TestMethod]
    public void Present_OnePendingVideo_UsesSingular()
    {
        var sut = new StatusPresenter(TimeProvider.System);

        var view = sut.Present(null, [], [], QueueOf(1));

        Assert.AreEqual("1 video waiting to be processed", view.QueueText);
    }

    [TestMethod]
    public void Present_ManyPendingVideos_ShowsTotalAndOnlyFirstTenInOrder()
    {
        var sut = new StatusPresenter(TimeProvider.System);

        var view = sut.Present(null, [], [], QueueOf(407));

        Assert.AreEqual("407 videos waiting to be processed", view.QueueText);
        var titles = view.Queue.Select(video => video.Title).ToArray();
        CollectionAssert.AreEqual(
            Enumerable.Range(1, 10).Select(number => $"Video {number}").ToArray(),
            titles,
            $"expected the first ten in queue order but got: {string.Join(" | ", titles)}");
    }

    [TestMethod]
    public void Present_ExactlyTenPendingVideos_ShowsAll()
    {
        var sut = new StatusPresenter(TimeProvider.System);

        var view = sut.Present(null, [], [], QueueOf(10));

        Assert.HasCount(10, view.Queue, "all ten should be listed");
        Assert.AreEqual("10 videos waiting to be processed", view.QueueText);
    }

    [TestMethod]
    public void Present_QueuedVideo_ShowsChannelAndPublishDate()
    {
        var sut = new StatusPresenter(TimeProvider.System);
        var video = new TrackedVideo(1, 7, "abc 1", "Big news", VideoState.Pending, 0, null, DateTimeOffset.UnixEpoch, new DateOnly(2026, 10, 31));
        var queue = new PendingQueue([video], new Dictionary<long, string> { [7] = "Locally" });

        var view = sut.Present(null, [], [], queue);

        Assert.AreEqual("Locally, 2026-10-31", view.Queue[0].Detail);
        Assert.AreEqual("https://www.youtube.com/watch?v=abc%201", view.Queue[0].Url);
    }

    [TestMethod]
    public void Present_QueuedVideoWithoutDate_ShowsChannelOnly()
    {
        var sut = new StatusPresenter(TimeProvider.System);
        var video = new TrackedVideo(1, 7, "abc", "Big news", VideoState.Pending, 0, null, DateTimeOffset.UnixEpoch);
        var queue = new PendingQueue([video], new Dictionary<long, string> { [7] = "Locally" });

        var view = sut.Present(null, [], [], queue);

        Assert.AreEqual("Locally", view.Queue[0].Detail);
    }

    [TestMethod]
    public void Present_QueuedVideoOfUnknownChannelAndNoDate_HasEmptyDetail()
    {
        var sut = new StatusPresenter(TimeProvider.System);
        var video = new TrackedVideo(1, 99, "abc", "Big news", VideoState.Pending, 0, null, DateTimeOffset.UnixEpoch);

        var view = sut.Present(null, [], [], new PendingQueue([video], new Dictionary<long, string>()));

        Assert.AreEqual("", view.Queue[0].Detail);
    }

    [TestMethod]
    public void Present_QueueAndFailedVideos_KeepsBothLists()
    {
        var sut = new StatusPresenter(TimeProvider.System);
        var failed = new TrackedVideo(50, 1, "fff", "Failed one", VideoState.Pending, 2, "no captions", DateTimeOffset.UnixEpoch);

        var view = sut.Present(null, [failed], [], QueueOf(3));

        Assert.HasCount(1, view.Waiting, "the retry list must stay as before");
        Assert.AreEqual("tried 2 times", view.Waiting[0].Detail);
        Assert.HasCount(3, view.Queue);
    }

    //

    private static PendingQueue QueueOf(int count) =>
        new(
            Enumerable.Range(1, count)
                .Select(number => new TrackedVideo(number, 1, $"id{number}", $"Video {number}", VideoState.Pending, 0, null, DateTimeOffset.UnixEpoch))
                .ToList(),
            new Dictionary<long, string> { [1] = "Channel" });
}