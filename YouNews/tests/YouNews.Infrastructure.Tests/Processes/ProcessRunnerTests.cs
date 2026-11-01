using YouNews.Infrastructure.Processes;

namespace YouNews.Infrastructure.Tests.Processes;

[TestClass]
public sealed class ProcessRunnerTests
{
    [TestMethod]
    public async Task CapturesOutputAndExitCode()
    {
        var sut = new ProcessRunner();

        var result = await sut.RunAsync("cmd.exe", ["/c", "echo hello & exit 3"], null, TimeSpan.FromSeconds(30), CancellationToken.None);

        Assert.AreEqual(3, result.ExitCode, result.StandardError);
        StringAssert.Contains(result.StandardOutput, "hello");
    }

    [TestMethod]
    public async Task ThrowsTimeoutWhenProgramOverruns()
    {
        var sut = new ProcessRunner();

        await Assert.ThrowsExactlyAsync<TimeoutException>(() =>
            sut.RunAsync("cmd.exe", ["/c", "ping -n 30 127.0.0.1"], null, TimeSpan.FromMilliseconds(300), CancellationToken.None));
    }

    [TestMethod]
    public async Task ThrowsCancellationWhenCallerCancels()
    {
        var sut = new ProcessRunner();
        using var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            sut.RunAsync("cmd.exe", ["/c", "ping -n 30 127.0.0.1"], null, TimeSpan.FromSeconds(60), source.Token));
    }
}