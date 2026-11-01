using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.YouTube;

[TestClass]
public sealed class ChannelAddressParserTests
{
    [TestMethod]
    [DataRow("@matthew_berman", "https://www.youtube.com/@matthew_berman")]
    [DataRow("  @matthew_berman ", "https://www.youtube.com/@matthew_berman")]
    [DataRow("matthew_berman", "https://www.youtube.com/@matthew_berman")]
    [DataRow("https://www.youtube.com/@matthew_berman/", "https://www.youtube.com/@matthew_berman")]
    [DataRow("youtube.com/@matthew_berman", "https://www.youtube.com/@matthew_berman")]
    [DataRow("https://m.youtube.com/@matthew_berman", "https://www.youtube.com/@matthew_berman")]
    [DataRow("https://www.youtube.com/channel/UCawZsQWqfGSbCI5yjkdVkTA", "https://www.youtube.com/channel/UCawZsQWqfGSbCI5yjkdVkTA")]
    [DataRow("UCawZsQWqfGSbCI5yjkdVkTA", "https://www.youtube.com/channel/UCawZsQWqfGSbCI5yjkdVkTA")]
    public void AcceptsYouTubeChannelForms(string text, string expected)
    {
        var sut = new ChannelAddressParser();

        Assert.AreEqual(expected, sut.ToChannelUrl(text));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("https://example.com/@matthew_berman")]
    [DataRow("https://notyoutube.com/@x")]
    [DataRow("https://youtube.com.evil.example/@x")]
    [DataRow("ftp://www.youtube.com/@x")]
    [DataRow("https://www.youtube.com/")]
    [DataRow("@bad handle")]
    [DataRow("two words")]
    public void RejectsWhatIsClearlyNotYouTube(string text)
    {
        var sut = new ChannelAddressParser();

        Assert.IsNull(sut.ToChannelUrl(text), $"\"{text}\" must not be accepted");
    }
}