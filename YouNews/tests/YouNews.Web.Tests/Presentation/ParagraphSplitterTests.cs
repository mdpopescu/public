using YouNews.Web.Presentation;

namespace YouNews.Web.Tests.Presentation;

[TestClass]
public sealed class ParagraphSplitterTests
{
    [TestMethod]
    public void Split_BlankLineSeparated_GivesOneParagraphEach()
    {
        var result = sut.Split("First thought.\r\n\r\nSecond thought.\n\n\n  Third thought.  ");

        CollectionAssert.AreEqual(new[] { "First thought.", "Second thought.", "Third thought." }, result.ToArray());
    }

    [TestMethod]
    public void Split_OnlySingleNewlines_GivesOneParagraphPerLine()
    {
        var result = sut.Split("One line.\nAnother line.");

        CollectionAssert.AreEqual(new[] { "One line.", "Another line." }, result.ToArray());
    }

    [TestMethod]
    public void Split_EmptyOrBlank_GivesNothing()
    {
        Assert.IsEmpty(sut.Split("  \n\n "), "blank text has no paragraphs");
    }

    [TestMethod]
    public void Split_LongBlockWithoutBreaks_IsCutAtSentencesAndLosesNothing()
    {
        var sentence = "The presenter walks through the benchmark results in detail. ";
        var text = string.Concat(Enumerable.Repeat(sentence, 20)).Trim();

        var result = sut.Split(text);

        Assert.IsGreaterThan(1, result.Count, "a 1200 character block should become several paragraphs");
        Assert.IsTrue(result.All(paragraph => paragraph.Length <= 520), "paragraphs should stay short: " + string.Join(" | ", result.Select(p => p.Length)));
        Assert.AreEqual(text, string.Join(" ", result), "no text may be lost or changed");
    }

    [TestMethod]
    public void Split_ShortParagraph_IsLeftAlone()
    {
        var text = string.Concat(Enumerable.Repeat("Short sentence here. ", 20)).Trim();

        var result = sut.Split(text);

        Assert.HasCount(1, result, "400 characters is not a wall of text");
    }

    //

    private readonly ParagraphSplitter sut = new();
}