using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Captures pictures for an entry.</summary>
public interface IImageSource
{
    /// <summary>
    /// Stores the video's thumbnail and a frame per timestamp. Images that cannot be captured are
    /// left out rather than raising an error, so the result may be empty.
    /// </summary>
    Task<IReadOnlyList<EntryImage>> CaptureAsync(string youTubeId, IReadOnlyList<TimeSpan> frameTimes, CancellationToken cancellationToken);
}