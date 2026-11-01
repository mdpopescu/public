using System.Globalization;
using Microsoft.Extensions.Options;
using YouNews.Core.Model;
using YouNews.Infrastructure.Processes;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Downloads a video at low resolution and cuts single frames out of it with ffmpeg.</summary>
public sealed class FrameExtractor
{
    public FrameExtractor(YtDlpRunner ytDlp, IProcessRunner processRunner, IOptions<YouNewsOptions> options)
    {
        this.ytDlp = ytDlp;
        this.processRunner = processRunner;
        this.options = options.Value;
    }

    /// <summary>Extracts a frame per time. Frames that cannot be produced are left out; a failed download gives an empty list.</summary>
    public async Task<IReadOnlyList<EntryImage>> ExtractAsync(string youTubeId, IReadOnlyList<TimeSpan> times, CancellationToken cancellationToken)
    {
        if (times.Count == 0)
            return [];

        var url = ytDlp.WatchUrl(youTubeId);
        using var folder = new ScratchFolder(options.WorkDirectory);
        try
        {
            var video = await DownloadVideoAsync(url, folder.Path, cancellationToken).ConfigureAwait(false);
            return video == null ? [] : await CutFramesAsync(youTubeId, video, times, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return [];
        }
    }

    //

    private readonly YtDlpRunner ytDlp;
    private readonly IProcessRunner processRunner;
    private readonly YouNewsOptions options;

    private async Task<string?> DownloadVideoAsync(string url, string folder, CancellationToken cancellationToken)
    {
        var template = Path.Combine(folder, "video.%(ext)s");
        var result = await ytDlp.RunAsync(["-f", "bv*[height<=480][ext=mp4]/b[height<=480]", "-o", template, url], cancellationToken).ConfigureAwait(false);

        return result.ExitCode != 0
            ? null
            : Directory.GetFiles(folder, "video.*").FirstOrDefault(file => !file.EndsWith(".part", StringComparison.Ordinal));
    }

    private async Task<IReadOnlyList<EntryImage>> CutFramesAsync(string youTubeId, string video, IReadOnlyList<TimeSpan> times, CancellationToken cancellationToken)
    {
        var folder = Path.Combine(options.ImagesDirectory, youTubeId);
        Directory.CreateDirectory(folder);

        var images = new List<EntryImage>();
        foreach (var time in times)
        {
            var name = $"frame-{(int)time.TotalSeconds}.jpg";
            if (await CutFrameAsync(video, time, Path.Combine(folder, name), cancellationToken).ConfigureAwait(false))
                images.Add(new EntryImage($"{youTubeId}/{name}", ImageKind.Frame));
        }

        return images;
    }

    private async Task<bool> CutFrameAsync(string video, TimeSpan time, string output, CancellationToken cancellationToken)
    {
        var seconds = time.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        var timeout = TimeSpan.FromMinutes(options.Tools.TimeoutMinutes);
        await processRunner.RunAsync(options.Tools.FfmpegPath, ["-y", "-loglevel", "error", "-ss", seconds, "-i", video, "-frames:v", "1", "-q:v", "3", output], null, timeout, cancellationToken).ConfigureAwait(false);

        if (File.Exists(output) && new FileInfo(output).Length > 0)
            return true;

        File.Delete(output);
        return false;
    }
}