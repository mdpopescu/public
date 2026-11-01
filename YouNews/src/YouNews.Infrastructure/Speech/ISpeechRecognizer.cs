namespace YouNews.Infrastructure.Speech;

/// <summary>A stretch of speech found in the audio. The text is empty when nothing could be recognized.</summary>
public sealed record SpokenSegment(TimeSpan Start, string Text);

/// <summary>Finds the speech in an audio file and recognizes it.</summary>
public interface ISpeechRecognizer
{
    /// <summary>Recognizes every voiced stretch of a 16 kHz mono 16-bit WAV file, in order of time.</summary>
    /// <exception cref="OperationCanceledException">Cancelled between two segments.</exception>
    Task<IReadOnlyList<SpokenSegment>> RecognizeAsync(string wavPath, CancellationToken cancellationToken);
}