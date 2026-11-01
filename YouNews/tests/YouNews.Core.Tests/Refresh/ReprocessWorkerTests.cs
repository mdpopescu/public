using Microsoft.Extensions.Time.Testing;
using YouNews.Core.Model;
using YouNews.Core.Refresh;
using YouNews.Core.Setup;

namespace YouNews.Core.Tests.Refresh;

[TestClass]
public sealed class ReprocessWorkerTests
{
    [TestInitialize]
    public void Initialize()
    {
        time = new FakeTimeProvider();
        queue = new FakeReprocessQueue();
        reprocessor = new FakeEntryReprocessor(queue);
        setupMonitor = new FakeSetupMonitor();
        stopping = new CancellationTokenSource();
        sut = CreateSut(setupMonitor);
    }

    [TestCleanup]
    public void Cleanup() => stopping.Dispose();

    [TestMethod]
    public async Task RequestAsync_WorkerIsIdle_WakesItAndRunsTheJob()
    {
        var run = sut.RunAsync(stopping.Token);

        await sut.RequestAsync(5, CancellationToken.None);

        await WaitUntilAsync(() => reprocessor.Processed.Count == 1);
        CollectionAssert.AreEqual(new long[] { 5 }, reprocessor.Processed);
        await StopAsync(run);
    }

    [TestMethod]
    public async Task RunAsync_JobsAlreadyQueuedAtStart_RunWithoutAnyWakeUp()
    {
        await queue.RequestAsync(1, CancellationToken.None);
        await queue.RequestAsync(2, CancellationToken.None);

        var run = sut.RunAsync(stopping.Token);

        await WaitUntilAsync(() => reprocessor.Processed.Count == 2);
        CollectionAssert.AreEqual(new long[] { 1, 2 }, reprocessor.Processed);
        await StopAsync(run);
    }

    [TestMethod]
    public async Task RunAsync_Job_PassesTheCurrentSetupToTheReprocessor()
    {
        setupMonitor.Snapshot = new SetupSnapshot(["gemma4-26b"], "RTX 5060 Ti", time.GetUtcNow(), null);
        await queue.RequestAsync(1, CancellationToken.None);

        var run = sut.RunAsync(stopping.Token);

        await WaitUntilAsync(() => reprocessor.Processed.Count == 1);
        Assert.AreSame(setupMonitor.Snapshot, reprocessor.Setups.Single());
        await StopAsync(run);
    }

    [TestMethod]
    public async Task RunAsync_JobFails_RecordsTheReasonAndStillRunsTheNextJob()
    {
        reprocessor.Handler = (job, _) => job.EntryId == 1 ? throw new InvalidOperationException("model is down") : Task.CompletedTask;
        await queue.RequestAsync(1, CancellationToken.None);
        await queue.RequestAsync(2, CancellationToken.None);

        var run = sut.RunAsync(stopping.Token);

        await WaitUntilAsync(() => reprocessor.Processed.Count == 1);
        CollectionAssert.AreEqual(new long[] { 2 }, reprocessor.Processed);
        Assert.AreEqual("model is down", queue.Failures[1]);
        CollectionAssert.Contains(queue.Queued, 1L, "a failed request stays so the failure can be shown");
        await StopAsync(run);
    }

    [TestMethod]
    public async Task RunAsync_FetchingTheSetupFails_RecordsTheFailureAndKeepsRunning()
    {
        sut = CreateSut(new FailingSetupMonitor());
        await queue.RequestAsync(1, CancellationToken.None);

        var run = sut.RunAsync(stopping.Token);

        await WaitUntilAsync(() => queue.Failures.ContainsKey(1));
        Assert.AreEqual("setup unavailable", queue.Failures[1]);
        await sut.RequestAsync(2, CancellationToken.None);
        await WaitUntilAsync(() => queue.Failures.ContainsKey(2));
        Assert.IsFalse(run.IsCompleted, "the worker must not die");
        await StopAsync(run);
    }

