using YouNews.Core.Briefing;
using YouNews.Core.Model;

namespace YouNews.Core.Tests.Briefing;

[TestClass]
public sealed class TranscriptFormatterTests
{
    [TestMethod]
    public void MarksAboutEveryThirtySecondsAndKeepsAllText()
    {
        var sut = new TranscriptFormatter();
        var transcript = new Transcript(
        [
            new TranscriptSegment(TimeSpan.FromSeconds(0), "one"),
            new TranscriptSegment(TimeSpan.FromSeconds(10), "two\nlines"),
            new TranscriptSegment(TimeSpan.FromSeconds(29), "three"),
            new TranscriptSegment(TimeSpan.FromSeconds(30), "four"),
            new TranscriptSegment(TimeSpan.FromSeconds(59), "five"),
            new TranscriptSegment(TimeSpan.FromSeconds(75), "six"),
        ]);

        var result = sut.Format(transcript);

        Assert.AreEqual("[00:00] one two lines three\n[00:30] four five\n[01:15] six", result);
    }

    [TestMethod]
    public void UsesHoursOnlyFromTheFirstHour()
    {
        var sut = new TranscriptFormatter();
        var transcript = new Transcript(
        [
            new TranscriptSegment(new TimeSpan(0, 59, 59), "before"),
            new TranscriptSegment(new TimeSpan(1, 2, 3), "after"),
            new TranscriptSegment(new TimeSpan(12, 0, 5), "long stream"),
        ]);

        var result = sut.Format(transcript);

        Assert.AreEqual("[59:59] before\n[1:02:03] after\n[12:00:05] long stream", result);
    }
}