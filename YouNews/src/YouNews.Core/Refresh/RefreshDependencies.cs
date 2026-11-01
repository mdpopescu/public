using YouNews.Core.Ports;
using YouNews.Core.Setup;

namespace YouNews.Core.Refresh;

/// <summary>The stores and services a refresh run reads and reports through.</summary>
public sealed record RefreshDependencies(
    IChannelStore Channels,
    IVideoStore Videos,
    IInterestStore Interests,
    IRefreshLog Log,
    ISetupMonitor SetupMonitor,
    TimeProvider Time);