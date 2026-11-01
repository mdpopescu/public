using System.Globalization;

namespace YouNews.Infrastructure.Storage;

/// <summary>Converts values to and from the text the database keeps, so no global type handlers are needed.</summary>
internal sealed class StorageFormat
{
    private const string DAY_FORMAT = "yyyy-MM-dd";

    public static string ToText(DateOnly day) => day.ToString(DAY_FORMAT, CultureInfo.InvariantCulture);

    public static string ToText(DateTimeOffset moment) => moment.ToString("O", CultureInfo.InvariantCulture);

    public static DateOnly ParseDay(string text) => DateOnly.ParseExact(text, DAY_FORMAT, CultureInfo.InvariantCulture);

    public static DateTimeOffset ParseMoment(string text) =>
        DateTimeOffset.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
}