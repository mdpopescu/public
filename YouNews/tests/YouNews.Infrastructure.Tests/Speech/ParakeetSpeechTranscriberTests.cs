using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.Speech;
using YouNews.Infrastructure.Tests.YouTube;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.Speech;

[TestClass]
public sealed class ParakeetSpeechTranscriberTests
{
    [TestInitialize]
    public void Setup()
    {
        environment = new TestEnvironment();
        environment.Options.Tools.FfmpegPath = "C:/tools/ffmpeg.exe";
        downloader = new FakeDownloader(environment.Options.WorkDirectory);
        ffmpeg = new FakeProcessRunner { Behavior = ConvertToWav };
        recognizer = new FakeRecognizer();
        sut = new ParakeetSpeechTranscriber(downloader, ffmpeg, recognizer, environment.Wrapped);
    }

    [TestCleanup]
    public void Cleanup() => environment.Dispose();

    [TestMethod]
    public async Task OneSegmentPerVoicedChunkWithItsStartOffset()
    {
        recognizer.Result = [new SpokenSegment(TimeSpan.FromSeconds(1.5), "Hello and welcome."), new SpokenSegment(TimeSpan.FromSeconds(62), "Today we look at speech.")];

        var transcript = await sut.TranscribeAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.AreEqual(2, transcript.Segments.Count);
        Assert.AreEqual(TimeSpan.FromSeconds(1.5), transcript.Segments[0].Start);
        Assert.AreEqual("Hello and welcome.", transcript.Segments[0].Text);
        Assert.AreEqual(TimeSpan.FromSeconds(62), transcript.Segments[1].Start);
        Assert.AreEqual(8, transcript.WordCount);
    }

