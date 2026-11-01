using System.Globalization;
using System.Text.RegularExpressions;
using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

public sealed partial class EntryPresenter : IEntryPresenter
{
    public EntryPresenter(ParagraphSplitter splitter)
    {
        this.splitter = splitter;
    }

    public EntryView Present(Entry entry)
    {
        var tags = entry.Tags.Select(PresentTag).ToList();
        return new EntryView(
            entry.Id,
            $"entry-{entry.Id}",
            PageUrl(entry.Day, entry.Id),
            entry.Headline,
            entry.ChannelName,
            $"https://www.youtube.com/watch?v={Uri.EscapeDataString(entry.VideoYouTubeId)}",
            tags,
            splitter.Split(entry.Summary),
            splitter.Split(entry.Opinion),
            entry.WebSources.Where(IsWebAddress).Select(source => new SourceView(SourceTitle(source), source.Url)).ToList(),
            ImageUrl(entry.Images.FirstOrDefault(image => image.Kind == ImageKind.Thumbnail)),
            entry.Images.Where(image => image.Kind == ImageKind.Frame).Select(image => ImageUrl(image)!).ToList(),
            entry.Related.Select(PresentRelated).ToList(),
            entry.Tags.Contains(EntryTags.OFF_TOPIC),
            tags.Count > 0 ? tags[0].CssClass : "tag-none",
            entry.IsReprocessQueued,
            entry.IsReprocessQueued ? "Queued" : "Reprocess",
            entry.ReprocessFailure is null ? null : $"Reprocess failed: {entry.ReprocessFailure}");
    }

    //

    private readonly ParagraphSplitter splitter;

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonAlphanumeric();

    private static TagView PresentTag(string tag)
    {
        var text = tag.Length == 0 ? tag : char.ToUpperInvariant(tag[0]) + tag[1..];
        return new TagView(text, "tag-" + NonAlphanumeric().Replace(tag.ToLowerInvariant(), "-").Trim('-'));
    }

    private static bool IsWebAddress(WebSource source) =>
        Uri.TryCreate(source.Url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    private static string SourceTitle(WebSource source) =>
        string.IsNullOrWhiteSpace(source.Title) ? new Uri(source.Url).Host : source.Title;

    private static string? ImageUrl(EntryImage? image) =>
        image is null ? null : WebBootstrapper.IMAGES_PATH + "/" + string.Join('/', image.RelativePath.Split('/').Select(Uri.EscapeDataString));

    private static string PageUrl(DateOnly day, long entryId) => $"/day/{day:yyyy-MM-dd}#entry-{entryId}";

    private static RelatedView PresentRelated(RelatedLink link) =>
        new(PageUrl(link.Day, link.EntryId), $"{link.Day.ToString("MMM d", CultureInfo.InvariantCulture)}: {link.Headline}");
}