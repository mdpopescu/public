using Microsoft.Extensions.Options;
using YouNews.Infrastructure.Processes;

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

        ProcessResult result;
        try
        {
            result = await ytDlp.RunAsync(["-f", "ba[abr<=64]/ba", "-o", Path.Combine(workDirectory, $"{prefix}.%(ext)s"), url], cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            DeleteFiles(prefix);
            throw;
        }

        var audio = Directory.GetFiles(workDirectory, $"{prefix}.*").FirstOrDefault(file => !file.EndsWith(".part", StringComparison.Ordinal));
        if (result.ExitCode == 0 && audio != null)
            return audio;

        DeleteFiles(prefix);
        throw new InvalidOperationException($"yt-dlp could not download the audio of {youTubeId}: {result.StandardError.Trim()}");
    }

    //

    private readonly YtDlpRunner ytDlp;
    private readonly string workDirectory;

    private void DeleteFiles(string prefix)
    {
        foreach (var file in Directory.GetFiles(workDirectory, $"{prefix}.*"))
            File.Delete(file);
    }
}