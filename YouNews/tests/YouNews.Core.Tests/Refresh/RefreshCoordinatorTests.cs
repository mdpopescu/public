using Microsoft.Extensions.Time.Testing;
using YouNews.Core.Model;
using YouNews.Core.Refresh;

namespace YouNews.Core.Tests.Refresh;

[TestClass]
public sealed class RefreshCoordinatorTests
{
    [TestInitialize]
    public void Initialize()
    {
        run = new ControlledRun();
        log = new FakeRefreshLog();
        sut = new RefreshCoordinator(run, log, new FakeTimeProvider());
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        run.Finish();
        await sut.DisposeAsync();
    }

    [TestMethod]
    public async Task SecondStartIsRefusedWhileOneIsRunning()
    {
        Assert.IsTrue(sut.TryStart());
        await run.Started;

        Assert.IsFalse(sut.TryStart());
        Assert.AreEqual(1, run.Executions, "A second request must not start a second run.");
    }

    [TestMethod]
    public async Task ReportsProgressWhileRunningAndNothingAfterwards()
    {
        Assert.IsNull(sut.Current);

        sut.TryStart();
        await run.Started;
        Assert.AreEqual("Working", sut.Current?.Activity);

        run.Finish();
        await WaitUntilIdleAsync();
        Assert.IsNull(sut.Current);
    }

    [TestMethod]
    public async Task CanRunAgainAfterFinishing()
    {
        sut.TryStart();
        await run.Started;
        run.Finish();
        await WaitUntilIdleAsync();

        Assert.IsTrue(sut.TryStart());
    }

    [TestMethod]
    public async Task CrashIsLoggedAndReleasesTheRun()
    {
        run.Exception = new InvalidOperationException("database is locked");
        sut.TryStart();
        await run.Started;
        run.Finish();
        await WaitUntilIdleAsync();

        StringAssert.Contains(log.Runs.Single().Problem!, "database is locked");
        Assert.IsTrue(sut.TryStart());
    }

    [TestMethod]
    public async Task DisposingCancelsTheRunningRun()
    {
        sut.TryStart();
        await run.Started;

        await sut.DisposeAsync();

        Assert.IsTrue(run.WasCancelled);
        Assert.IsEmpty(log.Runs, "A run stopped by shutting down is not a crash.");
    }

    //

    private ControlledRun run = null!;
    private FakeRefreshLog log = null!;
    private RefreshCoordinator sut = null!;

    private async Task WaitUntilIdleAsync()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (sut.Current is not null)
        {
            await Task.Delay(5, timeout.Token);
        }
    }

    private sealed class ControlledRun : IRefreshRun
    {
        public int Executions { get; private set; }

        public bool WasCancelled { get; private set; }

        public Exception? Exception { get; set; }

        public Task Started => started.Task;

        public void Finish() => finish.TrySetResult();

        public async Task ExecuteAsync(Action<RefreshProgress> reportProgress, CancellationToken cancellationToken)
        {
            Executions++;
            reportProgress(new RefreshProgress("Working", 0, 1));
            started.TrySetResult();
            try
            {
                await finish.Task.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                WasCancelled = true;
                throw;
            }

            finish = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            if (Exception is not null)
            {
                throw Exception;
            }
        }

        private readonly TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private TaskCompletionSource finish = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}