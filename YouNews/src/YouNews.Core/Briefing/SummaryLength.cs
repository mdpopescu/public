namespace YouNews.Core.Briefing;

/// <summary>Keeps summaries proportional to their videos, so that a 3-minute video and a 2-hour one differ.</summary>
public sealed class SummaryLength
{
    public const int TARGET_PERCENT = 8;
    public const int MINIMUM_WORDS = 80;
    public const int MAXIMUM_WORDS = 1200;

    public int TargetWords(int transcriptWords) =>
        Math.Clamp((int)Math.Round(transcriptWords * (TARGET_PERCENT / 100.0)), MINIMUM_WORDS, MAXIMUM_WORDS);
}