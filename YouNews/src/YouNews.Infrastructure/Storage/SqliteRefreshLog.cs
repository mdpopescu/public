using Dapper;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteRefreshLog : IRefreshLog
{
    public SqliteRefreshLog(ISqliteConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task RecordAsync(RefreshRun run, CancellationToken cancellationToken)
    {
        var parameters = new
        {
            StartedAt = StorageFormat.ToText(run.StartedAt),
            FinishedAt = StorageFormat.ToText(run.FinishedAt),
            run.Completed,
            run.Failed,
            run.Problem,
        };
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(INSERT, parameters, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    public async Task<RefreshRun?> GetLatestAsync(CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        var row = await connection.QuerySingleOrDefaultAsync<RunRow>(new CommandDefinition(LATEST, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return row is null
            ? null
            : new RefreshRun(StorageFormat.ParseMoment(row.StartedAt), StorageFormat.ParseMoment(row.FinishedAt), row.Completed, row.Failed, row.Problem);
    }

    //

    private const string INSERT = """
        INSERT INTO refresh_runs (started_at, finished_at, completed, failed, problem)
        VALUES (@StartedAt, @FinishedAt, @Completed, @Failed, @Problem)
        """;

    private const string LATEST = """
        SELECT started_at AS StartedAt, finished_at AS FinishedAt, completed AS Completed, failed AS Failed, problem AS Problem
        FROM refresh_runs ORDER BY id DESC LIMIT 1
        """;

    private readonly ISqliteConnectionFactory factory;

    private sealed class RunRow
    {
        public string StartedAt { get; set; } = "";

        public string FinishedAt { get; set; } = "";

        public int Completed { get; set; }

        public int Failed { get; set; }

        public string? Problem { get; set; }
    }
}