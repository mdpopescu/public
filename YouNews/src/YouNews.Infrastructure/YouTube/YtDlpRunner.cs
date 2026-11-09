using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using YouNews.Infrastructure.Processes;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Runs yt-dlp with the flags every call needs.</summary>
public sealed partial class YtDlpRunner
{
    public YtDlpRunner(IProcessRunner processRunner, IOptions<YouNewsOptions> options)
    {
        this.processRunner = processRunner;
        tools = options.Value.Tools;
    }

    /// <summary>Tells whether yt-dlp failed because YouTube refused the request for being one too many.</summary>
    public static bool IsRateLimited(ProcessResult result) =>
        result.ExitCode != 0 && result.StandardError.Contains("HTTP Error 429", StringComparison.Ordinal);

    /// <summary>Gets the watch page address of a video, rejecting identifiers that are not safe to use in paths.</summary>
    public string WatchUrl(string youTubeId)
    {
        if (!VideoIdPattern().IsMatch(youTubeId))
            throw new ArgumentException($"\"{youTubeId}\" is not a valid YouTube video ID.", nameof(youTubeId));

        return $"https://www.youtube.com/watch?v={youTubeId}";
    }

    public Task<ProcessResult> RunAsync(IEnumerable<string> arguments, CancellationToken cancellationToken)
    {
        List<string> all = ["--no-update", "--js-runtimes", $"node:{tools.NodePath}", "--no-warnings"];
        all.AddRange(arguments);

        return processRunner.RunAsync(tools.YtDlpPath, all, null, TimeSpan.FromMinutes(tools.TimeoutMinutes), cancellationToken);
    }

    //

    private readonly IProcessRunner processRunner;
    private readonly ToolOptions tools;

    [GeneratedRegex("^[A-Za-z0-9_-]{1,64}$")]
    private static partial Regex VideoIdPattern();
}