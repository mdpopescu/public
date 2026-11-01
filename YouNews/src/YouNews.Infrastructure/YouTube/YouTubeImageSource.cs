using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Gets the thumbnail and requested frames of a YouTube video.</summary>
public sealed class YouTubeImageSource : IImageSource
{
    public YouTubeImageSource(ThumbnailDownloader thumbnails, FrameExtractor frames)
    {
        this.thumbnails = thumbnails;
        this.frames = frames;
    }

    public async Task<IReadOnlyList<EntryImage>> CaptureAsync(string youTubeId, IReadOnlyList<TimeSpan> frameTimes, CancellationToken cancellationToken)
    {
        var images = new List<EntryImage>();

        var thumbnail = await thumbnails.DownloadAsync(youTubeId, cancellationToken).ConfigureAwait(false);
        if (thumbnail != null)
            images.Add(thumbnail);

        images.AddRange(await frames.ExtractAsync(youTubeId, frameTimes, cancellationToken).ConfigureAwait(false));
        return images;
    }

    //

    private readonly ThumbnailDownloader thumbnails;
    private readonly FrameExtractor frames;
}