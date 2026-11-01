using YouNews.Core.Briefing;

namespace YouNews.Core.Tests.Briefing;

[TestClass]
public sealed class PlainTextCleanerTests
{
    [TestMethod]
    [DataRow("The **claims** are **overblown**.", "The claims are overblown.")]
    [DataRow("# Verdict\n\nOverblown.", "Verdict\n\nOverblown.")]
    [DataRow("### Verdict\nOverblown.", "Verdict\nOverblown.")]
    [DataRow("- first\n- second", "first\nsecond")]
    [DataRow("  * first\n  * second", "first\nsecond")]
    [DataRow("**# Verdict**", "Verdict")]
    [DataRow("one\r\n\r\ntwo", "one\n\ntwo")]
    [DataRow("one\n\n\n\ntwo\n", "one\n\ntwo")]
    [DataRow("one   \n  \ntwo", "one\n\ntwo")]
    public void RemovesMarkdownMarkers(string text, string expected)
    {
        var sut = new PlainTextCleaner();

        Assert.AreEqual(expected, sut.Clean(text));
    }

    [TestMethod]
    [DataRow("The #1 model costs 3 - 4 dollars per hour.")]
    [DataRow("#1 on the leaderboard is not the same as useful.")]
    [DataRow("It needs 16 GB * 2 cards, and well-known tools.")]
    [DataRow("-5% is what the independent test found.")]
    public void LeavesOrdinaryTextAlone(string text)
    {
        var sut = new PlainTextCleaner();

        Assert.AreEqual(text, sut.Clean(text));
    }
}