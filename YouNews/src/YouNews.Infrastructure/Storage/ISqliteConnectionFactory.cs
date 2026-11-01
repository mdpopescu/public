using System.Data.Common;

namespace YouNews.Infrastructure.Storage;

/// <summary>Opens connections to the application database.</summary>
public interface ISqliteConnectionFactory
{
    /// <summary>Opens a connection with foreign keys enforced. The caller disposes it.</summary>
    Task<DbConnection> OpenAsync(CancellationToken cancellationToken);
}