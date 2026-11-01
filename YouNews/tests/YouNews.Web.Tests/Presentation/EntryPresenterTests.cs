using YouNews.Core.Model;
using YouNews.Web.Presentation;

namespace YouNews.Web.Tests.Presentation;

[TestClass]
public sealed class EntryPresenterTests
{
    [TestInitialize]
    public void Initialize()
    {
        sut = new EntryPresenter(new ParagraphSplitter());
    }

    [TestMethod]
    public void Present_BuildsAnchorVideoLinkAndParagraphs()
    {
        var view = sut.Present(MakeEntry(summary: "One.\n\nTwo.", opinion: "Mine."));

        Assert.AreEqual("entry-42", view.AnchorId);
        Assert.AreEqual("https://www.youtube.com/watch?v=abc123XYZ_-", view.VideoUrl);
        Assert.HasCount(2, view.SummaryParagraphs);
        CollectionAssert.AreEqual(new[] { "Mine." }, view.OpinionParagraphs.ToArray());
    }

    [TestMethod]
    public void Present_Tags_GetDisplayTextAndCssClass()
    {
        var view = sut.Present(MakeEntry(tags: [EntryTags.HARDWARE_MARKET, EntryTags.LOCAL_MODELS]));

        Assert.AreEqual("Hardware market", view.Tags[0].Text);
        Assert.AreEqual("tag-hardware-market", view.Tags[0].CssClass);
        Assert.AreEqual("tag-hardware-market", view.ArtClass, "the picture placeholder follows the first topic");
        Assert.IsFalse(view.IsOffTopic);
    }

    [TestMethod]
    public void Present_OffTopicTag_IsFlagged()
    {
        var view = sut.Present(MakeEntry(tags: [EntryTags.OFF_TOPIC]));

        Assert.IsTrue(view.IsOffTopic);
        Assert.AreEqual("tag-off-topic", view.Tags[0].CssClass);
    }

    [TestMethod]
    public void Present_NoTags_UsesNeutralArtClass()
    {
        Assert.AreEqual("tag-none", sut.Present(MakeEntry(tags: [])).ArtClass);
    }

    [TestMethod]
    public void Present_Images_UseImagesPathThumbnailFirstAndFramesAfter()
    {
        var images = new[]
        {
            new EntryImage("2026/11/frame 1.jpg", ImageKind.Frame),
            new EntryImage("2026/11/abc.jpg", ImageKind.Thumbnail),
            new EntryImage("2026/11/frame2.jpg", ImageKind.Frame),
        };

        var view = sut.Present(MakeEntry(images: images));

        Assert.AreEqual("/images/2026/11/abc.jpg", view.MainImageUrl);
        CollectionAssert.AreEqual(new[] { "/images/2026/11/frame%201.jpg", "/images/2026/11/frame2.jpg" }, view.FrameUrls.ToArray());
    }

    [TestMethod]
    public void Present_NoImages_HasNoMainImage()
    {
        var view = sut.Present(MakeEntry(images: []));

        Assert.IsNull(view.MainImageUrl);
        Assert.IsEmpty(view.FrameUrls);
    }

    [TestMethod]
    public void Present_Related_PointsToTheDayAnchor()
    {
        var related = new[] { new RelatedLink(7, new DateOnly(2026, 10, 28), "The previous version") };

        var view = sut.Present(MakeEntry(related: related));

        Assert.AreEqual("/day/2026-10-28#entry-7", view.Related[0].Url);
        Assert.AreEqual("Oct 28: The previous version", view.Related[0].Text);
    }

    [TestMethod]
    public void Present_SourcesWithUnsafeAddresses_AreDropped()
    {
        var sources = new[]
        {
            new WebSource("Good", "https://example.com/a"),
            new WebSource("Evil", "javascript:alert(1)"),
            new WebSource("Relative", "/etc/passwd"),
            new WebSource("", "http://example.org/x"),
        };

        var view = sut.Present(MakeEntry(sources: sources));

        CollectionAssert.AreEqual(new[] { "Good", "example.org" }, view.Sources.Select(source => source.Title).ToArray());
    }

    //

    private EntryPresenter sut = null!;

    private static Entry MakeEntry(
        string summary = "Summary.",
        string opinion = "Opinion.",
        string[]? tags = null,
        EntryImage[]? images = null,
        RelatedLink[]? related = null,
        WebSource[]? sources = null) =>
        new(42, new DateOnly(2026, 11, 1), "Sample Channel", "abc123XYZ_-", "Headline", tags ?? [EntryTags.LOCAL_MODELS], summary, opinion, sources ?? [], images ?? [], related ?? []);
}