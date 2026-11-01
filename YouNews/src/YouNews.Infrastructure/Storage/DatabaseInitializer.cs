using System.Data.Common;
using Dapper;
using Microsoft.Extensions.Options;

namespace YouNews.Infrastructure.Storage;

public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private const string CREATE_VERSION_TABLE = "CREATE TABLE IF NOT EXISTS schema_version (version INTEGER PRIMARY KEY, applied_at TEXT NOT NULL)";
    private const string SELECT_VERSION = "SELECT COALESCE(MAX(version), 0) FROM schema_version";
    private const string INSERT_VERSION = "INSERT INTO schema_version (version, applied_at) VALUES (@Version, @AppliedAt)";
    private const string INSERT_CHANNEL = "INSERT INTO channels (youtube_id, url, name) VALUES (@YouTubeId, @Url, @Name)";
    private const string INSERT_INTEREST = "INSERT INTO interests (position, text) VALUES (@Position, @Text)";

    private static readonly string[] SEED_INTERESTS =
    [
        "New local models, and whether they beat what is on my server",
        "AI legislation, including rumors of it",
        "The RAM and video card market",
        "New AI research",
    ];

    private readonly ISqliteConnectionFactory factory;
    private readonly IDatabaseMigrations migrations;
    private readonly TimeProvider timeProvider;
    private readonly string dataDirectory;

    public DatabaseInitializer(ISqliteConnectionFactory factory, IDatabaseMigrations migrations, TimeProvider timeProvider, IOptions<YouNewsOptions> options)
    {
        this.factory = factory;
        this.migrations = migrations;
        this.timeProvider = timeProvider;
        dataDirectory = options.Value.DataDirectory;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(dataDirectory);
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition("PRAGMA journal_mode=WAL", cancellationToken: cancellationToken)).ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(CREATE_VERSION_TABLE, cancellationToken: cancellationToken)).ConfigureAwait(false);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        var applied = await connection.ExecuteScalarAsync<int>(new CommandDefinition(SELECT_VERSION, transaction: transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
        await ApplyMigrationsAsync(connection, transaction, applied, cancellationToken).ConfigureAwait(false);
        if (applied == 0)
            await SeedAsync(connection, transaction, cancellationToken).ConfigureAwait(false);
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    //

    private static async Task SeedAsync(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        var channel = new { YouTubeId = "UCawZsQWqfGSbCI5yjkdVkTA", Url = "https://www.youtube.com/@matthew_berman/", Name = "Matthew Berman" };
        await connection.ExecuteAsync(new CommandDefinition(INSERT_CHANNEL, channel, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
        var interests = SEED_INTERESTS.Select((text, position) => new { Position = position, Text = text }).ToList();
        await connection.ExecuteAsync(new CommandDefinition(INSERT_INTEREST, interests, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    private async Task ApplyMigrationsAsync(DbConnection connection, DbTransaction transaction, int applied, CancellationToken cancellationToken)
    {
        for (var index = applied; index < migrations.Scripts.Count; index++)
        {
            await connection.ExecuteAsync(new CommandDefinition(migrations.Scripts[index], transaction: transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
            var version = new { Version = index + 1, AppliedAt = StorageFormat.ToText(timeProvider.GetUtcNow()) };
            await connection.ExecuteAsync(new CommandDefinition(INSERT_VERSION, version, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
        }
    }
}