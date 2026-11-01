using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YouNews.Core.Refresh;

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
}