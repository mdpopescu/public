using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Core.Refresh;

namespace YouNews.Web.Tests.Pages;

/// <summary>The real web application, with every Core port replaced by an in-memory fake.</summary>
public sealed partial class TestApp : WebApplicationFactory<Program>
{
    public FakeEntryReader Entries { get; } = new();

    public FakeChannelStore Channels { get; } = new();

    public FakeInterestStore Interests { get; } = new();

    public FakeRefreshLog Log { get; } = new();

    public FakeRunAlertStore Alerts { get; } = new();

    public FakeSetupStore Setup { get; } = new();

    public FakeModelSelection Model { get; } = new();

    public FakeToolHealth Tools { get; } = new();

    public FakeVideoCatalog Catalog { get; } = new();

    public FakeVideoStore Videos { get; } = new();

    public FakeRefresh Refresh { get; } = new();

    public FakeReprocessTrigger Reprocess { get; } = new();

    public HttpClient CreateBrowser() =>
        CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });

    /// <summary>Loads a page as a browser would, takes the antiforgery token from its form, and posts the fields to the given address.</summary>
    public async Task<HttpResponseMessage> SubmitAsync(HttpClient browser, string formPage, string postUrl, params (string Name, string Value)[] fields)
    {
        var html = await browser.GetStringAsync(formPage);
        var token = WebUtility.HtmlDecode(TokenPattern().Match(html).Groups[1].Value);
        var form = fields.Select(field => new KeyValuePair<string, string>(field.Name, field.Value)).Append(new("__RequestVerificationToken", token));
        return await browser.PostAsync(postUrl, new FormUrlEncodedContent(form));
    }

    public static Entry MakeEntry(long id, DateOnly day, string headline = "A new model", string summary = "A summary.", string opinion = "An opinion.", string[]? tags = null) =>
        new(id, day, "Sample Channel One", "vid" + id, headline, tags ?? [EntryTags.LOCAL_MODELS], summary, opinion, [], [], []);

    //

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("YouNews:DataDirectory", dataDirectory);
        builder.UseSetting("YouNews:Refresh:OnLaunch", "false");
        builder.UseDefaultServiceProvider(options => options.ValidateOnBuild = false);
        builder.ConfigureServices(services =>
        {
            Replace<IEntryReader>(services, Entries);
            Replace<IChannelStore>(services, Channels);
            Replace<IInterestStore>(services, Interests);
            Replace<IRefreshLog>(services, Log);
            Replace<IRunAlertStore>(services, Alerts);
            Replace<ISetupStore>(services, Setup);
            Replace<ICurrentModel>(services, Model);
            Replace<IModelSelector>(services, Model);
            Replace<IToolHealth>(services, Tools);
            Replace<IVideoCatalog>(services, Catalog);
            Replace<IVideoStore>(services, Videos);
            Replace<IRefreshTrigger>(services, Refresh);
            Replace<IRefreshStatus>(services, Refresh);
            Replace<IReprocessTrigger>(services, Reprocess);
            Replace<IReprocessWorker>(services, new IdleReprocessWorker());
        });
    }

    //

    private readonly string dataDirectory = Path.Combine(AppContext.BaseDirectory, "web-test-data", Guid.NewGuid().ToString("N"));

    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex TokenPattern();

    private static void Replace<T>(IServiceCollection services, T instance) where T : class =>
        services.Replace(ServiceDescriptor.Singleton(instance));
}