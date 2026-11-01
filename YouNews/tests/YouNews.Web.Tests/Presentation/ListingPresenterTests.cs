using Microsoft.Extensions.Time.Testing;
using YouNews.Core.Model;
using YouNews.Web.Presentation;

namespace YouNews.Web.Tests.Presentation;

[TestClass]
public sealed class ListingPresenterTests
{
    [TestMethod]
    public void DayList_FormatsLabelsCountsAndMarksCurrent()
    {
        var sut = new DayListPresenter();
        var days = new[] { new DayCount(new DateOnly(2026, 11, 1), 6), new DayCount(new DateOnly(2026, 10, 31), 1), new DayCount(new DateOnly(2025, 12, 30), 2) };

        var result = sut.Present(days, new DateOnly(2026, 10, 31));

        Assert.AreEqual("/day/2026-11-01", result[0].Url);
        Assert.AreEqual("Nov 1", result[0].Label);
        Assert.AreEqual("6 videos", result[0].CountText);
        Assert.AreEqual("1 video", result[1].CountText);
        Assert.IsTrue(result[1].IsCurrent);
        Assert.IsFalse(result[0].IsCurrent);
        Assert.AreEqual("Dec 30, 2025", result[2].Label, "older years are spelled out");
    }

    [TestMethod]
    public void DayList_KeepsOnlyTheMostRecentSixty()
    {
        var sut = new DayListPresenter();
        var days = Enumerable.Range(0, 100).Select(offset => new DayCount(new DateOnly(2026, 11, 1).AddDays(-offset), 1)).ToList();

        var result = sut.Present(days, null);

        Assert.HasCount(60, result);
        Assert.AreEqual("2026-11-01", result[0].FullDate);
    }

    [TestMethod]
    public void Refresh_NotRunning_IsAButton()
    {
        Assert.IsFalse(new RefreshPresenter().Present(null).IsRunning);
    }

    [TestMethod]
    public void Refresh_Running_ComputesPercent()
    {
        var view = new RefreshPresenter().Present(new RefreshProgress("Summarizing", 1, 4));

        Assert.IsTrue(view.IsRunning);
        Assert.AreEqual(25, view.Percent);
        Assert.AreEqual("Summarizing", view.Activity);
    }

    [TestMethod]
    public void Refresh_RunningWithUnknownTotal_HasZeroPercent()
    {
        Assert.AreEqual(0, new RefreshPresenter().Present(new RefreshProgress("Starting", 0, 0)).Percent);
    }

    [TestMethod]
    public void Setup_NeverChecked_HasNoData()
    {
        Assert.IsFalse(new SetupPresenter(TimeProvider.System).Present(null).HasData);
    }

    [TestMethod]
    public void Setup_ShowsCheckedAtInLocalTime()
    {
        var time = new FakeTimeProvider();
        time.SetLocalTimeZone(TimeZoneInfo.CreateCustomTimeZone("test", TimeSpan.FromHours(2), "test", "test"));
        var snapshot = new SetupSnapshot(["gemma4-26b"], "RTX 5060 Ti, 62 GB RAM", new DateTimeOffset(2026, 11, 1, 22, 30, 0, TimeSpan.Zero), "server unreachable");

        var view = new SetupPresenter(time).Present(snapshot);

        Assert.AreEqual("2026-11-02 00:30", view.CheckedAtText);
        Assert.AreEqual("server unreachable", view.Warning);
        CollectionAssert.AreEqual(new[] { "gemma4-26b" }, view.Models.ToArray());
    }

    [TestMethod]
    public void Status_DescribesLastRunAndWaitingVideos()
    {
        var time = new FakeTimeProvider();
        time.SetLocalTimeZone(TimeZoneInfo.Utc);
        var run = new RefreshRun(new DateTimeOffset(2026, 11, 1, 6, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 11, 1, 6, 12, 0, TimeSpan.Zero), 3, 1, "server was down");
        var waiting = new[]
        {
            new TrackedVideo(1, 1, "aaa", "First", VideoState.Pending, 2, "no captions", DateTimeOffset.UnixEpoch),
            new TrackedVideo(2, 1, "bbb", "Second", VideoState.GaveUp, 5, null, DateTimeOffset.UnixEpoch),
        };

        var view = new StatusPresenter(time).Present(run, waiting, ["yt-dlp too old"]);

        Assert.AreEqual("Last refresh 2026-11-01 06:12: 3 completed, 1 failed.", view.LastRunText);
        Assert.AreEqual("server was down", view.LastRunProblem);
        Assert.AreEqual("tried 2 times", view.Waiting[0].Detail);
        Assert.AreEqual("no captions", view.Waiting[0].Reason);
        Assert.AreEqual("gave up", view.Waiting[1].Detail);
        Assert.AreEqual("no reason recorded", view.Waiting[1].Reason);
        Assert.AreEqual("https://www.youtube.com/watch?v=aaa", view.Waiting[0].Url);
        CollectionAssert.AreEqual(new[] { "yt-dlp too old" }, view.ToolProblems.ToArray());
    }

    [TestMethod]
    public void Status_NoRunYet_HasNoRunText()
    {
        var view = new StatusPresenter(TimeProvider.System).Present(null, [], []);

        Assert.IsNull(view.LastRunText);
        Assert.IsNull(view.LastRunProblem);
    }
}