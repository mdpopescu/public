using System.Globalization;
using System.Text;
using YouNews.Core.Model;

namespace YouNews.Core.Briefing;

/// <summary>Writes a transcript as text. All words stay, but timestamps appear only about every 30 seconds to save tokens.</summary>
public sealed class TranscriptFormatter
{
    public string Format(Transcript transcript)
    {
        var text = new StringBuilder();
        TimeSpan? lastMarker = null;
        foreach (var segment in transcript.Segments)
        {
            if (lastMarker is null || segment.Start - lastMarker >= MARKER_INTERVAL)
            {
                text.Append(text.Length == 0 ? "" : "\n").Append(Marker(segment.Start));
                lastMarker = segment.Start;
            }

            text.Append(' ').Append(string.Join(' ', segment.Text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)));
        }

        return text.ToString();
    }

    //

    private static readonly TimeSpan MARKER_INTERVAL = TimeSpan.FromSeconds(30);

    private static string Marker(TimeSpan time) =>
        time.TotalHours >= 1
            ? string.Create(CultureInfo.InvariantCulture, $"[{(int)time.TotalHours}:{time.Minutes:00}:{time.Seconds:00}]")
            : string.Create(CultureInfo.InvariantCulture, $"[{(int)time.TotalMinutes:00}:{time.Seconds:00}]");
}