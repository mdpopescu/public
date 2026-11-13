using System.Net;
using System.Text.RegularExpressions;

namespace YouNews.Web.Tests.Pages;

[TestClass]
public sealed class RunAlertPageTests
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
    public async Task Pages_ShowTheAlertInABannerWithADismissButton()
    {
        await sut.Alerts.RaiseAsync("The run was stopped because YouTube refused <5> requests.", new DateTimeOffset(2026, 11, 1, 23, 30, 0, TimeSpan.Zero), CancellationToken.None);

        var html = await browser.GetStringAsync("/");

        var banner = Regex.Match(html, @"<section class=""banner"" role=""alert"" data-run-alert>(.*?)</section>", RegexOptions.Singleline);
        Assert.IsTrue(banner.Success, "No alert banner in:\n" + html);
        StringAssert.Contains(banner.Value, "The run was stopped because YouTube refused &lt;5&gt; requests.");
        StringAssert.Contains(banner.Value, "Raised 2026-11-0");
        StringAssert.Contains(banner.Value, "method=\"post\"");
        StringAssert.Contains(banner.Value, "action=\"/dismiss-alert\"");
        StringAssert.Contains(banner.Value, "__RequestVerificationToken");
        StringAssert.Matches(banner.Value, new Regex(@"<button type=""submit"" class=""btn"">Dismiss</button>"));
    }

    [TestMethod]
    public async Task Pages_WithoutAnAlert_ShowNoAlertBanner()
    {
        var html = await browser.GetStringAsync("/");

        Assert.DoesNotContain("data-run-alert", html);
    }

    [TestMethod]
    public async Task PostingDismiss_ClearsTheAlertAndReturnsToThePage()
    {
        await sut.Alerts.RaiseAsync("stopped", DateTimeOffset.UtcNow, CancellationToken.None);

        var response = await sut.SubmitAsync(browser, "/config", "/dismiss-alert", ("returnUrl", "/config"));

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual("/config", response.Headers.Location?.OriginalString);
        Assert.IsNull(sut.Alerts.Current, "The alert should be gone.");
    }

    [TestMethod]
    public async Task PostingDismiss_WithAnExternalReturnUrl_GoesHome()
    {
        await sut.Alerts.RaiseAsync("stopped", DateTimeOffset.UtcNow, CancellationToken.None);

        var response = await sut.SubmitAsync(browser, "/", "/dismiss-alert", ("returnUrl", "https://evil.example/x"));

        Assert.AreEqual("/", response.Headers.Location?.OriginalString);
        Assert.IsNull(sut.Alerts.Current);
    }

    [TestMethod]
    public async Task PostingDismiss_WithoutAnAntiforgeryToken_IsRejectedAndKeepsTheAlert()
    {
        await sut.Alerts.RaiseAsync("stopped", DateTimeOffset.UtcNow, CancellationToken.None);

        var response = await browser.PostAsync("/dismiss-alert", new FormUrlEncodedContent([]));

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsNotNull(sut.Alerts.Current, "A forged request must not dismiss the alert.");
    }

    [TestMethod]
    public async Task GettingDismiss_GoesHomeAndKeepsTheAlert()
    {
        await sut.Alerts.RaiseAsync("stopped", DateTimeOffset.UtcNow, CancellationToken.None);

        var response = await browser.GetAsync("/dismiss-alert");

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.IsNotNull(sut.Alerts.Current);
    }

    //

    private TestApp sut = null!;
    private HttpClient browser = null!;
}