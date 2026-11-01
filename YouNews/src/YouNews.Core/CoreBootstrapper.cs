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
        services.AddSingleton<IVideoProcessor, VideoProcessor>();
        services.AddSingleton<RefreshDependencies>();
        services.AddSingleton<IRefreshRun, RefreshRunner>();
        services.AddSingleton<RefreshCoordinator>();
        services.AddSingleton<IRefreshTrigger>(provider => provider.GetRequiredService<RefreshCoordinator>());
        services.AddSingleton<IRefreshStatus>(provider => provider.GetRequiredService<RefreshCoordinator>());
        return services;
    }
}