    [TestMethod]
    public async Task RunAsync_JobTimesOutWithoutStopping_CountsAsAFailure()
    {
        reprocessor.Handler = (_, _) => throw new OperationCanceledException("the model took too long");
        await queue.RequestAsync(1, CancellationToken.None);

        var run = sut.RunAsync(stopping.Token);

        await WaitUntilAsync(() => queue.Failures.ContainsKey(1));
        Assert.IsFalse(run.IsCompleted);
        await StopAsync(run);
    }

    [TestMethod]
    public async Task RunAsync_StoppedWhileAJobRuns_CompletesAndLeavesTheJobQueuedWithoutFailure()
    {
        var started = new TaskCompletionSource();
        reprocessor.Handler = async (_, cancellationToken) =>
        {
            started.SetResult();
            await Task.Delay(Timeout.Infinite, cancellationToken);
        };
        await queue.RequestAsync(1, CancellationToken.None);
        var run = sut.RunAsync(stopping.Token);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(10));

        await StopAsync(run);

        CollectionAssert.AreEqual(new long[] { 1 }, queue.Queued);
        Assert.IsEmpty(queue.Failures, "the request resumes after a restart");
    }

    [TestMethod]
    public async Task RunAsync_QueueFails_WaitsTheRetryDelayThenGoesOn()
    {
        queue.FailingCalls = 1;
        await queue.RequestAsync(1, CancellationToken.None);
        var run = sut.RunAsync(stopping.Token);

        await WaitUntilAsync(() => reprocessor.Processed.Count == 1, ReprocessWorker.RETRY_DELAY);

        Assert.IsGreaterThanOrEqualTo(2, queue.GetNextCalls);
        await StopAsync(run);
    }

    [TestMethod]
    public async Task RunAsync_QueueFails_DoesNotRetryBeforeTheDelayHasPassed()
    {
        queue.FailingCalls = 1;
        var failed = new TaskCompletionSource();
        queue.OnGetNextFailed = () => failed.TrySetResult();
        var run = sut.RunAsync(stopping.Token);
        await failed.Task.WaitAsync(TimeSpan.FromSeconds(10));

        time.Advance(ReprocessWorker.RETRY_DELAY - TimeSpan.FromSeconds(1));
        for (var turn = 0; turn < 1000; turn++)
        {
            await Task.Yield();
        }

        Assert.AreEqual(1, queue.GetNextCalls, "the retry delay must not elapse early");
        await StopAsync(run);
    }

    [TestMethod]
    public async Task RunAsync_StoppedWhileWaitingToRetry_CompletesWithoutThrowing()
    {
        queue.FailingCalls = 1000;
        var failed = new TaskCompletionSource();
        queue.OnGetNextFailed = () => failed.TrySetResult();
        var run = sut.RunAsync(stopping.Token);
        await failed.Task.WaitAsync(TimeSpan.FromSeconds(10));

        await StopAsync(run);
    }

    [TestMethod]
    public async Task RunAsync_StoppedWhileIdle_Completes()
    {
        var run = sut.RunAsync(stopping.Token);

        await StopAsync(run);
    }

    //

    private FakeTimeProvider time = null!;
    private FakeReprocessQueue queue = null!;
    private FakeEntryReprocessor reprocessor = null!;
    private FakeSetupMonitor setupMonitor = null!;
    private CancellationTokenSource stopping = null!;
    private ReprocessWorker sut = null!;

    private ReprocessWorker CreateSut(ISetupMonitor monitor) => new(new ReprocessDependencies(queue, reprocessor, monitor, new FakeInterestStore(), time));

    private async Task StopAsync(Task run)
    {
        await stopping.CancelAsync();
        await run.WaitAsync(TimeSpan.FromSeconds(10));
    }

    private async Task WaitUntilAsync(Func<bool> condition, TimeSpan? advanceBy = null)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!condition())
        {
            Assert.IsLessThan(deadline, DateTime.UtcNow, "timed out waiting for the worker");
            if (advanceBy is { } step)
            {
                time.Advance(step);
            }

            await Task.Yield();
        }
    }
}