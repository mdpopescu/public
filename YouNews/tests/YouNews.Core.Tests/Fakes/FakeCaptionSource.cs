using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeCaptionSource : ICaptionSource
{
    public Transcript? Result { get; set; } = new([new TranscriptSegment(TimeSpan.Zero, "words from captions")]);

    public Task<Transcript?> GetCaptionsAsync(string youTubeId, CancellationToken cancellationToken) => Task.FromResult(Result);
}