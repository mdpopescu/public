using Microsoft.Extensions.Options;
using YouNews.Infrastructure.Storage;

namespace YouNews.Infrastructure.Tests.Storage;

[TestClass]
public class ModelSelectionTests
{
    [TestInitialize]
    public async Task Initialize() => await fixture.InitializeAsync();

    [TestCleanup]
    public void Cleanup() => fixture.Dispose();

    [TestMethod]
    public async Task GetAsync_NothingSelected_ReturnsTheConfiguredModel()
    {
        var sut = CreateSut("configured-model");

        Assert.AreEqual("configured-model", await sut.GetAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task GetAsync_AfterSelect_ReturnsTheSelectedModel()
    {
        var sut = CreateSut("configured-model");

        await sut.SelectAsync("chosen-model", CancellationToken.None);

        Assert.AreEqual("chosen-model", await sut.GetAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task SelectAsync_Twice_KeepsTheLast()
    {
        var sut = CreateSut("configured-model");

        await sut.SelectAsync("first", CancellationToken.None);
        await sut.SelectAsync("second", CancellationToken.None);

        Assert.AreEqual("second", await sut.GetAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task GetAsync_NewInstanceOnTheSameDatabase_ReturnsTheSelectedModel()
    {
        await CreateSut("configured-model").SelectAsync("chosen-model", CancellationToken.None);

        var sut = CreateSut("configured-model");

        Assert.AreEqual("chosen-model", await sut.GetAsync(CancellationToken.None));
    }

    //

    private readonly StorageFixture fixture = new();

    private SqliteModelSelection CreateSut(string configuredModel) =>
        new(fixture.Factory, Options.Create(new YouNewsOptions { LanguageModel = { Model = configuredModel } }));
}