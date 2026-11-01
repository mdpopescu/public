using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Options;
using YouNews.Core;
using YouNews.Core.Refresh;
using YouNews.Infrastructure;
using YouNews.Infrastructure.Storage;
using YouNews.Web.Hosting;
using YouNews.Web.Presentation;

namespace YouNews.Web;

public static class WebBootstrapper
{
    public const string IMAGES_PATH = "/images";

    public static WebApplicationBuilder AddYouNews(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);
        builder.Host.UseWindowsService();

        var refreshOptions = builder.Configuration.GetSection(RefreshOptions.SECTION_NAME).Get<RefreshOptions>() ?? new RefreshOptions();
        builder.Services.AddRazorPages();
        builder.Services.AddYouNewsPresentation();
        builder.Services.AddYouNewsCore(new RefreshLimits(refreshOptions.MaximumVideosPerRun));
        builder.Services.AddYouNewsInfrastructure(builder.Configuration);
        builder.Services.AddSingleton(new RefreshSchedule(refreshOptions.DailyAt, TimeZoneInfo.Local));

        if (WindowsServiceHelpers.IsWindowsService())
        {
            builder.Services.AddHostedService<DailyRefreshService>();
        }
        else if (builder.Configuration.GetValue("YouNews:Refresh:OnLaunch", true))
        {
            builder.Services.AddHostedService<RefreshOnLaunchService>();
        }

        return builder;
    }

    public static async Task UseYouNewsAsync(this WebApplication app)
    {
        await app.Services.GetRequiredService<IDatabaseInitializer>().InitializeAsync(app.Lifetime.ApplicationStopping).ConfigureAwait(false);

        var options = app.Services.GetRequiredService<IOptions<YouNewsOptions>>().Value;
        Directory.CreateDirectory(options.ImagesDirectory);

        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(options.ImagesDirectory),
            RequestPath = IMAGES_PATH,
        });
        app.UseRouting();
        app.MapRazorPages();
    }
}