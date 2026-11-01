using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace YouNews.Infrastructure.Server;

public sealed class SshRemoteCommandRunner : IRemoteCommandRunner
{
    public SshRemoteCommandRunner(IOptions<YouNewsOptions> options)
    {
        this.options = options.Value.Server;
    }

    public async Task<string> RunAsync(string command, CancellationToken cancellationToken)
    {
        using var key = new PrivateKeyFile(options.PrivateKeyPath);
        using var client = new SshClient(options.Host, options.Port, options.User, key);
        await client.ConnectAsync(cancellationToken).ConfigureAwait(false);

        using var remote = client.CreateCommand(command);
        await remote.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        if (remote.ExitStatus != 0)
            throw new InvalidOperationException($"`{command}` exited with code {remote.ExitStatus}: {remote.Error.Trim()}");

        return remote.Result;
    }

    //

    private readonly ServerOptions options;
}