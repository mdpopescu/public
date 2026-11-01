using Dapper;
using YouNews.Core.Model;
using YouNews.Infrastructure.Storage;

namespace YouNews.Infrastructure.Tests.Storage;

[TestClass]
public class DatabaseInitializerTests
{
    [TestCleanup]
    public void Cleanup() => fixture.Dispose();

    [TestMethod]
    public async Task InitializeAsync_NewDatabase_CreatesDataDirectoryAndFile()
    {
        await fixture.CreateInitializer().InitializeAsync(CancellationToken.None);

        Assert.IsTrue(File.Exists(fixture.Options.Value.DatabasePath), "database file should exist");
    }

    [TestMethod]
    public async Task InitializeAsync_NewDatabase_SeedsMatthewBerman()
    {
        await fixture.InitializeAsync();

        var store = new SqliteChannelStore(fixture.Factory);
        var channels = await store.ListAsync(CancellationToken.None);

        var channel = channels.Single();
                Assert.AreEqual("https://www.youtube.com/@matthew_berman/", channel.Url);
        Assert.AreEqual("Matthew Berman", channel.Name);
        var sameChannelAgain = new ChannelDetails("UCawZsQWqfGSbCI5yjkdVkTA", channel.Url, channel.Name);
        Assert.IsNull(await store.AddAsync(sameChannelAgain, CancellationToken.None), "The seed must be stored under the channel's real YouTube ID.");
    }

    [TestMethod]
    public async Task InitializeAsync_NewDatabase_SeedsInterestsInOrder()
    {
        await fixture.InitializeAsync();

        var interests = await new SqliteInterestStore(fixture.Factory).GetAsync(CancellationToken.None);

        CollectionAssert.AreEqual(
            new[]
            {
                "New local models, and whether they beat what is on my server",
                "AI legislation, including rumors of it",
                "The RAM and video card market",
                "New AI research",
            },
            interests.ToArray());
    }

    [TestMethod]
    public async Task InitializeAsync_Reopened_DoesNotReseedRemovedChannelOrEditedInterests()
    {
        await fixture.InitializeAsync();
        var channels = new SqliteChannelStore(fixture.Factory);
        var interests = new SqliteInterestStore(fixture.Factory);
        var seeded = (await channels.ListAsync(CancellationToken.None)).Single();
        await channels.RemoveAsync(seeded.Id, CancellationToken.None);
        await interests.SaveAsync(["Only this"], CancellationToken.None);

        await fixture.CreateInitializer().InitializeAsync(CancellationToken.None);

        Assert.IsEmpty((await channels.ListAsync(CancellationToken.None)), "removed channel must stay removed");
        CollectionAssert.AreEqual(new[] { "Only this" }, (await interests.GetAsync(CancellationToken.None)).ToArray());
    }

    [TestMethod]
    public async Task InitializeAsync_Reopened_DoesNotRerunMigrations()
    {
        await fixture.InitializeAsync();

        await fixture.CreateInitializer().InitializeAsync(CancellationToken.None);

        await using var connection = await fixture.Factory.OpenAsync(CancellationToken.None);
        var versions = (await connection.QueryAsync<int>("SELECT version FROM schema_version")).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, versions, "version table should list each migration exactly once");
    }

    [TestMethod]
    public async Task InitializeAsync_ExistingDatabaseWithNewerMigration_AppliesOnlyTheNewOne()
    {
        await fixture.InitializeAsync();
        var extended = new FakeMigrations(new DatabaseMigrations().Scripts.Append("CREATE TABLE extra (id INTEGER PRIMARY KEY)").ToList());

        await new DatabaseInitializer(fixture.Factory, extended, TimeProvider.System, fixture.Options).InitializeAsync(CancellationToken.None);

        await using var connection = await fixture.Factory.OpenAsync(CancellationToken.None);
        var versions = (await connection.QueryAsync<int>("SELECT version FROM schema_version ORDER BY version")).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, versions);
        Assert.HasCount(1, (await new SqliteChannelStore(fixture.Factory).ListAsync(CancellationToken.None)), "upgrade must not seed again");
    }

    [TestMethod]
    public async Task InitializeAsync_EntriesFiledUnderTheDiscoveryDay_MovesDatedOnesToThePublicationDay()
    {
        var beforeTheFix = new FakeMigrations(new DatabaseMigrations().Scripts.Take(2).ToList());
        await new DatabaseInitializer(fixture.Factory, beforeTheFix, TimeProvider.System, fixture.Options).InitializeAsync(CancellationToken.None);
        await using var connection = await fixture.Factory.OpenAsync(CancellationToken.None);
        await connection.ExecuteAsync("""
            INSERT INTO videos (id, channel_id, youtube_id, title, state, first_seen_at, first_seen_ticks, published_on) VALUES
                (1, 1, 'dated', 'Dated', 'Done', '2026-11-01T10:00:00+00:00', 0, '2026-10-19'),
                (2, 1, 'undated', 'Undated', 'Done', '2026-11-01T10:00:00+00:00', 0, NULL);
            INSERT INTO entries (video_id, day, headline, summary, opinion) VALUES
                (1, '2026-11-01', 'Dated', '', ''),
                (2, '2026-11-01', 'Undated', '', '');
            """);

        await fixture.CreateInitializer().InitializeAsync(CancellationToken.None);

        var days = (await connection.QueryAsync<string>("SELECT day FROM entries ORDER BY video_id")).ToArray();
        CollectionAssert.AreEqual(new[] { "2026-10-19", "2026-11-01" }, days, "An entry whose video has no date must keep its day.");
    }

    [TestMethod]
    public async Task InitializeAsync_NewDatabase_UsesWalAndEnforcesForeignKeys()
    {
        await fixture.InitializeAsync();

        await using var connection = await fixture.Factory.OpenAsync(CancellationToken.None);
        Assert.AreEqual("wal", await connection.ExecuteScalarAsync<string>("PRAGMA journal_mode"));
        Assert.AreEqual(1, await connection.ExecuteScalarAsync<int>("PRAGMA foreign_keys"));
    }

    //

    private readonly StorageFixture fixture = new();

    private sealed class FakeMigrations : IDatabaseMigrations
    {
        public IReadOnlyList<string> Scripts { get; }

        public FakeMigrations(IReadOnlyList<string> scripts)
        {
            Scripts = scripts;
        }
    }
}