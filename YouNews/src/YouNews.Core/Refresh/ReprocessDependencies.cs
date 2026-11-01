using YouNews.Core.Ports;
using YouNews.Core.Setup;

namespace YouNews.Core.Refresh;

/// <summary>The stores and services the reprocess worker works through.</summary>
public sealed record ReprocessDependencies(
    IReprocessQueue Queue,
    IEntryReprocessor Reprocessor,
    ISetupMonitor SetupMonitor,
    IInterestStore Interests,
    TimeProvider Time);