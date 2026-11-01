using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using YouNews.Core.Model;

namespace YouNews.Web.Tests.Pages;

[TestClass]
public sealed class DayPageTests
{
    [TestInitialize]
    public void Initialize()
    {
        sut = new TestApp();
        browser = sut.CreateBrowser();
    }

    [TestCleanup]
    public void Cleanup()
    {
        browser.Dispose();
        sut.Dispose();
    }

    [TestMethod]
    public async Task Home_ShowsTheLatestDayWithItsStory()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(1, new DateOnly(2026, 10, 31), headline: "Older news"));
        sut.Entries.Entries.Add(TestApp.MakeEntry(2, new DateOnly(2026, 11, 1), headline: "A new 30B model", summary: "First paragraph.\n\nSecond paragraph.", opinion: "Worth trying."));

        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, "<h1>AI News for 2026-11-01</h1>");
        StringAssert.Contains(html, "A new 30B model");
        StringAssert.Contains(html, "Sample Channel One");
        StringAssert.Contains(html, "<p>First paragraph.</p>");
        StringAssert.Contains(html, "<p>Second paragraph.</p>");
        StringAssert.Contains(html, "Worth trying.");
        StringAssert.Contains(html, "id=\"entry-2\"");
        StringAssert.Contains(html, "href=\"https://www.youtube.com/watch?v=vid2\"");
        Assert.DoesNotContain("Older news", html, "only the latest day is shown on the home page");
    }

    [TestMethod]
    public async Task DayPage_ListsDaysWithCountsNewestFirst()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(1, new DateOnly(2026, 10, 31)));
        sut.Entries.Entries.Add(TestApp.MakeEntry(2, new DateOnly(2026, 11, 1)));
        sut.Entries.Entries.Add(TestApp.MakeEntry(3, new DateOnly(2026, 11, 1)));

        var html = await browser.GetStringAsync("/day/2026-10-31");

        StringAssert.Contains(html, "href=\"/day/2026-11-01\"");
        StringAssert.Contains(html, "2 videos");
        Assert.IsLessThan(html.IndexOf("/day/2026-10-31", StringComparison.Ordinal), html.IndexOf("/day/2026-11-01", StringComparison.Ordinal), "newest day first");
        StringAssert.Contains(html, "<h1>AI News for 2026-10-31</h1>");
    }

    [TestMethod]
    public async Task ModelText_IsHtmlEncoded()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(1, new DateOnly(2026, 11, 1), headline: "<b>bold</b>", summary: "Hello <script>alert('x')</script> there", opinion: "<img src=x onerror=alert(1)>"));

        var html = await browser.GetStringAsync("/");

        Assert.DoesNotContain("<script>alert", html, "model text must never become markup");
        Assert.DoesNotContain("<img src=x", html);
        StringAssert.Contains(html, "&lt;script&gt;alert(&#x27;x&#x27;)&lt;/script&gt;");
        StringAssert.Contains(html, "&lt;b&gt;bold&lt;/b&gt;");
    }

    [TestMethod]
    public async Task Story_RelatedLinksPointToTheDayAnchor()
    {
        var entry = TestApp.MakeEntry(5, new DateOnly(2026, 11, 1)) with { Related = [new RelatedLink(3, new DateOnly(2026, 10, 28), "The previous version")] };
        sut.Entries.Entries.Add(entry);

        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, "<a href=\"/day/2026-10-28#entry-3\">Oct 28: The previous version</a>");
    }

    [TestMethod]
    public async Task Story_SourcesOpenInANewTabWithoutOpener()
    {
        var entry = TestApp.MakeEntry(5, new DateOnly(2026, 11, 1)) with { WebSources = [new WebSource("Vendor blog", "https://example.com/post")] };
        sut.Entries.Entries.Add(entry);

        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, "Sources checked");
        StringAssert.Contains(html, "<a href=\"https://example.com/post\" target=\"_blank\" rel=\"noopener noreferrer\">Vendor blog</a>");
    }

    [TestMethod]
    public async Task Story_WithImagesWhoseFilesAreMissing_StillRendersAndHidesBrokenOnes()
    {
        var images = new[] { new EntryImage("2026/11/gone.jpg", ImageKind.Thumbnail), new EntryImage("2026/11/frame.jpg", ImageKind.Frame) };
        sut.Entries.Entries.Add(TestApp.MakeEntry(5, new DateOnly(2026, 11, 1)) with { Images = images });

        var response = await browser.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, html);
        StringAssert.Contains(html, "src=\"/images/2026/11/gone.jpg\"");
        StringAssert.Contains(html, "src=\"/images/2026/11/frame.jpg\"");
        StringAssert.Contains(html, "onerror=\"this.style.display='none'\"");
        StringAssert.Contains(html, "class=\"art tag-local-models\"");
    }

    [TestMethod]
    public async Task Story_OffTopic_IsQuietAndKeepsOpinionCollapsed()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(5, new DateOnly(2026, 11, 1), opinion: "Not relevant.", tags: [EntryTags.OFF_TOPIC]));

        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, "story off-topic");
        StringAssert.Contains(html, "<details class=\"opinion\">");
        StringAssert.Contains(html, "Not relevant.");
    }

    [TestMethod]
    public async Task DayPage_With100Entries_StillRenders()
    {
        for (var id = 1; id <= 100; id++)
            sut.Entries.Entries.Add(TestApp.MakeEntry(id, new DateOnly(2026, 11, 1), headline: "Story number " + id));

        var response = await browser.GetAsync("/day/2026-11-01");
        var html = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        StringAssert.Contains(html, "id=\"entry-100\"");
        StringAssert.Contains(html, "Story number 1<");
    }

    [TestMethod]
    [DataRow("/day/not-a-date")]
    [DataRow("/day/2026-13-45")]
    [DataRow("/day/20261101")]
    public async Task DayPage_MalformedDate_IsAFriendly404(string url)
    {
        var response = await browser.GetAsync(url);
        var html = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        StringAssert.Contains(html, "href=\"/\"");
        StringAssert.Contains(html, "Back to the latest news");
    }

    [TestMethod]
    public async Task DayPage_ValidDateWithoutEntries_IsAFriendly404()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(1, new DateOnly(2026, 11, 1)));

        var response = await browser.GetAsync("/day/2026-05-05");
        var html = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        StringAssert.Contains(html, "There are no entries for 2026-05-05.");
        StringAssert.Contains(html, "Back to the latest news");
    }

    [TestMethod]
    public async Task Home_WithNothingCollected_ShowsFriendlyEmptyStateWithRefreshAndChannelsPointer()
    {
        var response = await browser.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        StringAssert.Contains(html, "Nothing here yet");
        StringAssert.Contains(html, "Refresh");
        StringAssert.Contains(html, "href=\"/config\">Add one on the Channels page");
    }

    [TestMethod]
    public async Task Home_WithNothingCollectedButChannels_DoesNotNagAboutChannels()
    {
        sut.Channels.Channels.Add(new Channel(1, "https://youtube.com/@one", "One"));

        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, "Nothing here yet");
        Assert.DoesNotContain("You are not following any channels", html);
    }

    [TestMethod]
    [DataRow(1, "Following 1 channel.")]
    [DataRow(2, "Following 2 channels.")]
    public async Task Home_WithNothingCollectedButChannels_LinksToChannelsAndSaysHowManyAreFollowed(int count, string expected)
    {
        for (var id = 1; id <= count; id++)
            sut.Channels.Channels.Add(new Channel(id, "https://youtube.com/@channel" + id, "Channel " + id));

        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, expected);
        StringAssert.Contains(html, "href=\"/config\">Manage channels");
    }

    [TestMethod]
    public async Task Home_WithNothingCollectedAndIdle_AsksForARefresh()
    {
        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, "Press Refresh");
        StringAssert.Contains(html, "Refresh</button>");
        Assert.DoesNotContain("collecting your first videos", html);
    }

    [TestMethod]
    public async Task Home_WithNothingCollectedWhileRefreshing_SaysSoAndShowsProgressOnce()
    {
        sut.Refresh.Current = new RefreshProgress("Looking for new videos", 0, 0);

        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, "collecting your first videos");
        Assert.DoesNotContain("Press Refresh", html, "there is no button to press during a run");
        Assert.DoesNotContain("Refresh</button>", html);
        Assert.HasCount(2, html.Split("data-refresh-progress"), "refresh.js only updates the first progress block, so a second one would freeze:\n" + html);
    }

    [TestMethod]
    public async Task Pages_ShowToolProblemsInABanner()
    {
        sut.Tools.Problems.Add("yt-dlp 2026.07.04 is older than 2026.10.26");
        sut.Entries.Entries.Add(TestApp.MakeEntry(1, new DateOnly(2026, 11, 1)));

        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, "class=\"banner\"");
        StringAssert.Contains(html, "yt-dlp 2026.07.04 is older than 2026.10.26");
    }

    [TestMethod]
    public async Task Pages_ShowLastRunAndVideosWaitingToRetry()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(1, new DateOnly(2026, 11, 1)));
        sut.Log.Latest = new RefreshRun(DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow, 3, 1, "The AI server did not answer");
        sut.Videos.Videos.Add(new TrackedVideo(1, 1, "pend1", "Needs captions", VideoState.Pending, 2, "no captions yet", DateTimeOffset.UtcNow));
        sut.Videos.Videos.Add(new TrackedVideo(2, 1, "pend2", "Never tried", VideoState.Pending, 0, null, DateTimeOffset.UtcNow));
        sut.Videos.Videos.Add(new TrackedVideo(3, 1, "gave3", "Hopeless", VideoState.GaveUp, 5, "video removed", DateTimeOffset.UtcNow));

        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, "3 completed, 1 failed.");
        StringAssert.Contains(html, "The AI server did not answer");
        StringAssert.Contains(html, "Waiting to retry (2)");
        StringAssert.Contains(html, "Needs captions");
        StringAssert.Contains(html, "no captions yet");
        StringAssert.Contains(html, "Hopeless");
        Assert.DoesNotContain("Never tried", html, "a video that was never attempted is not waiting to retry");
    }

    [TestMethod]
    public async Task Pages_WhileRefreshing_ShowProgressInsteadOfTheButton()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(1, new DateOnly(2026, 11, 1)));
        sut.Refresh.Current = new RefreshProgress("Summarizing <b>video</b>", 1, 4);

        var html = await browser.GetStringAsync("/");

        StringAssert.Contains(html, "data-refresh-progress");
        StringAssert.Contains(html, "width:25%");
        StringAssert.Contains(html, "Summarizing &lt;b&gt;video&lt;/b&gt;");
        Assert.DoesNotContain("Refresh</button>", html);
    }

    [TestMethod]
    public async Task RefreshPost_StartsOneRunAndRedirectsBack()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(1, new DateOnly(2026, 11, 1)));

        var response = await sut.SubmitAsync(browser, "/", "/refresh", ("returnUrl", "/day/2026-11-01"));

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("/day/2026-11-01", response.Headers.Location?.OriginalString);
        Assert.AreEqual(1, sut.Refresh.StartCount, "exactly one run should be requested, and none on launch");
    }

    [TestMethod]
    public async Task RefreshPost_WithForeignReturnUrl_RedirectsHome()
    {
        var response = await sut.SubmitAsync(browser, "/", "/refresh", ("returnUrl", "https://evil.example/"));

        Assert.AreEqual("/", response.Headers.Location?.OriginalString);
    }

    [TestMethod]
    public async Task RefreshPost_WithoutAntiforgeryToken_IsRejected()
    {
        var response = await browser.PostAsync("/refresh", new FormUrlEncodedContent([new KeyValuePair<string, string>("returnUrl", "/")]));

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual(0, sut.Refresh.StartCount);
    }

    [TestMethod]
    public async Task Status_WhenIdle_ReportsNotRunning()
    {
        var json = await ReadStatusAsync();

        Assert.IsFalse(json.GetProperty("running").GetBoolean());
        Assert.AreEqual(0, json.GetProperty("videosTotal").GetInt32());
    }

    [TestMethod]
    public async Task Status_WhenRunning_ReportsProgress()
    {
        sut.Refresh.Current = new RefreshProgress("Reading transcript", 2, 5);

        var json = await ReadStatusAsync();

        Assert.IsTrue(json.GetProperty("running").GetBoolean());
        Assert.AreEqual("Reading transcript", json.GetProperty("activity").GetString());
        Assert.AreEqual(2, json.GetProperty("videosDone").GetInt32());
        Assert.AreEqual(5, json.GetProperty("videosTotal").GetInt32());
    }

    [TestMethod]
    public async Task Status_WhileDiscovering_ReportsWhatDiscoveryIsDoing()
    {
        sut.Refresh.Current = new RefreshProgress("Reading transcript", 2, 5, "Checking channel 3 of 23 (Locally), 7 new so far");

        var json = await ReadStatusAsync();

        Assert.AreEqual("Checking channel 3 of 23 (Locally), 7 new so far", json.GetProperty("discovery").GetString());
    }

    [TestMethod]
    public async Task Entry_HasAnEnabledReprocessButtonPostingItsId()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(7, new DateOnly(2026, 11, 1)));

        var html = await browser.GetStringAsync("/day/2026-11-01");

        var form = ReprocessForm(html);
        StringAssert.Contains(form, "method=\"post\"");
        StringAssert.Contains(form, "action=\"/reprocess\"");
        StringAssert.Contains(form, "name=\"entryId\" value=\"7\"");
        StringAssert.Contains(form, "name=\"returnUrl\" value=\"/day/2026-11-01#entry-7\"");
        StringAssert.Contains(form, "name=\"__RequestVerificationToken\"");
        StringAssert.Matches(form, new Regex(@"<button type=""submit"" class=""btn quiet reprocess"">\s*Reprocess\s*</button>"));
    }

    [TestMethod]
    public async Task QueuedEntry_HasADisabledQueuedButton()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(7, new DateOnly(2026, 11, 1)) with { IsReprocessQueued = true });

        var html = await browser.GetStringAsync("/day/2026-11-01");

        var form = ReprocessForm(html);
        StringAssert.Matches(form, new Regex(@"<button type=""submit"" class=""btn quiet reprocess"" disabled[^>]*>\s*Queued\s*</button>"));
    }

    [TestMethod]
    public async Task FailedEntry_ShowsTheReasonBelowTheHeadingAndAllowsAnotherTry()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(7, new DateOnly(2026, 11, 1)) with { ReprocessFailure = "model <timed> out" });

        var html = await browser.GetStringAsync("/day/2026-11-01");

        var notice = Regex.Match(html, @"<p class=""error reprocess-failed""[^>]*>([^<]*)</p>");
        Assert.IsTrue(notice.Success, "the failure notice is missing from: " + html);
        Assert.AreEqual("Reprocess failed: model &lt;timed&gt; out", notice.Groups[1].Value);
        Assert.IsLessThan(notice.Index, html.IndexOf("</h2>", StringComparison.Ordinal), "the notice sits below the heading");
        Assert.DoesNotContain(" disabled", ReprocessForm(html), "the button is enabled again");
        StringAssert.Contains(ReprocessForm(html), "Reprocess");
    }

    [TestMethod]
    public async Task EntryWithoutAFailure_ShowsNoNotice()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(7, new DateOnly(2026, 11, 1)));

        var html = await browser.GetStringAsync("/day/2026-11-01");

        Assert.DoesNotContain("reprocess-failed", html);
    }

    [TestMethod]
    public async Task PostingReprocess_RequestsTheEntryAndReturnsToIt()
    {
        sut.Entries.Entries.Add(TestApp.MakeEntry(7, new DateOnly(2026, 11, 1)));

        var response = await sut.SubmitAsync(browser, "/day/2026-11-01", "/reprocess", ("entryId", "7"), ("returnUrl", "/day/2026-11-01#entry-7"));

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("/day/2026-11-01#entry-7", response.Headers.Location?.OriginalString);
        CollectionAssert.AreEqual(new long[] { 7 }, sut.Reprocess.Requested);
    }

    [TestMethod]
    public async Task PostingReprocess_WithAnExternalReturnUrl_GoesHome()
    {
        var response = await sut.SubmitAsync(browser, "/", "/reprocess", ("entryId", "7"), ("returnUrl", "https://evil.example/x"));

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("/", response.Headers.Location?.OriginalString);
        CollectionAssert.AreEqual(new long[] { 7 }, sut.Reprocess.Requested);
    }

    [TestMethod]
    public async Task PostingReprocess_WithoutAnAntiforgeryToken_IsRejected()
    {
        var form = new FormUrlEncodedContent([new KeyValuePair<string, string>("entryId", "7")]);

        var response = await browser.PostAsync("/reprocess", form);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsEmpty(sut.Reprocess.Requested);
    }

    [TestMethod]
    public async Task GettingReprocess_GoesHome()
    {
        var response = await browser.GetAsync("/reprocess");

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("/", response.Headers.Location?.OriginalString);
    }

    //

    private TestApp sut = null!;
    private HttpClient browser = null!;

    private static string ReprocessForm(string html)
    {
        var match = Regex.Match(html, @"<form[^>]*class=""reprocess-form""[^>]*>.*?</form>", RegexOptions.Singleline);
        Assert.IsTrue(match.Success, "no reprocess form found in: " + html);
        return match.Value;
    }

    private async Task<JsonElement> ReadStatusAsync()
    {
        var response = await browser.GetAsync("/status");
        Assert.AreEqual("application/json", response.Content.Headers.ContentType?.MediaType);
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
    }
}