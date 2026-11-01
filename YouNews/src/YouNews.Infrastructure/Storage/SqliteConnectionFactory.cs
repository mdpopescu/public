using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteConnectionFactory : ISqliteConnectionFactory
{
    public SqliteConnectionFactory(IOptions<YouNewsOptions> options)
    {
        connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = options.Value.DatabasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            ForeignKeys = true,
        }.ToString();
    }

    public async Task<DbConnection> OpenAsync(CancellationToken cancellationToken)
    {
        var connection = new SqliteConnection(connectionString);
        try
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    //

    private readonly string connectionString;
}