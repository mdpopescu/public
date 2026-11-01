using System.Net;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.Speech;
using YouNews.Infrastructure.Tests.YouTube;

namespace YouNews.Infrastructure.Tests.Speech;

[TestClass]
public sealed class SpeechModelStoreTests
{
    [TestInitialize]
    public void Setup()
    {
        environment = new TestEnvironment();
        environment.Options.Speech.ModelArchiveUrl = "https://example.test/models/parakeet-test.tar.bz2";
        environment.Options.Speech.VoiceDetectorUrl = "https://example.test/models/silero_vad.onnx";
        httpHandler = new FakeHandler();
        tar = new FakeProcessRunner { Behavior = Unpack };
        sut = new SpeechModelStore(new FakeHttpClientFactory(httpHandler), tar, environment.Wrapped);
    }

    [TestCleanup]
    public void Cleanup() => environment.Dispose();

    [TestMethod]
    public async Task DownloadsAndUnpacksOnFirstUse()
    {
        var files = await sut.EnsureModelsAsync(CancellationToken.None);

        AssertAllExist(files);
        Assert.AreEqual(Path.Combine(environment.Options.SpeechModelsDirectory, "parakeet-test", "encoder.int8.onnx"), files.Encoder);
        Assert.HasCount(2, httpHandler.Requests, string.Join(", ", httpHandler.Requests));
        var invocation = tar.Invocations.Single();
        Assert.AreEqual("tar.exe", invocation.FileName);
        Assert.AreEqual("-xf", invocation.Arguments[0], invocation.ToString());
    }

    [TestMethod]
    public async Task LeavesNoTemporaryFilesBehind()
    {
        await sut.EnsureModelsAsync(CancellationToken.None);

        var entries = Directory.GetFileSystemEntries(environment.Options.SpeechModelsDirectory).Select(Path.GetFileName).Order();
        CollectionAssert.AreEqual(new[] { "parakeet-test", "silero_vad.onnx" }, entries.ToArray());
    }

    [TestMethod]
    public async Task DoesNotDownloadWhenTheModelsArePresent()
    {
        await sut.EnsureModelsAsync(CancellationToken.None);
        httpHandler.Requests.Clear();
        tar.Invocations.Clear();

        var files = await sut.EnsureModelsAsync(CancellationToken.None);

        AssertAllExist(files);
        Assert.IsEmpty(httpHandler.Requests, string.Join(", ", httpHandler.Requests));
        Assert.IsEmpty(tar.Invocations, "nothing may be unpacked again");
    }

    [TestMethod]
    public async Task InterruptedUnpackingLeavesNothingUsableAndIsRepeatedNextTime()
    {
        tar.Behavior = invocation =>
        {
            var partial = Path.Combine(invocation.ValueAfter("-C"), "parakeet-test");
            Directory.CreateDirectory(partial);
            File.WriteAllText(Path.Combine(partial, "encoder.int8.onnx"), "half");
            return new ProcessResult(1, "", "tar: Unexpected EOF in archive");
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.EnsureModelsAsync(CancellationToken.None));

        Assert.Contains("Unexpected EOF", exception.Message);
        Assert.IsFalse(Directory.Exists(Path.Combine(environment.Options.SpeechModelsDirectory, "parakeet-test")), "a half-unpacked model must not look complete");
        Assert.IsEmpty(Directory.GetDirectories(environment.Options.SpeechModelsDirectory));

        tar.Behavior = Unpack;
        AssertAllExist(await sut.EnsureModelsAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task LeftoverFromAKilledProcessIsNotTreatedAsComplete()
    {
        var leftover = Path.Combine(environment.Options.SpeechModelsDirectory, "incoming-abc", "unpacked", "parakeet-test");
        Directory.CreateDirectory(leftover);
        File.WriteAllText(Path.Combine(leftover, "encoder.int8.onnx"), "half");

        var files = await sut.EnsureModelsAsync(CancellationToken.None);

        AssertAllExist(files);
        Assert.HasCount(2, httpHandler.Requests, "the model must be downloaded again");
        Assert.IsFalse(Directory.Exists(Path.Combine(environment.Options.SpeechModelsDirectory, "incoming-abc")), "stale leftovers must be removed");
    }

    [TestMethod]
    public async Task DownloadFailureNamesTheAddressAndStatus()
    {
        httpHandler.Status = HttpStatusCode.NotFound;

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.EnsureModelsAsync(CancellationToken.None));

        Assert.Contains("https://example.test/models/parakeet-test.tar.bz2", exception.Message);
        Assert.Contains("404", exception.Message);
        Assert.IsEmpty(tar.Invocations, "nothing may be unpacked after a failed download");
        Assert.IsEmpty(Directory.GetFileSystemEntries(environment.Options.SpeechModelsDirectory));
    }

    [TestMethod]
    public async Task NetworkFailureIsReportedClearly()
    {
        httpHandler.Failure = new HttpRequestException("No such host is known.");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.EnsureModelsAsync(CancellationToken.None));

        Assert.Contains("Could not download the speech model file", exception.Message);
        Assert.Contains("No such host is known.", exception.Message);
    }

    [TestMethod]
    public async Task VoiceDetectorFailureLeavesNoPartialFile()
    {
        await sut.EnsureModelsAsync(CancellationToken.None);
        File.Delete(Path.Combine(environment.Options.SpeechModelsDirectory, "silero_vad.onnx"));
        httpHandler.Status = HttpStatusCode.InternalServerError;

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.EnsureModelsAsync(CancellationToken.None));

        Assert.IsFalse(File.Exists(Path.Combine(environment.Options.SpeechModelsDirectory, "silero_vad.onnx.download")));
    }

    //

    private TestEnvironment environment = null!;
    private FakeHandler httpHandler = null!;
    private FakeProcessRunner tar = null!;
    private SpeechModelStore sut = null!;

    private static ProcessResult Unpack(FakeProcessRunner.Invocation invocation)
    {
        var model = Path.Combine(invocation.ValueAfter("-C"), "parakeet-test");
        Directory.CreateDirectory(model);
        foreach (var name in new[] { "encoder.int8.onnx", "decoder.int8.onnx", "joiner.int8.onnx", "tokens.txt" })
            File.WriteAllText(Path.Combine(model, name), name);
        return new ProcessResult(0, "", "");
    }

    private static void AssertAllExist(SpeechModelFiles files)
    {
        foreach (var path in new[] { files.Encoder, files.Decoder, files.Joiner, files.Tokens, files.VoiceDetector })
            Assert.IsTrue(File.Exists(path), $"missing {path}");
    }

    private sealed class FakeHandler : HttpMessageHandler
    {
        public List<string> Requests { get; } = [];

        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;

        public Exception? Failure { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request.RequestUri!.ToString());
            if (Failure != null)
                throw Failure;

            return Task.FromResult(new HttpResponseMessage(Status) { Content = new ByteArrayContent([1, 2, 3]) });
        }
    }

    private sealed class FakeHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            Assert.AreEqual(SpeechModelStore.HTTP_CLIENT_NAME, name);
            return new HttpClient(handler, false);
        }
    }
}