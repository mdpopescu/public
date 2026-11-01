using YouNews.Core.Refresh;
using YouNews.Web.Hosting;

namespace YouNews.Web.Tests;

[TestClass]
public sealed class ReprocessServiceTests
{
    [TestMethod]
    public async Task RunsTheWorkerUntilStopped()
    {
        var worker = new RecordingWorker();
        using var sut = new ReprocessService(worker);

        await sut.StartAsync(CancellationToken.None);
        await worker.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.IsFalse(worker.Stopping.IsCancellationRequested, "the worker runs while the application runs");

        await sut.StopAsync(CancellationToken.None);

        Assert.IsTrue(worker.Stopping.IsCancellationRequested, "the worker is told when the application stops");
    }

    //

    private sealed class RecordingWorker : IReprocessWorker
    {
        public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public CancellationToken Stopping { get; private set; }

        public async Task RunAsync(CancellationToken stopping)
        {
            Stopping = stopping;
            Started.SetResult();
            try
            {
                await Task.Delay(Timeout.Infinite, stopping);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}