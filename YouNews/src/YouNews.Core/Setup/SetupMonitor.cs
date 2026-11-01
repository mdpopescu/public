using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Setup;

public sealed class SetupMonitor : ISetupMonitor
{
    public SetupMonitor(IServerInspector inspector, ISetupStore store, TimeProvider timeProvider)
    {
        this.inspector = inspector;
        this.store = store;
        this.timeProvider = timeProvider;
    }

    public async Task<SetupSnapshot?> GetCurrentAsync(CancellationToken cancellationToken)
    {
        var stored = await store.GetLatestAsync(cancellationToken).ConfigureAwait(false);
        if (stored is { Warning: null } && WasCheckedToday(stored))
            return stored;

        var result = await InspectAsync(stored, cancellationToken).ConfigureAwait(false);
        if (result != null)
            await store.SaveAsync(result, cancellationToken).ConfigureAwait(false);

        return result;
    }

    //

    private readonly IServerInspector inspector;
    private readonly ISetupStore store;
    private readonly TimeProvider timeProvider;

    private bool WasCheckedToday(SetupSnapshot snapshot)
    {
        var checkedOn = TimeZoneInfo.ConvertTime(snapshot.CheckedAt, timeProvider.LocalTimeZone);
        return DateOnly.FromDateTime(checkedOn.DateTime) == DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    }

    private async Task<SetupSnapshot?> InspectAsync(SetupSnapshot? previous, CancellationToken cancellationToken)
    {
        try
        {
            var models = await inspector.ListModelsAsync(cancellationToken).ConfigureAwait(false);
            var hardware = await inspector.DescribeHardwareAsync(cancellationToken).ConfigureAwait(false);
            return new SetupSnapshot(models, hardware, timeProvider.GetUtcNow(), null);
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return previous == null ? null : previous with { Warning = ex.Message };
        }
    }
}