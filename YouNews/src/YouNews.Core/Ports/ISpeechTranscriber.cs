using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Produces a transcript from a video's audio, for videos without captions.</summary>
public interface ISpeechTranscriber
{
    /// <summary>Transcribes a video. Throws when transcription is not possible.</summary>
    Task<Transcript> TranscribeAsync(string youTubeId, CancellationToken cancellationToken);
}