using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeCaptionSource : ICaptionSource
{
    public Transcript? Result { get; set; } = new([new TranscriptSegment(TimeSpan.Zero, "words from captions")]);

    /// <summary>Videos whose captions YouTube refuses to give because of its rate limit.</summary>
    public HashSet<string> RateLimitedIds { get; } = [];

    public Task<Transcript?> GetCaptionsAsync(string youTubeId, CancellationToken cancellationToken) =>
        RateLimitedIds.Contains(youTubeId)
            ? throw new RateLimitedException($"HTTP Error 429: Too Many Requests for {youTubeId}")
            : Task.FromResult(Result);
}