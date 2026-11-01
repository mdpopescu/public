namespace YouNews.Infrastructure.Server;

/// <summary>Runs shell commands on the AI server.</summary>
public interface IRemoteCommandRunner
{
    /// <summary>Runs a command and returns its standard output. Throws when the server cannot be reached or the command exits with a non-zero code.</summary>
    Task<string> RunAsync(string command, CancellationToken cancellationToken);
}