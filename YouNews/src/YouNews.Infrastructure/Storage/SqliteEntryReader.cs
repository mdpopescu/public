using Dapper;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteEntryReader(ISqliteConnectionFactory factory) : IEntryReader
{
    public async Task<IReadOnlyList<DayCount>> ListDaysAsync(CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connection1 = connection.ConfigureAwait(false);
        var rows = await connection.QueryAsync<DayRow>(new CommandDefinition(DAYS, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return [.. rows.Select(row => new DayCount(StorageFormat.ParseDay(row.Day), row.EntryCount))];
    }

    public async Task<IReadOnlyList<Entry>> ListForDayAsync(DateOnly day, CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connection1 = connection.ConfigureAwait(false);
        var query = new CommandDefinition(FOR_DAY, new { Day = StorageFormat.ToText(day) }, cancellationToken: cancellationToken);
        var rows = (await connection.QueryAsync<EntryRow>(query).ConfigureAwait(false)).ToList();
        var ids = rows.Select(row => row.Id).ToList();
        var tags = await parts.LoadTagsAsync(connection, ids, cancellationToken).ConfigureAwait(false);
        var sources = await parts.LoadSourcesAsync(connection, ids, cancellationToken).ConfigureAwait(false);
        var images = await parts.LoadImagesAsync(connection, ids, cancellationToken).ConfigureAwait(false);
        var related = await parts.LoadRelatedAsync(connection, ids, cancellationToken).ConfigureAwait(false);
        return [.. rows.Select(row => ToEntry(row, tags[row.Id], sources[row.Id], images[row.Id], related[row.Id]))];
    }

    //

    private const string DAYS = "SELECT day AS Day, COUNT(*) AS EntryCount FROM entries GROUP BY day ORDER BY day DESC";

    private const string FOR_DAY = """
                                   SELECT e.id AS Id, e.day AS Day, c.name AS ChannelName, v.youtube_id AS VideoYouTubeId,
                                          e.headline AS Headline, e.summary AS Summary, e.opinion AS Opinion,
                                          r.id IS NOT NULL AND r.failure IS NULL AS IsReprocessQueued, r.failure AS ReprocessFailure
                                   FROM entries e JOIN videos v ON v.id = e.video_id JOIN channels c ON c.id = v.channel_id
                                   LEFT JOIN reprocess_requests r ON r.entry_id = e.id
                                   WHERE e.day = @Day ORDER BY v.published_on DESC, e.id DESC
                                   """;

    private readonly EntryParts parts = new();

    private static Entry ToEntry(EntryRow row, IEnumerable<string> tags, IEnumerable<WebSource> sources, IEnumerable<EntryImage> images, IEnumerable<RelatedLink> related) => new(
        row.Id,
        StorageFormat.ParseDay(row.Day),
        row.ChannelName,
        row.VideoYouTubeId,
        row.Headline,
        [.. tags],
        row.Summary,
        row.Opinion,
        [.. sources],
        [.. images],
        [.. related],
        row.IsReprocessQueued,
        row.ReprocessFailure);

    private sealed class DayRow
    {
        public string Day { get; set; } = "";

        public int EntryCount { get; set; }
    }

    private sealed class EntryRow
    {
        public long Id { get; set; }

        public string Day { get; set; } = "";

        public string ChannelName { get; set; } = "";

        public string VideoYouTubeId { get; set; } = "";

        public string Headline { get; set; } = "";

        public string Summary { get; set; } = "";

        public string Opinion { get; set; } = "";

        public bool IsReprocessQueued { get; set; }

        public string? ReprocessFailure { get; set; }
    }
}