using Microsoft.Extensions.Options;

namespace YouNews.Infrastructure.Tests.YouTube;

/// <summary>Options that point at a private data folder below the test output folder.</summary>
public sealed class TestEnvironment : IDisposable
{
    public YouNewsOptions Options { get; }

    public IOptions<YouNewsOptions> Wrapped => Microsoft.Extensions.Options.Options.Create(Options);

    public TestEnvironment()
    {
        Options = new YouNewsOptions
        {
            DataDirectory = Path.Combine(AppContext.BaseDirectory, "test-data", Guid.NewGuid().ToString("N")),
        };
        Options.Tools.TimeoutMinutes = 7;
    }

    public string[] WorkFiles() =>
        Directory.Exists(Options.WorkDirectory) ? Directory.GetFileSystemEntries(Options.WorkDirectory, "*", SearchOption.AllDirectories) : [];

    public void Dispose()
    {
        if (Directory.Exists(Options.DataDirectory))
            Directory.Delete(Options.DataDirectory, true);
    }
}