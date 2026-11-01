using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Gets the captions published with a video.</summary>
public interface ICaptionSource
{
    /// <summary>Gets English captions, including automatic ones. Returns null when the video has none.</summary>
    Task<Transcript?> GetCaptionsAsync(string youTubeId, CancellationToken cancellationToken);
}