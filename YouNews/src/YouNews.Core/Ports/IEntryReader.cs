using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Reads entries for display.</summary>
public interface IEntryReader
{
    /// <summary>Lists the days that have entries, newest first, with their entry counts.</summary>
    Task<IReadOnlyList<DayCount>> ListDaysAsync(CancellationToken cancellationToken);

    /// <summary>Gets all entries of one day, most recent first.</summary>
    Task<IReadOnlyList<Entry>> ListForDayAsync(DateOnly day, CancellationToken cancellationToken);
}