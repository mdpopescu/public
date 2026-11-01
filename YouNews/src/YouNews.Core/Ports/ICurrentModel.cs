namespace YouNews.Core.Ports;

/// <summary>Tells which language model every call should use right now.</summary>
public interface ICurrentModel
{
    /// <summary>Gets the model in use for all language model calls.</summary>
    Task<string> GetAsync(CancellationToken cancellationToken);
}