    [TestMethod]
    public async Task EmptySegmentsAreSkipped()
    {
        recognizer.Result =
        [
            new SpokenSegment(TimeSpan.FromSeconds(1), "First."),
            new SpokenSegment(TimeSpan.FromSeconds(5), ""),
            new SpokenSegment(TimeSpan.FromSeconds(9), "   "),
            new SpokenSegment(TimeSpan.FromSeconds(12), "Last."),
        ];

        var transcript = await sut.TranscribeAsync("zhnJo7s91ls", CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "First.", "Last." }, transcript.Segments.Select(segment => segment.Text).ToArray());
        CollectionAssert.AreEqual(new[] { 1.0, 12.0 }, transcript.Segments.Select(segment => segment.Start.TotalSeconds).ToArray());
    }

    [TestMethod]
    public async Task ConvertsTheDownloadedAudioToSixteenKilohertzMonoWav()
    {
        await sut.TranscribeAsync("zhnJo7s91ls", CancellationToken.None);

        var invocation = ffmpeg.Invocations.Single();
        Assert.AreEqual("C:/tools/ffmpeg.exe", invocation.FileName);
        Assert.AreEqual(downloader.AudioPath, invocation.ValueAfter("-i"), invocation.ToString());
        Assert.AreEqual("16000", invocation.ValueAfter("-ar"), invocation.ToString());
        Assert.AreEqual("1", invocation.ValueAfter("-ac"), invocation.ToString());
        Assert.AreEqual(Path.GetDirectoryName(downloader.AudioPath), Path.GetDirectoryName(invocation.Arguments[^1]));
        Assert.EndsWith(".wav", invocation.Arguments[^1]);
        Assert.AreEqual(invocation.Arguments[^1], recognizer.ReceivedPath);
    }

    [TestMethod]
    public async Task DeletesAudioAndWavAfterSuccess()
    {
        await sut.TranscribeAsync("zhnJo7s91ls", CancellationToken.None);

        AssertWorkDirectoryEmpty();
    }

    [TestMethod]
    public async Task DeletesAudioWhenConversionFails()
    {
        ffmpeg.Behavior = invocation =>
        {
            File.WriteAllText(invocation.Arguments[^1], "partial");
            return new ProcessResult(1, "", "Invalid data found when processing input\n");
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.TranscribeAsync("zhnJo7s91ls", CancellationToken.None));

        Assert.Contains("Invalid data found", exception.Message);
        Assert.AreEqual(0, recognizer.Calls, "nothing may be recognized without a WAV");
        AssertWorkDirectoryEmpty();
    }

    [TestMethod]
    public async Task DeletesFilesWhenRecognitionFails()
    {
        recognizer.Failure = new InvalidDataException("bad wav");

        await Assert.ThrowsAsync<InvalidDataException>(() => sut.TranscribeAsync("zhnJo7s91ls", CancellationToken.None));

        AssertWorkDirectoryEmpty();
    }

    [TestMethod]
    public async Task DeletesFilesWhenCancelled()
    {
        recognizer.Failure = new OperationCanceledException();

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.TranscribeAsync("zhnJo7s91ls", CancellationToken.None));

        AssertWorkDirectoryEmpty();
    }

    [TestMethod]
    public async Task NoSpeechAtAllIsAFailureWithAReasonAndCleansUp()
    {
        recognizer.Result = [new SpokenSegment(TimeSpan.Zero, ""), new SpokenSegment(TimeSpan.FromSeconds(3), " ")];

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.TranscribeAsync("zhnJo7s91ls", CancellationToken.None));

        Assert.Contains("No speech was recognized", exception.Message);
        Assert.Contains("zhnJo7s91ls", exception.Message);
        AssertWorkDirectoryEmpty();
    }

    [TestMethod]
    public async Task DownloadFailureSkipsEveryLaterStep()
    {
        downloader.Failure = new InvalidOperationException("yt-dlp could not download the audio");

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.TranscribeAsync("zhnJo7s91ls", CancellationToken.None));

        Assert.AreEqual(0, ffmpeg.Invocations.Count);
        Assert.AreEqual(0, recognizer.Calls);
    }

    //

    private TestEnvironment environment = null!;
    private FakeDownloader downloader = null!;
    private FakeProcessRunner ffmpeg = null!;
    private FakeRecognizer recognizer = null!;
    private ParakeetSpeechTranscriber sut = null!;

    private static ProcessResult ConvertToWav(FakeProcessRunner.Invocation invocation)
    {
        File.WriteAllText(invocation.Arguments[^1], "wav");
        return new ProcessResult(0, "", "");
    }

    private void AssertWorkDirectoryEmpty() =>
        Assert.AreEqual(0, environment.WorkFiles().Length, "left behind: " + string.Join(", ", environment.WorkFiles()));

    private sealed class FakeDownloader(string workDirectory) : IAudioDownloader
    {
        public string AudioPath { get; } = Path.Combine(workDirectory, "zhnJo7s91ls-abc.m4a");

        public Exception? Failure { get; set; }

        public Task<string> DownloadAudioAsync(string youTubeId, CancellationToken cancellationToken)
        {
            if (Failure != null)
                throw Failure;

            Directory.CreateDirectory(workDirectory);
            File.WriteAllText(AudioPath, "audio");
            return Task.FromResult(AudioPath);
        }
    }

    private sealed class FakeRecognizer : ISpeechRecognizer
    {
        public IReadOnlyList<SpokenSegment> Result { get; set; } = [new SpokenSegment(TimeSpan.Zero, "Some words.")];

        public Exception? Failure { get; set; }

        public int Calls { get; private set; }

        public string? ReceivedPath { get; private set; }

        public Task<IReadOnlyList<SpokenSegment>> RecognizeAsync(string wavPath, CancellationToken cancellationToken)
        {
            Calls++;
            ReceivedPath = wavPath;
            if (Failure != null)
                throw Failure;

            Assert.IsTrue(File.Exists(wavPath), "the WAV must exist while recognizing");
            return Task.FromResult(Result);
        }
    }
}