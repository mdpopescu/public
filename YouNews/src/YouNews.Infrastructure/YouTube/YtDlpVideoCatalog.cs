using System.Globalization;
using System.Text.Json;
using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Infrastructure.Processes;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Finds channels and videos by running yt-dlp.</summary>
public sealed class YtDlpVideoCatalog : IVideoCatalog
{
    public YtDlpVideoCatalog(YtDlpRunner ytDlp, VideoErrorClassifier classifier, ChannelAddressParser addressParser)
    {
        this.ytDlp = ytDlp;
        this.classifier = classifier;
        this.addressParser = addressParser;
    }

    public async Task<ChannelDetails?> ResolveChannelAsync(string urlOrHandle, CancellationToken cancellationToken)
    {
        var url = addressParser.ToChannelUrl(urlOrHandle);
        if (url == null)
            return null;

        var result = await ytDlp.RunAsync(["--flat-playlist", "--playlist-end", "1", "--dump-single-json", "--skip-download", url], cancellationToken).ConfigureAwait(false);
        if (result.ExitCode != 0)
            return IsMissingChannel(result.StandardError)
                ? null
                : throw new InvalidOperationException($"yt-dlp could not look up {url}: {result.StandardError.Trim()}");

        return ReadChannel(result.StandardOutput, url);
    }

    public async Task<IReadOnlyList<IReadOnlyList<FeedVideo>>> ListVideoFeedsAsync(Channel channel, CancellationToken cancellationToken)
    {
        var videos = await ListTabAsync(channel, "videos", cancellationToken).ConfigureAwait(false);
        var streams = await ListTabAsync(channel, "streams", cancellationToken).ConfigureAwait(false);

        return [videos ?? throw new InvalidOperationException($"Could not list the videos of {channel.Name}."), streams ?? []];
    }

    public async Task<VideoDetails> GetDetailsAsync(string youTubeId, CancellationToken cancellationToken)
    {
        var url = ytDlp.WatchUrl(youTubeId);
        ProcessResult result;
        try
        {
            result = await ytDlp.RunAsync(["--skip-download", "--dump-json", url], cancellationToken).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            return new VideoDetails(youTubeId, VideoAvailability.RecheckLater, null, "yt-dlp timed out.");
        }

        if (result.ExitCode != 0)
        {
            if (YtDlpRunner.IsRateLimited(result))
                throw new RateLimitedException($"yt-dlp could not get the details of {youTubeId}: {result.StandardError.Trim()}");

            var (availability, reason) = classifier.Classify(result.StandardError, result.ExitCode);
            return new VideoDetails(youTubeId, availability, null, reason);
        }

        return ReadDetails(youTubeId, result.StandardOutput);
    }

    //

    private readonly YtDlpRunner ytDlp;
    private readonly VideoErrorClassifier classifier;
    private readonly ChannelAddressParser addressParser;

    private static bool IsMissingChannel(string standardError) =>
        standardError.Contains("does not exist", StringComparison.OrdinalIgnoreCase)
        || standardError.Contains("HTTP Error 404", StringComparison.OrdinalIgnoreCase);

    private static ChannelDetails? ReadChannel(string json, string requestedUrl)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var id = ReadString(root, "channel_id") ?? ReadString(root, "id");
        var name = ReadString(root, "channel") ?? ReadString(root, "uploader") ?? ReadString(root, "title");
        if (id == null || name == null)
            return null;

        var url = ReadString(root, "channel_url") ?? ReadString(root, "uploader_url") ?? requestedUrl;
        return new ChannelDetails(id, url, name);
    }

    private static VideoDetails ReadDetails(string youTubeId, string output)
    {
        var json = output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault() ?? "{}";
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var title = ReadString(root, "title") ?? youTubeId;

        return ByLiveStatus(root, title);
    }

    private static VideoDetails ByLiveStatus(JsonElement root, string title)
    {
        var publishedOn = ReadPublishedOn(root);
        var liveStatus = ReadString(root, "live_status");
        return liveStatus is null or "not_live" or "was_live"
            ? new VideoDetails(title, VideoAvailability.Eligible, publishedOn, null)
            : new VideoDetails(title, VideoAvailability.RecheckLater, publishedOn, $"The video is not finished yet (live status: {liveStatus}).");
    }

    private static FeedVideo ReadFeedVideo(string line)
    {
        using var document = JsonDocument.Parse(line);
        var root = document.RootElement;
        var id = ReadString(root, "id") ?? throw new InvalidOperationException("yt-dlp listed a video without an ID.");
        var title = ReadString(root, "title") ?? id;
        var availability = ReadString(root, "availability");
        var details = availability is "subscriber_only" or "premium_only" or "needs_auth" or "private"
            ? new VideoDetails(title, VideoAvailability.PermanentlyUnavailable, ReadPublishedOn(root), $"YouTube does not show this video to everyone ({availability}).")
            : ByLiveStatus(root, title);

        return new FeedVideo(id, details);
    }

    private static DateOnly? ReadPublishedOn(JsonElement root)
    {
        var uploadDate = ReadString(root, "upload_date");
        if (uploadDate != null && DateOnly.TryParseExact(uploadDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        if (root.TryGetProperty("timestamp", out var timestamp) && timestamp.ValueKind == JsonValueKind.Number && timestamp.TryGetInt64(out var seconds))
            return DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime);

        return null;
    }

    private static string? ReadString(JsonElement root, string name) =>
        root.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private async Task<IReadOnlyList<FeedVideo>?> ListTabAsync(Channel channel, string tab, CancellationToken cancellationToken)
    {
        var url = $"{channel.Url.TrimEnd('/')}/{tab}";
        var result = await ytDlp.RunAsync(["--flat-playlist", "--playlist-end", "200", "--extractor-args", "youtubetab:approximate_date", "--dump-json", url], cancellationToken).ConfigureAwait(false);

        if (YtDlpRunner.IsRateLimited(result))
            throw new RateLimitedException($"yt-dlp could not list {url}: {result.StandardError.Trim()}");

        return result.ExitCode != 0
            ? null
            : result.StandardOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(ReadFeedVideo).ToList();
    }
}