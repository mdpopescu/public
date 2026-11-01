using YouNews.Core.Model;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.YouTube;

[TestClass]
public sealed class VideoErrorClassifierTests
{
    [TestMethod]
    [DataRow("ERROR: [youtube] ZerTreaAgPA: Join this channel to get access to members-only content like this video", VideoAvailability.PermanentlyUnavailable, "Join this channel to get access to members-only content like this video")]
    [DataRow("ERROR: [youtube] abc: Private video. Sign in if you've been granted access to this video", VideoAvailability.PermanentlyUnavailable, "Private video. Sign in if you've been granted access to this video")]
    [DataRow("ERROR: [youtube] abc: Video unavailable", VideoAvailability.PermanentlyUnavailable, "Video unavailable")]
    [DataRow("ERROR: [youtube] abc: Video unavailable. This video has been removed by the uploader", VideoAvailability.PermanentlyUnavailable, "Video unavailable. This video has been removed by the uploader")]
    [DataRow("ERROR: [youtube] abc: This video has been deleted", VideoAvailability.PermanentlyUnavailable, "This video has been deleted")]
    [DataRow("ERROR: [youtube] abc: This video is no longer available because the YouTube account associated with this video has been terminated.", VideoAvailability.PermanentlyUnavailable, "This video is no longer available because the YouTube account associated with this video has been terminated.")]
    [DataRow("ERROR: [youtube] abc: Sign in to confirm you're not a bot", VideoAvailability.RecheckLater, "Sign in to confirm you're not a bot")]
    [DataRow("ERROR: [youtube] abc: Premieres in 3 hours", VideoAvailability.RecheckLater, "Premieres in 3 hours")]
    [DataRow("ERROR: [youtube] abc: This live event will begin in 2 days.", VideoAvailability.RecheckLater, "This live event will begin in 2 days.")]
    [DataRow("ERROR: Unable to download webpage: <urlopen error [Errno 11001] getaddrinfo failed>", VideoAvailability.RecheckLater, "Unable to download webpage: <urlopen error [Errno 11001] getaddrinfo failed>")]
    public void ClassifiesFailureAndExtractsReason(string standardError, VideoAvailability expected, string expectedReason)
    {
        var sut = new VideoErrorClassifier();

        var (availability, reason) = sut.Classify(standardError + "\n", 1);

        Assert.AreEqual(expected, availability, $"stderr was: {standardError}");
        Assert.AreEqual(expectedReason, reason);
    }

    [TestMethod]
    public void BotCheckThatMentionsUnavailableIsStillTransient()
    {
        var sut = new VideoErrorClassifier();

        var (availability, _) = sut.Classify("ERROR: [youtube] abc: Sign in to confirm you're not a bot. Video unavailable", 1);

        Assert.AreEqual(VideoAvailability.RecheckLater, availability);
    }

    [TestMethod]
    public void EmptyErrorOutputStillGivesAReason()
    {
        var sut = new VideoErrorClassifier();

        var (availability, reason) = sut.Classify("", 5);

        Assert.AreEqual(VideoAvailability.RecheckLater, availability);
        StringAssert.Contains(reason, "exit code 5");
    }
}