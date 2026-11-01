using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YouNews.Core.Refresh;
using YouNews.Web.Hosting;

namespace YouNews.Web.Tests;

[TestClass]
public sealed class CompositionRootTests
{
    [TestMethod]
    public async Task RealRegistrationsFormACompleteServiceGraph()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = Environments.Production });
        builder.AddYouNews();
        builder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        await using var app = builder.Build();

        Assert.IsNotNull(app.Services.GetRequiredService<IRefreshTrigger>(), "The Refresh button depends on this.");
    }

    [TestMethod]
    public async Task RealRegistrations_RunTheReprocessWorker()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = Environments.Production });
        builder.AddYouNews();
        builder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        await using var app = builder.Build();

        Assert.IsTrue(app.Services.GetServices<IHostedService>().Any(service => service is ReprocessService), "Reprocess requests are only worked off if the worker runs.");
        Assert.IsNotNull(app.Services.GetRequiredService<IReprocessTrigger>(), "The Reprocess button depends on this.");
    }
}