namespace YouNews.Core.Refresh;

/// <summary>Finds videos that need attention.</summary>
public interface IVideoDiscovery
{
    /// <summary>
    /// Records new videos of all followed channels and settles videos waiting for a recheck.
    /// Tells the listener as it goes, so videos can be processed before the search ends.
    /// Returns a description of each channel that could not be checked; the others are not affected.
    /// </summary>
    Task<IReadOnlyList<string>> DiscoverAsync(IDiscoveryListener listener, CancellationToken cancellationToken);
}