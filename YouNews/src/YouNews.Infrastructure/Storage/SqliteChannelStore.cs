using System.Data.Common;
using Dapper;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteChannelStore : IChannelStore
{
    public SqliteChannelStore(ISqliteConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task<IReadOnlyList<Channel>> ListAsync(CancellationToken cancellationToken)
    {
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        var rows = await connection.QueryAsync<Channel>(new CommandDefinition(LIST, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return rows.ToList();
    }

    public async Task<Channel?> AddAsync(ChannelDetails details, CancellationToken cancellationToken)
    {
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        var existing = await connection.QuerySingleOrDefaultAsync<ExistingRow>(new CommandDefinition(FIND, details, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
        if (existing is { Removed: not 0 } or null)
        {
            var id = existing is null
                ? await connection.ExecuteScalarAsync<long>(new CommandDefinition(INSERT, details, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false)
                : await ReactivateAsync(connection, transaction, existing.Id, details, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return new Channel(id, details.Url, details.Name);
        }

        return null;
    }

    public async Task RemoveAsync(long channelId, CancellationToken cancellationToken)
    {
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(REMOVE, new { ChannelId = channelId }, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    //

    private const string LIST = "SELECT id AS Id, url AS Url, name AS Name FROM channels WHERE removed = 0 ORDER BY name, id";
    private const string FIND = "SELECT id AS Id, removed AS Removed FROM channels WHERE youtube_id = @YouTubeId";
    private const string INSERT = "INSERT INTO channels (youtube_id, url, name) VALUES (@YouTubeId, @Url, @Name); SELECT last_insert_rowid()";
    private const string REACTIVATE = "UPDATE channels SET removed = 0, url = @Url, name = @Name WHERE id = @Id";
    private const string REMOVE = "UPDATE channels SET removed = 1 WHERE id = @ChannelId";

    private readonly ISqliteConnectionFactory factory;

    private static async Task<long> ReactivateAsync(DbConnection connection, DbTransaction transaction, long channelId, ChannelDetails details, CancellationToken cancellationToken)
    {
        var parameters = new { Id = channelId, details.Url, details.Name };
        await connection.ExecuteAsync(new CommandDefinition(REACTIVATE, parameters, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return channelId;
    }

    private sealed class ExistingRow
    {
        public long Id { get; set; }

        public long Removed { get; set; }
    }
}