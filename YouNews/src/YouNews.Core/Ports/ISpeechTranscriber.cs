using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Produces a transcript from a video's audio, for videos without captions.</summary>
public interface ISpeechTranscriber
{
    /// <summary>
    ///     Transcribes a video. Throws when transcription is not possible,
    ///     <see cref="RateLimitedException" /> when YouTube refuses the download because of too many requests.
    /// </summary>
    Task<Transcript> TranscribeAsync(string youTubeId, CancellationToken cancellationToken);
}