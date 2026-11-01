using System.ComponentModel;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using YouNews.Core.Ports;
using YouNews.Infrastructure.Processes;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Checks that yt-dlp is recent enough and that ffmpeg and node can be started.</summary>
public sealed partial class ToolHealth : IToolHealth
{
    public ToolHealth(IProcessRunner processRunner, IOptions<YouNewsOptions> options)
    {
        this.processRunner = processRunner;
        tools = options.Value.Tools;
    }

    public async Task<IReadOnlyList<string>> FindProblemsAsync(CancellationToken cancellationToken)
    {
        var problems = new List<string>();
        AddIfNotNull(problems, await CheckYtDlpAsync(cancellationToken).ConfigureAwait(false));
        AddIfNotNull(problems, await CheckStartsAsync("ffmpeg", tools.FfmpegPath, "-version", "Install ffmpeg (for example `winget install Gyan.FFmpeg`) or set YouNews:Tools:FfmpegPath.", cancellationToken).ConfigureAwait(false));
        AddIfNotNull(problems, await CheckStartsAsync("node", tools.NodePath, "--version", "Install Node.js (for example `winget install OpenJS.NodeJS.LTS`) or set YouNews:Tools:NodePath.", cancellationToken).ConfigureAwait(false));
        return problems;
    }

    //

    private readonly IProcessRunner processRunner;
    private readonly ToolOptions tools;

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberPattern();

    private static void AddIfNotNull(List<string> problems, string? problem)
    {
        if (problem != null)
            problems.Add(problem);
    }

    private static int[] ParseVersion(string text) =>
        NumberPattern().Matches(text).Select(match => int.Parse(match.Value)).ToArray();

    private static bool IsOlder(int[] actual, int[] minimum)
    {
        for (var i = 0; i < Math.Max(actual.Length, minimum.Length); i++)
        {
            var difference = (i < actual.Length ? actual[i] : 0) - (i < minimum.Length ? minimum[i] : 0);
            if (difference != 0)
                return difference < 0;
        }

        return false;
    }

    private async Task<string?> CheckYtDlpAsync(CancellationToken cancellationToken)
    {
        var upgrade = "Upgrade it with `pip install --upgrade \"yt-dlp[default]\"` or set YouNews:Tools:YtDlpPath.";
        var result = await TryRunAsync(tools.YtDlpPath, "--version", cancellationToken).ConfigureAwait(false);
        if (result == null)
            return $"yt-dlp could not be started from \"{tools.YtDlpPath}\". Install it with `pip install --upgrade \"yt-dlp[default]\"` or set YouNews:Tools:YtDlpPath.";

        var installed = result.StandardOutput.Trim();
        if (result.ExitCode != 0 || ParseVersion(installed).Length == 0)
            return $"yt-dlp did not report a version (exit code {result.ExitCode}). {upgrade}";

        return IsOlder(ParseVersion(installed), ParseVersion(tools.MinimumYtDlpVersion))
            ? $"yt-dlp {installed} is older than {tools.MinimumYtDlpVersion}, so video downloads for images may fail. {upgrade}"
            : null;
    }

    private async Task<string?> CheckStartsAsync(string name, string path, string argument, string fix, CancellationToken cancellationToken)
    {
        var result = await TryRunAsync(path, argument, cancellationToken).ConfigureAwait(false);
        return result is { ExitCode: 0 } ? null : $"{name} could not be started from \"{path}\". {fix}";
    }

    private async Task<ProcessResult?> TryRunAsync(string path, string argument, CancellationToken cancellationToken)
    {
        try
        {
            return await processRunner.RunAsync(path, [argument], null, TimeSpan.FromMinutes(tools.TimeoutMinutes), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is Win32Exception or TimeoutException)
        {
            return null;
        }
    }
}