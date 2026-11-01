using Dapper;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteInterestStore : IInterestStore
{
    public SqliteInterestStore(ISqliteConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task<IReadOnlyList<string>> GetAsync(CancellationToken cancellationToken)
    {
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        var rows = await connection.QueryAsync<string>(new CommandDefinition(SELECT, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return rows.ToList();
    }

    public async Task SaveAsync(IReadOnlyList<string> interests, CancellationToken cancellationToken)
    {
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(DELETE, transaction: transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
        var rows = interests.Select((text, position) => new { Position = position, Text = text }).ToList();
        await connection.ExecuteAsync(new CommandDefinition(INSERT, rows, transaction, cancellationToken: cancellationToken)).ConfigureAwait(false);
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    //

    private const string SELECT = "SELECT text FROM interests ORDER BY position";
    private const string DELETE = "DELETE FROM interests";
    private const string INSERT = "INSERT INTO interests (position, text) VALUES (@Position, @Text)";

    private readonly ISqliteConnectionFactory factory;
}