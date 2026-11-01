namespace YouNews.Core.Ports;

/// <summary>Checks that the external tools the application needs are usable.</summary>
public interface IToolHealth
{
    /// <summary>Lists problems the user should know about, such as a yt-dlp version that is too old.</summary>
    Task<IReadOnlyList<string>> FindProblemsAsync(CancellationToken cancellationToken);
}