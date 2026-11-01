using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeSpeechTranscriber : ISpeechTranscriber
{
    public Transcript? Result { get; set; }

    public Task<Transcript> TranscribeAsync(string youTubeId, CancellationToken cancellationToken) =>
        Result is null ? throw new InvalidOperationException("speech unavailable") : Task.FromResult(Result);
}