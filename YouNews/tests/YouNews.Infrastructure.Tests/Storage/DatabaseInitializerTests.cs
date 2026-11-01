using Dapper;
using YouNews.Core.Model;
using YouNews.Infrastructure.Storage;

namespace YouNews.Infrastructure.Tests.Storage;

[TestClass]
public class DatabaseInitializerTests
{
    private readonly StorageFixture fixture = new();

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

        var channels = await new SqliteChannelStore(fixture.Factory).ListAsync(CancellationToken.None);

        var channel = channels.Single();
        Assert.AreEqual("UCawZsQWqfGSbCI5yjkdVkTA", channel.YouTubeId);
        Assert.AreEqual("https://www.youtube.com/@matthew_berman/", channel.Url);
        Assert.AreEqual("Matthew Berman", channel.Name);
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
        CollectionAssert.AreEqual(new[] { 1 }, versions, "version table should list each migration exactly once");
    }

    [TestMethod]
    public async Task InitializeAsync_ExistingDatabaseWithNewerMigration_AppliesOnlyTheNewOne()
    {
        await fixture.InitializeAsync();
        var extended = new FakeMigrations(new DatabaseMigrations().Scripts.Append("CREATE TABLE extra (id INTEGER PRIMARY KEY)").ToList());

        await new DatabaseInitializer(fixture.Factory, extended, TimeProvider.System, fixture.Options).InitializeAsync(CancellationToken.None);

        await using var connection = await fixture.Factory.OpenAsync(CancellationToken.None);
        var versions = (await connection.QueryAsync<int>("SELECT version FROM schema_version ORDER BY version")).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2 }, versions);
        Assert.HasCount(1, (await new SqliteChannelStore(fixture.Factory).ListAsync(CancellationToken.None)), "upgrade must not seed again");
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

    private sealed class FakeMigrations : IDatabaseMigrations
    {
        public IReadOnlyList<string> Scripts { get; }

        public FakeMigrations(IReadOnlyList<string> scripts)
        {
            Scripts = scripts;
        }
    }
}