using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.YouTube;

[TestClass]
public sealed class Json3ParserTests
{
    public const string AUTO_CAPTIONS = """
        {"wireMagic":"pb3","events":[
          {"tStartMs":0,"dDurationMs":1200000,"id":1,"wpWinPosId":1},
          {"tStartMs":240,"dDurationMs":4560,"wWinId":1,"segs":[{"utf8":"hello","acAsrConf":0},{"utf8":" everyone","tOffsetMs":400}]},
          {"tStartMs":2600,"dDurationMs":2200,"wWinId":1,"aAppend":1,"segs":[{"utf8":"\n"}]},
          {"tStartMs":2610,"dDurationMs":4000,"segs":[{"utf8":"welcome   to\nthe  channel"}]},
          {"tStartMs":9000,"aAppend":1,"segs":[{"utf8":"\n"}]},
          {"tStartMs":12500,"segs":[{"utf8":"  "},{"utf8":"\n"}]},
          {"tStartMs":65250,"segs":[{"utf8":"one minute in"}]}
        ]}
        """;

    [TestMethod]
    public void ReadsTextWithTimestampsAndSkipsEmptyEvents()
    {
        var sut = new Json3Parser();

        var segments = sut.Parse(AUTO_CAPTIONS);

        CollectionAssert.AreEqual(
            new[] { "hello everyone", "welcome to the channel", "one minute in" },
            segments.Select(segment => segment.Text).ToArray(),
            string.Join(" | ", segments.Select(segment => segment.Text)));
        CollectionAssert.AreEqual(
            new[] { TimeSpan.FromMilliseconds(240), TimeSpan.FromMilliseconds(2610), TimeSpan.FromMilliseconds(65250) },
            segments.Select(segment => segment.Start).ToArray());
    }

    [TestMethod]
    public void FileWithoutEventsGivesNoSegments()
    {
        var sut = new Json3Parser();

        var segments = sut.Parse("""{"wireMagic":"pb3"}""");

        Assert.IsEmpty(segments);
    }

    [TestMethod]
    public void EventWithANullStartIsReadFromTheBeginning()
    {
        var sut = new Json3Parser();

        var segments = sut.Parse("""{"events":[{"tStartMs":null,"segs":[{"utf8":"no start"}]}]}""");

        Assert.AreEqual(TimeSpan.Zero, segments.Single().Start);
        Assert.AreEqual("no start", segments.Single().Text);
    }
}