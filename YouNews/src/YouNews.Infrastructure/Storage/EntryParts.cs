using System.Data.Common;
using Dapper;
using YouNews.Core.Model;

namespace YouNews.Infrastructure.Storage;

/// <summary>Loads the tags, sources, images and related links of many entries with one query each.</summary>
internal sealed class EntryParts
{
    private const string TAGS = "SELECT entry_id AS EntryId, tag AS Tag FROM entry_tags WHERE entry_id IN @Ids ORDER BY entry_id, position";
    private const string SOURCES = "SELECT entry_id AS EntryId, title AS Title, url AS Url FROM entry_web_sources WHERE entry_id IN @Ids ORDER BY entry_id, position";
    private const string IMAGES = "SELECT entry_id AS EntryId, relative_path AS RelativePath, kind AS Kind FROM entry_images WHERE entry_id IN @Ids ORDER BY entry_id, position";

    private const string RELATED = """
        SELECT r.entry_id AS EntryId, e.id AS RelatedId, e.day AS Day, e.headline AS Headline
        FROM entry_related r JOIN entries e ON e.id = r.related_entry_id
        WHERE r.entry_id IN @Ids ORDER BY r.entry_id, r.position
        """;

    public async Task<ILookup<long, string>> LoadTagsAsync(DbConnection connection, IReadOnlyList<long> ids, CancellationToken cancellationToken)
    {
        var rows = await QueryAsync<TagRow>(connection, TAGS, ids, cancellationToken).ConfigureAwait(false);
        return rows.ToLookup(row => row.EntryId, row => row.Tag);
    }

    public async Task<ILookup<long, WebSource>> LoadSourcesAsync(DbConnection connection, IReadOnlyList<long> ids, CancellationToken cancellationToken)
    {
        var rows = await QueryAsync<SourceRow>(connection, SOURCES, ids, cancellationToken).ConfigureAwait(false);
        return rows.ToLookup(row => row.EntryId, row => new WebSource(row.Title, row.Url));
    }

    public async Task<ILookup<long, EntryImage>> LoadImagesAsync(DbConnection connection, IReadOnlyList<long> ids, CancellationToken cancellationToken)
    {
        var rows = await QueryAsync<ImageRow>(connection, IMAGES, ids, cancellationToken).ConfigureAwait(false);
        return rows.ToLookup(row => row.EntryId, row => new EntryImage(row.RelativePath, Enum.Parse<ImageKind>(row.Kind)));
    }

    public async Task<ILookup<long, RelatedLink>> LoadRelatedAsync(DbConnection connection, IReadOnlyList<long> ids, CancellationToken cancellationToken)
    {
        var rows = await QueryAsync<RelatedRow>(connection, RELATED, ids, cancellationToken).ConfigureAwait(false);
        return rows.ToLookup(row => row.EntryId, row => new RelatedLink(row.RelatedId, StorageFormat.ParseDay(row.Day), row.Headline));
    }

    //

    private static async Task<List<T>> QueryAsync<T>(DbConnection connection, string sql, IReadOnlyList<long> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
            return [];

        var rows = await connection.QueryAsync<T>(new CommandDefinition(sql, new { Ids = ids }, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return rows.ToList();
    }

    private sealed class TagRow
    {
        public long EntryId { get; set; }

        public string Tag { get; set; } = "";
    }

    private sealed class SourceRow
    {
        public long EntryId { get; set; }

        public string Title { get; set; } = "";

        public string Url { get; set; } = "";
    }

    private sealed class ImageRow
    {
        public long EntryId { get; set; }

        public string RelativePath { get; set; } = "";

        public string Kind { get; set; } = "";
    }

    private sealed class RelatedRow
    {
        public long EntryId { get; set; }

        public long RelatedId { get; set; }

        public string Day { get; set; } = "";

        public string Headline { get; set; } = "";
    }
}