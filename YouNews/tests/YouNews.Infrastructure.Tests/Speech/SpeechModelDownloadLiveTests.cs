using Microsoft.Extensions.Options;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.Speech;

namespace YouNews.Infrastructure.Tests.Speech;

[TestClass]
public sealed class SpeechModelDownloadLiveTests
{
    [TestMethod]
    [TestCategory("Live")]
    public async Task DownloadsAndExtractsTheRealModelFiles()
    {
        var dataDirectory = Path.Combine(AppContext.BaseDirectory, "speech-download-" + Guid.NewGuid().ToString("N"));
        var options = Options.Create(new YouNewsOptions { DataDirectory = dataDirectory });
        var sut = new SpeechModelStore(new SimpleHttpClientFactory(), new ProcessRunner(), options);

        try
        {
            var files = await sut.EnsureModelsAsync(CancellationToken.None);

            foreach (var file in new[] { files.Encoder, files.Decoder, files.Joiner, files.Tokens, files.VoiceDetector })
            {
                Assert.IsTrue(File.Exists(file), "Missing after download: " + file);
                Assert.IsGreaterThan(0L, new FileInfo(file).Length, "Empty after download: " + file);
            }
        }
        finally
        {
            if (Directory.Exists(dataDirectory))
            {
                Directory.Delete(dataDirectory, recursive: true);
            }
        }
    }

    //

    private sealed class SimpleHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new() { Timeout = Timeout.InfiniteTimeSpan };
    }
}