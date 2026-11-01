using System.Data.Common;
using Dapper;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteEntryStore : IEntryStore
{
    public SqliteEntryStore(ISqliteConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task<long> AddAsync(NewEntry entry, CancellationToken cancellationToken)
    {
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        var entryParameters = new { entry.VideoId, Day = StorageFormat.ToText(entry.Day), entry.Headline, entry.Summary, entry.Opinion };
        var id = await connection.ExecuteScalarAsync<long>(new CommandDefinition(INSERT_ENTRY, entryParameters, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
        await InsertPartsAsync(connection, transaction, id, entry, cancellationToken).ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(MARK_DONE, new { entry.VideoId }, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        return id;
    }

    public async Task<IReadOnlyList<EntryDigest>> ListRecentDigestsAsync(int limit, CancellationToken cancellationToken)
    {
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        var rows = (await connection.QueryAsync<DigestRow>(new CommandDefinition(RECENT, new { Limit = limit }, cancellationToken: cancellationToken)).ConfigureAwait(false)).ToList();
        var tags = await parts.LoadTagsAsync(connection, rows.Select(row => row.Id).ToList(), cancellationToken).ConfigureAwait(false);
        return rows.Select(row => new EntryDigest(row.Id, StorageFormat.ParseDay(row.Day), row.Headline, tags[row.Id].ToList(), row.Summary)).ToList();
    }

    //

    private const string INSERT_ENTRY = """
        INSERT INTO entries (video_id, day, headline, summary, opinion) VALUES (@VideoId, @Day, @Headline, @Summary, @Opinion);
        SELECT last_insert_rowid()
        """;

    private const string INSERT_TAG = "INSERT INTO entry_tags (entry_id, position, tag) VALUES (@EntryId, @Position, @Tag)";
    private const string INSERT_SOURCE = "INSERT INTO entry_web_sources (entry_id, position, title, url) VALUES (@EntryId, @Position, @Title, @Url)";
    private const string INSERT_IMAGE = "INSERT INTO entry_images (entry_id, position, relative_path, kind) VALUES (@EntryId, @Position, @RelativePath, @Kind)";
    private const string INSERT_RELATED = "INSERT INTO entry_related (entry_id, related_entry_id, position) VALUES (@EntryId, @RelatedId, @Position)";
    private const string MARK_DONE = "UPDATE videos SET state = 'Done', last_reason = NULL WHERE id = @VideoId";

    private const string RECENT = """
        SELECT id AS Id, day AS Day, headline AS Headline, summary AS Summary
        FROM entries ORDER BY day DESC, id DESC LIMIT @Limit
        """;

    private readonly ISqliteConnectionFactory factory;
    private readonly EntryParts parts = new();

    private static Task InsertAllAsync(DbConnection connection, DbTransaction transaction, string sql, IEnumerable<object> rows, CancellationToken cancellationToken) =>
        connection.ExecuteAsync(new CommandDefinition(sql, rows.ToList(), transaction, cancellationToken: cancellationToken));

    private static async Task InsertPartsAsync(DbConnection connection, DbTransaction transaction, long id, NewEntry entry, CancellationToken cancellationToken)
    {
        var tags = entry.Tags.Select((tag, position) => new { EntryId = id, Position = position, Tag = tag });
        var sources = entry.WebSources.Select((source, position) => new { EntryId = id, Position = position, source.Title, source.Url });
        var images = entry.Images.Select((image, position) => new { EntryId = id, Position = position, image.RelativePath, Kind = image.Kind.ToString() });
        var related = entry.RelatedEntryIds.Select((relatedId, position) => new { EntryId = id, RelatedId = relatedId, Position = position });
        await InsertAllAsync(connection, transaction, INSERT_TAG, tags, cancellationToken).ConfigureAwait(false);
        await InsertAllAsync(connection, transaction, INSERT_SOURCE, sources, cancellationToken).ConfigureAwait(false);
        await InsertAllAsync(connection, transaction, INSERT_IMAGE, images, cancellationToken).ConfigureAwait(false);
        await InsertAllAsync(connection, transaction, INSERT_RELATED, related, cancellationToken).ConfigureAwait(false);
    }

    private sealed class DigestRow
    {
        public long Id { get; set; }

        public string Day { get; set; } = "";

        public string Headline { get; set; } = "";

        public string Summary { get; set; } = "";
    }
}