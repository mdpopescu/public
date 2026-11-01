namespace YouNews.Web.Hosting;

/// <summary>When refreshes run, bound from the "YouNews:Refresh" configuration section.</summary>
public sealed class RefreshOptions
{
    public const string SECTION_NAME = "YouNews:Refresh";

    public TimeOnly DailyAt { get; set; } = new(6, 0);

    public int? MaximumVideosPerRun { get; set; }
}