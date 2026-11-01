namespace YouNews.Infrastructure.Storage;

/// <summary>Supplies the schema scripts the initializer applies.</summary>
public interface IDatabaseMigrations
{
    /// <summary>The scripts in order. A script's position plus one is its version.</summary>
    IReadOnlyList<string> Scripts { get; }
}