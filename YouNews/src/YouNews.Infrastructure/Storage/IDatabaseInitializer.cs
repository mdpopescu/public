namespace YouNews.Infrastructure.Storage;

/// <summary>Prepares the database file so the stores can use it.</summary>
public interface IDatabaseInitializer
{
    /// <summary>
    /// Creates the data directory and the database when missing, applies migrations that have not run yet,
    /// and seeds the default channel and interests only when the database is created.
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken);
}