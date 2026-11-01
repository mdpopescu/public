using Dapper;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteReprocessQueue : IReprocessQueue
{
    public SqliteReprocessQueue(ISqliteConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task RequestAsync(long entryId, CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(REQUEST, new { EntryId = entryId }, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    public async Task<ReprocessJob?> GetNextAsync(CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        var row = await connection.QueryFirstOrDefaultAsync<JobRow>(new CommandDefinition(NEXT, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return row is null ? null : new ReprocessJob(row.EntryId, row.YouTubeId, row.Title, row.ChannelName);
    }

    public async Task RecordFailureAsync(long entryId, string reason, CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(FAIL, new { EntryId = entryId, Reason = reason }, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    //

    private const string REQUEST = """
        INSERT INTO reprocess_requests (entry_id) SELECT id FROM entries WHERE id = @EntryId
        ON CONFLICT(entry_id) DO UPDATE SET failure = NULL
        """;

    private const string NEXT = """
        SELECT r.entry_id AS EntryId, v.youtube_id AS YouTubeId, v.title AS Title, c.name AS ChannelName
        FROM reprocess_requests r
        JOIN entries e ON e.id = r.entry_id
        JOIN videos v ON v.id = e.video_id
        JOIN channels c ON c.id = v.channel_id
        WHERE r.failure IS NULL ORDER BY r.id LIMIT 1
        """;

    private const string FAIL = "UPDATE reprocess_requests SET failure = @Reason WHERE entry_id = @EntryId";

    private readonly ISqliteConnectionFactory factory;

    private sealed class JobRow
    {
        public long EntryId { get; set; }

        public string YouTubeId { get; set; } = "";

        public string Title { get; set; } = "";

        public string ChannelName { get; set; } = "";
    }
}