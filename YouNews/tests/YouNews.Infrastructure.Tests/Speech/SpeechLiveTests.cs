using System.Diagnostics;
using Microsoft.Extensions.Options;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.Speech;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.Speech;

/// <summary>Runs the real yt-dlp, ffmpeg and Parakeet model files found in the repository's .scratch/speech-models folder.</summary>
[TestClass]
[TestCategory("Live")]
public sealed class SpeechLiveTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public async Task TranscribesTheFirstMinuteOfARealVideo()
    {
        var options = Options.Create(new YouNewsOptions { DataDirectory = FindScratchDirectory() });
        var processRunner = new ProcessRunner();
        var downloader = new FirstMinuteDownloader(new YouTubeAudioDownloader(new YtDlpRunner(processRunner, options), options), processRunner, options);
        var store = new SpeechModelStore(new FailingHttpClientFactory(), processRunner, options);
        using var recognizer = new SherpaSpeechRecognizer(store, options);
        var sut = new ParakeetSpeechTranscriber(downloader, processRunner, recognizer, options);

        var watch = Stopwatch.StartNew();
        var transcript = await sut.TranscribeAsync("zhnJo7s91ls", CancellationToken.None);
        watch.Stop();

        var text = string.Join(' ', transcript.Segments.Select(segment => segment.Text));
        TestContext.WriteLine($"Segments: {transcript.Segments.Count}, words: {transcript.WordCount}");
        TestContext.WriteLine($"Time {watch.Elapsed.TotalSeconds:0.0} s for {FIRST_MINUTE_SECONDS} s of audio, real-time factor {watch.Elapsed.TotalSeconds / FIRST_MINUTE_SECONDS:0.00} (includes download and model load)");
        TestContext.WriteLine($"Peak working set {Process.GetCurrentProcess().PeakWorkingSet64 / 1024 / 1024} MB");
        TestContext.WriteLine("Text: " + text);
        Assert.IsGreaterThan(20, transcript.WordCount, text);
        Assert.IsEmpty(Directory.GetFiles(options.Value.WorkDirectory), "the work directory must be left clean");
    }

    //

    private const int FIRST_MINUTE_SECONDS = 60;

    private static string FindScratchDirectory()
    {
        for (var folder = new DirectoryInfo(AppContext.BaseDirectory); folder != null; folder = folder.Parent)
        {
            var scratch = Path.Combine(folder.FullName, ".scratch");
            if (Directory.Exists(Path.Combine(scratch, "speech-models")))
                return scratch;
        }

        throw new AssertInconclusiveException("There is no .scratch/speech-models folder above the test output.");
    }

    private sealed class FailingHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => throw new InvalidOperationException("The model files must already be present.");
    }

    private sealed class FirstMinuteDownloader(IAudioDownloader inner, IProcessRunner processRunner, IOptions<YouNewsOptions> options) : IAudioDownloader
    {
        public async Task<string> DownloadAudioAsync(string youTubeId, CancellationToken cancellationToken)
        {
            var full = await inner.DownloadAudioAsync(youTubeId, cancellationToken);
            var trimmed = Path.Combine(options.Value.WorkDirectory, $"{youTubeId}-{Guid.NewGuid():N}-trimmed.m4a");
            try
            {
                string[] arguments = ["-nostdin", "-y", "-i", full, "-t", FIRST_MINUTE_SECONDS.ToString(), "-c:a", "aac", trimmed];
                var result = await processRunner.RunAsync(options.Value.Tools.FfmpegPath, arguments, null, TimeSpan.FromMinutes(5), cancellationToken);
                Assert.AreEqual(0, result.ExitCode, result.StandardError);
                return trimmed;
            }
            finally
            {
                File.Delete(full);
            }
        }
    }
}