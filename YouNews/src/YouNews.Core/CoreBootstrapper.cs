using Microsoft.Extensions.DependencyInjection;
using YouNews.Core.Briefing;
using YouNews.Core.Ports;
using YouNews.Core.Refresh;
using YouNews.Core.Setup;

namespace YouNews.Core;

public static class CoreBootstrapper
{
    public static IServiceCollection AddYouNewsCore(this IServiceCollection services, RefreshLimits limits)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton(limits);
        services.AddSingleton<ISetupMonitor, SetupMonitor>();
        services.AddSingleton<IBriefingWriter, BriefingWriter>();
        services.AddSingleton<IVideoDiscovery, VideoDiscovery>();
        services.AddSingleton<VideoProcessorDependencies>();
        services.AddSingleton<VideoProcessor>();
        services.AddSingleton<IVideoProcessor>(provider => provider.GetRequiredService<VideoProcessor>());
        services.AddSingleton<IEntryReprocessor>(provider => provider.GetRequiredService<VideoProcessor>());
        services.AddSingleton<ReprocessDependencies>();
        services.AddSingleton<ReprocessWorker>();
        services.AddSingleton<IReprocessTrigger>(provider => provider.GetRequiredService<ReprocessWorker>());
        services.AddSingleton<IReprocessWorker>(provider => provider.GetRequiredService<ReprocessWorker>());
        services.AddSingleton<RefreshDependencies>();
        services.AddSingleton<IRefreshRun, RefreshRunner>();
        services.AddSingleton<RefreshCoordinator>();
        services.AddSingleton<IRefreshTrigger>(provider => provider.GetRequiredService<RefreshCoordinator>());
        services.AddSingleton<IRefreshStatus>(provider => provider.GetRequiredService<RefreshCoordinator>());
        return services;
    }
}