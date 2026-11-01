namespace YouNews.Infrastructure.Speech;

/// <summary>The files the recognizer needs, all present on disk.</summary>
public sealed record SpeechModelFiles(string Encoder, string Decoder, string Joiner, string Tokens, string VoiceDetector);

/// <summary>Keeps the speech models on disk, downloading them on first use.</summary>
public interface ISpeechModelStore
{
    /// <summary>
    /// Returns the model files, downloading and unpacking them first when they are not there yet.
    /// A download that was interrupted is not treated as complete and is repeated.
    /// </summary>
    /// <exception cref="InvalidOperationException">The download or unpacking failed.</exception>
    Task<SpeechModelFiles> EnsureModelsAsync(CancellationToken cancellationToken);
}