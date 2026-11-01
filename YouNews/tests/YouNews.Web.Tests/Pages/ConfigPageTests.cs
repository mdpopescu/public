using System.Net;
using YouNews.Core.Model;

namespace YouNews.Web.Tests.Pages;

[TestClass]
public sealed class ConfigPageTests
{
    [TestInitialize]
    public void Initialize()
    {
        sut = new TestApp();
        browser = sut.CreateBrowser();
        sut.Catalog.Known["@matthew_berman"] = new ChannelDetails("UC123", "https://www.youtube.com/@matthew_berman", "Matthew Berman");
    }

    [TestCleanup]
    public void Cleanup()
    {
        browser.Dispose();
        sut.Dispose();
    }

    [TestMethod]
    public async Task Config_ListsChannelsInterestsAndSetup()
    {
        sut.Channels.Channels.Add(new Channel(7, "https://www.youtube.com/@nine", "Channel Nine"));
        sut.Interests.Interests.AddRange(["New local models", "RAM and GPU market"]);
        sut.Setup.Snapshot = new SetupSnapshot(["gemma4-26b", "qwen3-30b"], "RTX 5060 Ti, 62 GB RAM", DateTimeOffset.UtcNow, "Last check failed");

        var html = await browser.GetStringAsync("/config");

        StringAssert.Contains(html, "Channel Nine");
        StringAssert.Contains(html, "New local models&#xA;RAM and GPU market");
        StringAssert.Contains(html, "RTX 5060 Ti, 62 GB RAM");
        StringAssert.Contains(html, "qwen3-30b");
        StringAssert.Contains(html, "Last check failed");
    }

    [TestMethod]
    public async Task Config_NeverCheckedSetup_ExplainsInsteadOfShowingBlanks()
    {
        var html = await browser.GetStringAsync("/config");

        StringAssert.Contains(html, "has not been checked yet");
        StringAssert.Contains(html, "not following any channels yet");
    }

    [TestMethod]
    public async Task Add_KnownHandle_FollowsItAndRedirects()
    {
        var response = await sut.SubmitAsync(browser, "/config", "/config?handler=Add", ("address", "@matthew_berman"));

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("Matthew Berman", sut.Channels.Channels.Single().Name);
        var html = await browser.GetStringAsync("/config");
        StringAssert.Contains(html, "Now following Matthew Berman.");
    }

    [TestMethod]
    public async Task Add_UnknownHandle_ShowsInlineError()
    {
        var response = await sut.SubmitAsync(browser, "/config", "/config?handler=Add", ("address", "@nobody_here"));
        var html = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, html);
        StringAssert.Contains(html, "No YouTube channel was found for &quot;@nobody_here&quot;");
        StringAssert.Contains(html, "value=\"@nobody_here\"", "the typed text stays in the box");
        Assert.IsEmpty(sut.Channels.Channels);
    }

    [TestMethod]
    public async Task Add_BlankAddress_AsksForOne()
    {
        var response = await sut.SubmitAsync(browser, "/config", "/config?handler=Add", ("address", "   "));

        StringAssert.Contains(await response.Content.ReadAsStringAsync(), "Enter a channel URL");
    }

    [TestMethod]
    public async Task Add_ChannelAlreadyFollowed_SaysSo()
    {
        sut.Channels.Channels.Add(new Channel(1, "https://www.youtube.com/@matthew_berman", "Matthew Berman"));

        var response = await sut.SubmitAsync(browser, "/config", "/config?handler=Add", ("address", "@matthew_berman"));
        var html = await response.Content.ReadAsStringAsync();

        StringAssert.Contains(html, "You already follow Matthew Berman.");
        Assert.HasCount(1, sut.Channels.Channels);
    }

    [TestMethod]
    public async Task Add_WhenToolsFail_ShowsErrorInsteadOfCrashing()
    {
        sut.Catalog.Failure = new InvalidOperationException("yt-dlp could not be started");

        var response = await sut.SubmitAsync(browser, "/config", "/config?handler=Add", ("address", "@matthew_berman"));
        var html = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, html);
        StringAssert.Contains(html, "yt-dlp could not be started");
    }

    [TestMethod]
    public async Task Remove_StopsFollowingTheChannel()
    {
        sut.Channels.Channels.Add(new Channel(7, "https://www.youtube.com/@nine", "Channel Nine"));
        sut.Channels.Channels.Add(new Channel(8, "https://www.youtube.com/@ten", "Channel Ten"));

        var response = await sut.SubmitAsync(browser, "/config", "/config?handler=Remove&id=7");

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("Channel Ten", sut.Channels.Channels.Single().Name);
    }

    [TestMethod]
    public async Task Interests_AreSavedOnePerLineWithBlankLinesDropped()
    {
        var response = await sut.SubmitAsync(browser, "/config", "/config?handler=Interests", ("interestsText", "New local models\r\n\r\n  AI legislation  \r\n   \r\nRAM and GPU market\r\n"));

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        CollectionAssert.AreEqual(new[] { "New local models", "AI legislation", "RAM and GPU market" }, sut.Interests.Interests);
    }

    [TestMethod]
    public async Task Post_WithoutAntiforgeryToken_IsRejected()
    {
        var response = await browser.PostAsync("/config?handler=Interests", new FormUrlEncodedContent([new KeyValuePair<string, string>("interestsText", "x")]));

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsEmpty(sut.Interests.Interests);
    }

    //

    private TestApp sut = null!;
    private HttpClient browser = null!;
}