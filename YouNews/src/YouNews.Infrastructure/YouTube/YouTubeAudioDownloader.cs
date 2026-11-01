using Microsoft.Extensions.Options;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Downloads a small audio track with yt-dlp into the work directory.</summary>
public sealed class YouTubeAudioDownloader : IAudioDownloader
{
    public YouTubeAudioDownloader(YtDlpRunner ytDlp, IOptions<YouNewsOptions> options)
    {
        this.ytDlp = ytDlp;
        workDirectory = options.Value.WorkDirectory;
    }

    public async Task<string> DownloadAudioAsync(string youTubeId, CancellationToken cancellationToken)
    {
        var url = ytDlp.WatchUrl(youTubeId);
        Directory.CreateDirectory(workDirectory);
        var prefix = $"{youTubeId}-{Guid.NewGuid():N}";

        var result = await ytDlp.RunAsync(["-f", "ba[abr<=64]/ba", "-o", Path.Combine(workDirectory, $"{prefix}.%(ext)s"), url], cancellationToken).ConfigureAwait(false);

        var files = Directory.GetFiles(workDirectory, $"{prefix}.*");
        var audio = files.FirstOrDefault(file => !file.EndsWith(".part", StringComparison.Ordinal));
        if (result.ExitCode == 0 && audio != null)
            return audio;

        foreach (var file in files)
            File.Delete(file);
        throw new InvalidOperationException($"yt-dlp could not download the audio of {youTubeId}: {result.StandardError.Trim()}");
    }

    //

    private readonly YtDlpRunner ytDlp;
    private readonly string workDirectory;
}