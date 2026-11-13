using Dapper;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteRunAlertStore : IRunAlertStore
{
    public SqliteRunAlertStore(ISqliteConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task RaiseAsync(string message, DateTimeOffset raisedAt, CancellationToken cancellationToken)
    {
        var parameters = new { Message = message, RaisedAt = StorageFormat.ToText(raisedAt) };
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(UPSERT, parameters, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    public async Task<RunAlert?> GetAsync(CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        var row = await connection.QuerySingleOrDefaultAsync<AlertRow>(new CommandDefinition(SELECT, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return row is null ? null : new RunAlert(row.Message, StorageFormat.ParseMoment(row.RaisedAt));
    }

    public async Task DismissAsync(CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(DELETE, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    //

    private const string SELECT = "SELECT message AS Message, raised_at AS RaisedAt FROM run_alert WHERE id = 1";

    private const string DELETE = "DELETE FROM run_alert";

    private const string UPSERT = """
        INSERT INTO run_alert (id, message, raised_at) VALUES (1, @Message, @RaisedAt)
        ON CONFLICT(id) DO UPDATE SET message = excluded.message, raised_at = excluded.raised_at
        """;

    private readonly ISqliteConnectionFactory factory;

    private sealed class AlertRow
    {
        public string Message { get; set; } = "";

        public string RaisedAt { get; set; } = "";
    }
}