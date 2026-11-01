namespace YouNews.Core.Ports;

/// <summary>Lets the user choose the language model.</summary>
public interface IModelSelector
{
    /// <summary>Makes the given model the one in use from now on.</summary>
    Task SelectAsync(string model, CancellationToken cancellationToken);
}