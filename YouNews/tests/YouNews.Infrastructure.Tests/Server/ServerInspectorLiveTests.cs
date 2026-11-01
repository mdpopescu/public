using Microsoft.Extensions.Options;
using YouNews.Infrastructure.Server;

namespace YouNews.Infrastructure.Tests.Server;

[TestClass]
public sealed class ServerInspectorLiveTests
{
    [TestMethod]
    [TestCategory("Live")]
    public async Task DescribesRealServerOverSsh()
    {
        var keyPath = Environment.GetEnvironmentVariable("YOUNEWS_SSH_KEY");
        if (string.IsNullOrEmpty(keyPath))
            Assert.Inconclusive("Set YOUNEWS_SSH_KEY to the path of the private key.");

        var options = Options.Create(new YouNewsOptions { Server = { PrivateKeyPath = keyPath } });
        using var http = new HttpClient();
        var sut = new ServerInspector(http, new SshRemoteCommandRunner(options), options);

        var hardware = await sut.DescribeHardwareAsync(CancellationToken.None);
        var models = await sut.ListModelsAsync(CancellationToken.None);

        Console.WriteLine(hardware);
        Console.WriteLine($"{models.Count} models");
        StringAssert.Contains(hardware, "GPUs:", hardware);
        Assert.DoesNotContain("unknown", hardware, hardware);
        Assert.IsNotEmpty(models, "The server should list at least one model.");
    }
}