using Microsoft.Extensions.Time.Testing;
using YouNews.Core.Model;
using YouNews.Core.Refresh;
using YouNews.Core.Tests.Fakes;

namespace YouNews.Core.Tests.Refresh;

[TestClass]
public sealed class EntryReprocessTests
{
    [TestInitialize]
    public void Initialize()
    {
        time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 1, 23, 30, 0, TimeSpan.Zero));
        catalog = new FakeCatalog { UnknownVideosHaveNoDate = true };
        captions = new FakeCaptionSource();
        writer = new FakeBriefingWriter { Summary = "New summary", Opinion = "New opinion" };
        search = new FakeWebSearch();
        images = new FakeImageSource();
        entries = new FakeEntryStore();
        sut = new VideoProcessor(new VideoProcessorDependencies(catalog, captions, new FakeSpeechTranscriber(), writer, search, images, entries, time));
    }

    [TestCleanup]
    public void Cleanup() => sut.Dispose();

    [TestMethod]
    public async Task ReprocessAsync_Job_StoresARevisionWithNewTextsSourcesAndRelatedEntries()
    {
        entries.Digests.Add(new EntryDigest(7, new DateOnly(2026, 10, 1), "Earlier", ["research"], "Earlier summary"));
        writer.Analysis = new VideoAnalysis("Headline", ["research"], [], [7], ["query one"]);
        search.Results.Add(new WebResult("A source", "https://example.com/a", "snippet"));

        await sut.ReprocessAsync(JOB, null, ["local models"], CancellationToken.None);

        var revision = entries.Revised.Single();
        Assert.AreEqual(5, revision.EntryId);
        Assert.AreEqual("New summary", revision.Summary);
        Assert.AreEqual("New opinion", revision.Opinion);
        CollectionAssert.AreEqual(new[] { new WebSource("A source", "https://example.com/a") }, revision.WebSources.ToArray());
        CollectionAssert.AreEqual(new long[] { 7 }, revision.RelatedEntryIds.ToArray());
        Assert.IsEmpty(entries.Added, "no new entry");
    }

    [TestMethod]
    public async Task ReprocessAsync_Job_GivesTheWriterTheVideosTitleChannelSetupAndInterests()
    {
        var setup = new SetupSnapshot(["gemma4-26b"], "RTX 5060 Ti", time.GetUtcNow(), null);

        await sut.ReprocessAsync(JOB, setup, ["local models"], CancellationToken.None);

        var request = writer.LastOpinionRequest!;
        Assert.AreEqual("Title of video", request.Title);
        Assert.AreEqual("Channel One", request.ChannelName);
        Assert.AreSame(setup, request.Setup);
        CollectionAssert.AreEqual(new[] { "local models" }, request.Interests.ToArray());
    }

    [TestMethod]
    public async Task ReprocessAsync_EntryIsAmongTheCandidates_ItIsRemovedSoItCannotRelateToItself()
    {
        entries.Digests.Add(new EntryDigest(5, new DateOnly(2026, 10, 2), "Itself", ["research"], "Own summary"));
        entries.Digests.Add(new EntryDigest(7, new DateOnly(2026, 10, 1), "Earlier", ["research"], "Earlier summary"));
        writer.Analysis = new VideoAnalysis("Headline", ["research"], [], [5, 7], []);

        await sut.ReprocessAsync(JOB, null, [], CancellationToken.None);

        CollectionAssert.AreEqual(new long[] { 7 }, writer.LastCandidates!.Select(candidate => candidate.Id).ToArray());
        CollectionAssert.AreEqual(new long[] { 7 }, entries.Revised.Single().RelatedEntryIds.ToArray());
    }

    [TestMethod]
    public async Task ReprocessAsync_Job_NeitherAsksTheCatalogForDetailsNorCapturesImages()
    {
        await sut.ReprocessAsync(JOB, null, [], CancellationToken.None);

        Assert.IsEmpty(catalog.DetailRequests);
        Assert.AreEqual(0, images.Captures);
    }

    [TestMethod]
    public async Task ReprocessAsync_AnalysisFails_StoresNothingAndThrows()
    {
        writer.FailingTitles.Add("Title of video");

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ReprocessAsync(JOB, null, [], CancellationToken.None));

        Assert.IsEmpty(entries.Revised);
    }

    [TestMethod]
    public async Task ReprocessAsync_OpinionStepFails_StoresNothingAndThrows()
    {
        writer.DuringOpinion = () => throw new TimeoutException("model timed out");

        await Assert.ThrowsAsync<TimeoutException>(() => sut.ReprocessAsync(JOB, null, [], CancellationToken.None));

        Assert.IsEmpty(entries.Revised);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("  \n ")]
    public async Task ReprocessAsync_BlankOpinion_StoresNothingAndThrows(string opinion)
    {
        writer.Opinion = opinion;

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ReprocessAsync(JOB, null, [], CancellationToken.None));

        Assert.IsEmpty(entries.Revised, "good text must not be overwritten with nothing");
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("  \n ")]
    public async Task ReprocessAsync_BlankSummary_StoresNothingAndThrows(string summary)
    {
        writer.Summary = summary;

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ReprocessAsync(JOB, null, [], CancellationToken.None));

        Assert.IsEmpty(entries.Revised, "good text must not be overwritten with nothing");
    }

    [TestMethod]
    public async Task ProcessAsyncAndReprocessAsync_RunTogether_TakeTurns()
    {
        var release = new TaskCompletionSource();
        var inOpinion = new TaskCompletionSource();
        writer.DuringOpinion = () =>
        {
            inOpinion.TrySetResult();
            return release.Task;
        };
        var processing = sut.ProcessAsync(VIDEO, CONTEXT, CancellationToken.None);
        await inOpinion.Task.WaitAsync(TimeSpan.FromSeconds(10));

        var reprocessing = sut.ReprocessAsync(JOB with { Title = "Second" }, null, [], CancellationToken.None);

        Assert.IsFalse(reprocessing.IsCompleted);
        CollectionAssert.AreEqual(new[] { "analyze Title of video", "opinion Title of video" }, writer.Events, "the second call must not have started");
        writer.DuringOpinion = null;
        release.SetResult();
        await Task.WhenAll(processing, reprocessing).WaitAsync(TimeSpan.FromSeconds(10));
        CollectionAssert.AreEqual(
            new[] { "analyze Title of video", "opinion Title of video", "opinion done Title of video", "analyze Second", "opinion Second", "opinion done Second" },
            writer.Events);
    }

    //

    private static readonly ReprocessJob JOB = new(5, "video", "Title of video", "Channel One");
    private static readonly TrackedVideo VIDEO = new(1, 1, "video", "Title of video", VideoState.Pending, 0, null, new DateTimeOffset(2026, 11, 1, 23, 30, 0, TimeSpan.Zero), new DateOnly(2026, 10, 19));
    private static readonly ProcessingContext CONTEXT = new(new Dictionary<long, Channel> { [1] = new Channel(1, "https://youtube.com/@one", "Channel One") }, null, []);

    private FakeTimeProvider time = null!;
    private FakeCatalog catalog = null!;
    private FakeCaptionSource captions = null!;
    private FakeBriefingWriter writer = null!;
    private FakeWebSearch search = null!;
    private FakeImageSource images = null!;
    private FakeEntryStore entries = null!;
    private VideoProcessor sut = null!;
}