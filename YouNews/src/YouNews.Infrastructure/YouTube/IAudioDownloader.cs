namespace YouNews.Infrastructure.YouTube;

/// <summary>Downloads the audio track of a video, for speech transcription.</summary>
public interface IAudioDownloader
{
    /// <summary>
    /// Downloads a low-bitrate audio track into the work directory and returns the path of the file.
    /// The caller owns the file and must delete it when done.
    /// </summary>
    /// <exception cref="InvalidOperationException">The download failed.</exception>
    Task<string> DownloadAudioAsync(string youTubeId, CancellationToken cancellationToken);
}