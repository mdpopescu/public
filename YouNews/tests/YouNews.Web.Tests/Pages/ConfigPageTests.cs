using System.Net;
using System.Text.RegularExpressions;
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
    public async Task Config_ShowsTheModelInUseSelectedAndTheButtonDisabled()
    {
        sut.Setup.Snapshot = new SetupSnapshot(["gemma4-26b", "qwen3-30b"], "RTX 5060 Ti, 62 GB RAM", DateTimeOffset.UtcNow, null);
        sut.Model.Model = "qwen3-30b";

        var html = await browser.GetStringAsync("/config");

        StringAssert.Contains(html, "Model in use");
        StringAssert.Contains(html, "data-current-model=\"qwen3-30b\"");
        StringAssert.Contains(html, "<option value=\"gemma4-26b\">");
        StringAssert.Contains(html, "<option value=\"qwen3-30b\" selected=\"selected\">");
        Assert.IsTrue(Regex.IsMatch(html, "<button[^>]*disabled[^>]*>Use this model</button>"), html);
    }

    [TestMethod]
    public async Task Config_ModelInUseIsNotReported_ItIsStillListedAndSelected()
    {
        sut.Setup.Snapshot = new SetupSnapshot(["qwen3-30b"], "RTX 5060 Ti, 62 GB RAM", DateTimeOffset.UtcNow, null);

        var html = await browser.GetStringAsync("/config");

        StringAssert.Contains(html, "<option value=\"gemma4-26b\" selected=\"selected\">");
    }

    [TestMethod]
    public async Task Model_ListedModel_IsSavedAndAnnounced()
    {
        sut.Setup.Snapshot = new SetupSnapshot(["gemma4-26b", "qwen3-30b"], "RTX 5060 Ti, 62 GB RAM", DateTimeOffset.UtcNow, null);

        var response = await sut.SubmitAsync(browser, "/config", "/config?handler=Model", ("model", " qwen3-30b "));

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("qwen3-30b", sut.Model.Model);
        StringAssert.Contains(await browser.GetStringAsync("/config"), "Now using qwen3-30b.");
    }

    [TestMethod]
    public async Task Model_UnknownModel_SavesNothingAndSaysSo()
    {
        sut.Setup.Snapshot = new SetupSnapshot(["gemma4-26b", "qwen3-30b"], "RTX 5060 Ti, 62 GB RAM", DateTimeOffset.UtcNow, null);

        var response = await sut.SubmitAsync(browser, "/config", "/config?handler=Model", ("model", "not-a-model"));

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("gemma4-26b", sut.Model.Model);
        StringAssert.Contains(await browser.GetStringAsync("/config"), "not-a-model");
    }

    [TestMethod]
    public async Task Model_BlankModel_SavesNothing()
    {
        sut.Setup.Snapshot = new SetupSnapshot(["gemma4-26b", "qwen3-30b"], "RTX 5060 Ti, 62 GB RAM", DateTimeOffset.UtcNow, null);

        var response = await sut.SubmitAsync(browser, "/config", "/config?handler=Model", ("model", "   "));

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("gemma4-26b", sut.Model.Model);
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