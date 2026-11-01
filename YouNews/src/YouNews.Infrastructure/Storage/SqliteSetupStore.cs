using System.Text.Json;
using Dapper;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteSetupStore : ISetupStore
{
    public SqliteSetupStore(ISqliteConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task<SetupSnapshot?> GetLatestAsync(CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        var row = await connection.QuerySingleOrDefaultAsync<SnapshotRow>(new CommandDefinition(SELECT, cancellationToken: cancellationToken)).ConfigureAwait(false);
        if (row is null)
            return null;

        var models = JsonSerializer.Deserialize<List<string>>(row.Models) ?? [];
        return new SetupSnapshot(models, row.Hardware, StorageFormat.ParseMoment(row.CheckedAt), row.Warning);
    }

    public async Task SaveAsync(SetupSnapshot snapshot, CancellationToken cancellationToken)
    {
        var parameters = new
        {
            Models = JsonSerializer.Serialize(snapshot.Models),
            snapshot.Hardware,
            CheckedAt = StorageFormat.ToText(snapshot.CheckedAt),
            snapshot.Warning,
        };
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(UPSERT, parameters, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    //

    private const string SELECT = "SELECT models AS Models, hardware AS Hardware, checked_at AS CheckedAt, warning AS Warning FROM setup_snapshot WHERE id = 1";

    private const string UPSERT = """
        INSERT INTO setup_snapshot (id, models, hardware, checked_at, warning) VALUES (1, @Models, @Hardware, @CheckedAt, @Warning)
        ON CONFLICT(id) DO UPDATE SET models = excluded.models, hardware = excluded.hardware,
                                      checked_at = excluded.checked_at, warning = excluded.warning
        """;

    private readonly ISqliteConnectionFactory factory;

    private sealed class SnapshotRow
    {
        public string Models { get; set; } = "";

        public string Hardware { get; set; } = "";

        public string CheckedAt { get; set; } = "";

        public string? Warning { get; set; }
    }
}