using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using YouNews.Core.Model;
using YouNews.Infrastructure;
using YouNews.Infrastructure.Storage;

namespace YouNews.Infrastructure.Tests.Storage;

/// <summary>A real SQLite database in a folder unique to one test, deleted on disposal.</summary>
public sealed class StorageFixture : IDisposable
{
    public IOptions<YouNewsOptions> Options { get; }

    public SqliteConnectionFactory Factory { get; }

    public StorageFixture()
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "storage-tests", Guid.NewGuid().ToString("N"));
        Options = Microsoft.Extensions.Options.Options.Create(new YouNewsOptions { DataDirectory = directory });
        Factory = new SqliteConnectionFactory(Options);
    }

    public DatabaseInitializer CreateInitializer() =>
        new(Factory, new DatabaseMigrations(), TimeProvider.System, Options);

    public async Task<StorageFixture> InitializeAsync()
    {
        await CreateInitializer().InitializeAsync(CancellationToken.None);
        return this;
    }

    public async Task<(Channel Channel, TrackedVideo Video)> AddChannelWithVideoAsync(string suffix = "1")
    {
        var channels = new SqliteChannelStore(Factory);
        var videos = new SqliteVideoStore(Factory);
        var channel = await channels.AddAsync(new ChannelDetails($"UC{suffix}", $"https://www.youtube.com/@c{suffix}/", $"Channel {suffix}"), CancellationToken.None);
        var seen = new DateTimeOffset(2026, 10, 30, 8, 0, 0, TimeSpan.Zero);
        await videos.AddAsync(new NewVideo(channel!.Id, $"vid{suffix}", $"Video {suffix}", VideoState.Pending, null, seen), CancellationToken.None);
        var video = (await videos.ListByStateAsync(VideoState.Pending, CancellationToken.None)).Single(v => v.YouTubeId == $"vid{suffix}");
        return (channel, video);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        var directory = Options.Value.DataDirectory;
        if (Directory.Exists(directory))
            Directory.Delete(directory, true);
    }
}