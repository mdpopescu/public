namespace YouNews.Core.Ports;

/// <summary>Looks at the user's AI server.</summary>
public interface IServerInspector
{
    /// <summary>Lists the names of the models the server offers. Throws when the server cannot be reached.</summary>
    Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken);

    /// <summary>Describes the server's GPUs, CPU and memory as readable text. Throws when the server cannot be reached.</summary>
    Task<string> DescribeHardwareAsync(CancellationToken cancellationToken);
}