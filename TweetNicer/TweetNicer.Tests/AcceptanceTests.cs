using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetNicer.Library;
using TweetNicer.Library.Services;

namespace TweetNicer.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void GetsListOfTweets()
    {
      var authenticator = new TweetInviAuthenticator(Constants.API_KEY, Constants.API_SECRET);
      var api = authenticator.Authenticate();

      var tweets = api.GetTweets().ToList();

      Assert.AreNotEqual(0, tweets.Count);
    }
  }
}