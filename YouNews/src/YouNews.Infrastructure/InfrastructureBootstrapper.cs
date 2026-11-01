using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YouNews.Core.Ports;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.Search;
using YouNews.Infrastructure.Server;
using YouNews.Infrastructure.Storage;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure;

public static class InfrastructureBootstrapper
{
    public static IServiceCollection AddYouNewsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<YouNewsOptions>(configuration.GetSection(YouNewsOptions.SECTION_NAME));
        services.AddSingleton<IProcessRunner, ProcessRunner>();
        AddStorage(services);
        AddYouTube(services);
        AddServerInspection(services);
        AddSearch(services);
        return services;
    }

    //

    private static void AddStorage(IServiceCollection services)
    {
        services.AddSingleton<ISqliteConnectionFactory, SqliteConnectionFactory>();
        services.AddSingleton<IDatabaseMigrations, DatabaseMigrations>();
        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
        services.AddSingleton<IChannelStore, SqliteChannelStore>();
        services.AddSingleton<IVideoStore, SqliteVideoStore>();
        services.AddSingleton<IEntryStore, SqliteEntryStore>();
        services.AddSingleton<IEntryReader, SqliteEntryReader>();
        services.AddSingleton<IInterestStore, SqliteInterestStore>();
        services.AddSingleton<ISetupStore, SqliteSetupStore>();
        services.AddSingleton<IRefreshLog, SqliteRefreshLog>();
    }

    private static void AddYouTube(IServiceCollection services)
    {
        services.AddHttpClient("YouTubeImages");
        services.AddSingleton<YtDlpRunner>();
        services.AddSingleton<VideoErrorClassifier>();
        services.AddSingleton<ChannelAddressParser>();
        services.AddSingleton<Json3Parser>();
        services.AddSingleton<ThumbnailDownloader>();
        services.AddSingleton<FrameExtractor>();
        services.AddSingleton<IVideoCatalog, YtDlpVideoCatalog>();
        services.AddSingleton<ICaptionSource, YtDlpCaptionSource>();
        services.AddSingleton<IImageSource, YouTubeImageSource>();
        services.AddSingleton<IToolHealth, ToolHealth>();
        services.AddSingleton<IAudioDownloader, YouTubeAudioDownloader>();
    }

    private static void AddServerInspection(IServiceCollection services)
    {
        services.AddSingleton<IRemoteCommandRunner, SshRemoteCommandRunner>();
        services.AddHttpClient<IServerInspector, ServerInspector>();
    }

    private static void AddSearch(IServiceCollection services)
    {
        services.AddHttpClient<SearxngWebSearch>();
        services.AddSingleton<CamoufoxWebSearch>();
        services.AddTransient<IWebSearch>(provider => new TieredWebSearch(
            provider.GetRequiredService<SearxngWebSearch>(),
            provider.GetRequiredService<CamoufoxWebSearch>(),
            provider.GetRequiredService<IOptions<YouNewsOptions>>()));
    }
}