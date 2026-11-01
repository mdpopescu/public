using System.Net;
using Microsoft.Extensions.Options;
using YouNews.Core.Model;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Downloads a video's thumbnail from YouTube's image server.</summary>
public sealed class ThumbnailDownloader
{
    public const string HTTP_CLIENT_NAME = "YouTubeImages";

    public ThumbnailDownloader(IHttpClientFactory httpClientFactory, IOptions<YouNewsOptions> options)
    {
        this.httpClientFactory = httpClientFactory;
        imagesDirectory = options.Value.ImagesDirectory;
    }

    /// <summary>Stores the best available thumbnail. Returns null when YouTube has none or cannot be reached.</summary>
    public async Task<EntryImage?> DownloadAsync(string youTubeId, CancellationToken cancellationToken)
    {
        using var client = httpClientFactory.CreateClient(HTTP_CLIENT_NAME);
        try
        {
            var bytes = await GetBytesAsync(client, youTubeId, "maxresdefault", cancellationToken).ConfigureAwait(false)
                ?? await GetBytesAsync(client, youTubeId, "hqdefault", cancellationToken).ConfigureAwait(false);
            return bytes == null ? null : await StoreAsync(youTubeId, bytes, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    //

    private readonly IHttpClientFactory httpClientFactory;
    private readonly string imagesDirectory;

    private static async Task<byte[]?> GetBytesAsync(HttpClient client, string youTubeId, string name, CancellationToken cancellationToken)
    {
        using var response = await client.GetAsync($"https://i.ytimg.com/vi/{youTubeId}/{name}.jpg", cancellationToken).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<EntryImage> StoreAsync(string youTubeId, byte[] bytes, CancellationToken cancellationToken)
    {
        var folder = Path.Combine(imagesDirectory, youTubeId);
        Directory.CreateDirectory(folder);
        await File.WriteAllBytesAsync(Path.Combine(folder, "thumbnail.jpg"), bytes, cancellationToken).ConfigureAwait(false);
        return new EntryImage($"{youTubeId}/thumbnail.jpg", ImageKind.Thumbnail);
    }
}